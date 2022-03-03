/*
    Re-Volt Track Editor - Unity Edition
    A version of the track editor re-built from the ground up in Unity
    Copyright (C) 2022 Dummiesman

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class EditorTrack : IBinSerializable, ISaveLoad
{
    public string Name = string.Empty;
    public int Width { get; private set; }
    public int Height { get; private set; }
    public bool IsDirty => isDirty;
    public EditorTrackCell[] Cells
    {
        get => cells;
        private set
        {
            cells = value;
        }
    }

    // 
    private EditorTrackCell[] cells = null;
    private bool isDirty = false;

    // events
    public Action AfterLoadCallback;
    public Action AfterSaveCallback;

    // constants
    private const int TDF_MAGIC = 0x20464454; //'TDF '
    private const ushort TDF_VERSION_2 = 2;
    private const ushort TDF_VERSION_1 = 1;
    private const int MODULE_NULL = 32767;

    // helpers
    private EditorTrackCell[] CreateCellArray(int width, int height)
    {
        var array = new EditorTrackCell[width * height];
        for (int i = 0; i < array.Length; i++)
        {
            int x = i % width;
            int y = i / width;
            array[i] = new EditorTrackCell(new Vector2Int(x, y));
        }
        return array;
    }

    // public interface
    public void MarkDirty()
    {
        isDirty = true;
    }

    public void ClearDirtyFlag()
    {
        isDirty = false;
    }

    public EditorTrackCell GetCell(Vector2Int cell)
    {
        return GetCell(cell.x, cell.y);
    }

    public EditorTrackCell GetCell(int x, int y)
    {
        if (x >= Width || y >= Height || x < 0 || y < 0)
            throw new ArgumentException($"Requested cell outside grid! ({x}/{Width}, {y}/{Height})", "cell");
        return cells[(y * Width) + x];
    }

    public void SetSize(int width, int height)
    {
        // don't resize if this is already our size
        if (Width == width && Height == height)
            return;
        AdjustTrack(new Vector2Int(width, height), Vector2Int.zero);
    }

    public void SetSize(Vector2Int size)
    {
        SetSize(size.x, size.y);
    }

    public void AdjustTrack(Vector2Int newSize, Vector2Int shift)
    {
        //keep copy of old data
        int oldWidth = this.Width;
        int oldHeight = this.Height;
        var oldCells = cells;

        //
        this.Width = newSize.x;
        this.Height = newSize.y;

        //init new cells
        cells = CreateCellArray(newSize.x, newSize.y);

        //copy over old cell data, but shifted
        if (oldCells == null)
            return;

        for (int y = 0; y < oldHeight; y++)
        {
            for (int x = 0; x < oldWidth; x++)
            {
                var originalCell = oldCells[(y * oldWidth) + x];
                var shiftedCellPos = new Vector2Int(x + shift.x, y + shift.y);

                if(shiftedCellPos.x >= 0 && shiftedCellPos.y >= 0 && shiftedCellPos.x < Width && shiftedCellPos.y < Height)
                {
                    //copy over pickup
                    var shiftedCell = GetCell(shiftedCellPos);
                    shiftedCell.HasPickup = originalCell.HasPickup;

                    //copy over module
                    if (originalCell.Module != null && originalCell.Module.RootCell == originalCell)
                    {
                        //create shifted data
                        shiftedCell.Module = new ModulePlacement()
                        {
                            RootCell = shiftedCell,
                            Position = shiftedCell.Position,
                            Elevation = originalCell.Module.Elevation,
                            ModuleIndex = originalCell.Module.ModuleIndex,
                            Object = originalCell.Module.Object,
                            Rotation = originalCell.Module.Rotation
                        };

                        foreach (var touchedCell in originalCell.Module.TouchedCells)
                        {
                            var newTouchedCell = GetCell(touchedCell.Position + shift);
                            newTouchedCell.Module = shiftedCell.Module;
                            shiftedCell.Module.TouchedCells.Add(newTouchedCell);
                        }

                        //shift the actual object
                        shiftedCell.Module.PositionObject();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Get all ModulePlacement's where the modules origin resides
    /// </summary>
    public IEnumerable<ModulePlacement> GetAllModuleRootPlacements()
    {
        return cells.Where(x => x.Module != null && x.Module.RootCell == x).Select(x => x.Module);
    }

    public void Clear()
    {
        foreach(var cell in cells)
        {
            if (cell.Module != null && cell.Module.RootCell == cell)
                cell.Module.Delete();
        }
    }

    public EditorTrack Clone(bool keepObjectReferences)
    {
        var track = new EditorTrack(Width, Height)
        {
            Name = this.Name
        };

        foreach(var rootModulePlacement in this.GetAllModuleRootPlacements())
        {
            var newRootCell = track.GetCell(rootModulePlacement.Position);
            var newPlacement = new ModulePlacement()
            {
                Elevation = rootModulePlacement.Elevation,
                ModuleIndex = rootModulePlacement.ModuleIndex,
                Object = (keepObjectReferences) ? rootModulePlacement.Object : null,
                Position = rootModulePlacement.Position,
                RootCell = newRootCell,
                Rotation = rootModulePlacement.Rotation
            };
            newRootCell.Module = newPlacement;

            foreach(var touchedCell in rootModulePlacement.TouchedCells)
            {
                var newTouchedCell = track.GetCell(touchedCell.Position);
                newTouchedCell.HasPickup = touchedCell.HasPickup;
                newTouchedCell.Module = newPlacement;
            }
        }
        return track;
    }

    // File IO
    public void Save(string filepath)
    {
        using (var bw = new BinaryWriter(File.Open(filepath, FileMode.Create)))
            WriteBinary(bw);
    }

    public void Load(string filepath)
    {
        using (var br = new BinaryReader(File.OpenRead(filepath)))
            ReadBinary(br);
    }

    public void WriteBinary(BinaryWriter writer)
    {
        writer.Write(TDF_MAGIC);
        writer.Write(TDF_VERSION_2);

        writer.WritePaddedString(Name);

        writer.Write((ushort)0); // ThemeType, unused
        writer.Write((int)1); // SectionsUsed, unused

        writer.Write((ushort)Width);
        writer.Write((ushort)Height);

        // write modules
        for(int i=0; i < cells.Length; i++)
        {
            var cell = cells[i];

            if (cell.Module != null && cell.Module.RootCell == cell)
            {
                writer.Write((ushort)cell.Module.ModuleIndex);
                writer.Write((ushort)cell.Module.Rotation);
                writer.Write((ushort)(cell.Module.Elevation / EditorConstants.ElevationStep));
            }
            else
            {
                writer.Write((ushort)MODULE_NULL);
                writer.Write((ushort)0);
                writer.Write((ushort)0);
            }
        }

        //write pickups
        var cellsWithPickups = cells.Where(x => x.HasPickup);

        writer.Write((ushort)cellsWithPickups.Count());
        foreach(var cell in cellsWithPickups)
        {
            writer.Write((ushort)cell.Position.x);
            writer.Write((ushort)cell.Position.y);
        }

        //
        AfterSaveCallback?.Invoke();
    }

    public void ReadBinaryHeader(BinaryReader reader)
    {
        // read header
        int magic = reader.ReadInt32();
        if (magic != 0x20464454)
        {
            throw new InvalidDataException("Wrong TDF magic.");
        }

        int version = reader.ReadUInt16();
        if (version != TDF_VERSION_2 && version != TDF_VERSION_1)
        {
            throw new InvalidDataException("Wrong TDF version.");
        }

        Name = reader.ReadPaddedString();
    }

    public void ReadBinary(BinaryReader reader)
    {
        // read header
        int magic = reader.ReadInt32();
        if(magic != TDF_MAGIC)
        {
            throw new InvalidDataException("Wrong TDF magic.");
        }

        int version = reader.ReadUInt16();
        if(version != TDF_VERSION_2 && version != TDF_VERSION_1)
        {
            throw new InvalidDataException("Wrong TDF version.");
        }

        Name = reader.ReadPaddedString();

        // read grid info
        int themeType = reader.ReadUInt16(); // ThemeType, unused
        int sectionsUsed = reader.ReadInt32(); // SectionsUsed, unused

        int gridSizeX = reader.ReadUInt16();
        int gridSizeY = reader.ReadUInt16();

        SetSize(gridSizeX, gridSizeY);

        // read module info
        for(int i=0; i < Width * Height; i++)
        {
            int moduleId = reader.ReadUInt16();
            int direction = reader.ReadUInt16();
            int elevation = reader.ReadUInt16();

            if(moduleId != MODULE_NULL && moduleId != (int)Modules.ID.TWM_SPACE)
            {
                int y = i % this.Width;
                int x = i / this.Width;
                var cell = cells[i];

                cell.Module = new ModulePlacement()
                {
                    RootCell = cell,
                    Elevation = elevation * EditorConstants.ElevationStep,
                    Object = null,
                    ModuleIndex = moduleId,
                    Position = new Vector2Int(x, y),
                    Rotation = direction
                };
            }
        }

        if(version >= TDF_VERSION_2)
        {
            int numPickups = reader.ReadUInt16();
            for(int i=0; i < numPickups; i++)
            {
                int xPos = reader.ReadUInt16();
                int yPos = reader.ReadUInt16();
                GetCell(xPos, yPos).HasPickup = true;
            }
        }

        AfterLoadCallback?.Invoke();
    }

    public EditorTrack(int width, int height)
    {
        SetSize(width, height);
    }

    public EditorTrack()
    {
        cells = new EditorTrackCell[0];
        Width = 0;
        Height = 0;
    }
}
