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

using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ReVolt.Track
{
    [System.Flags]
    public enum CollisionPolyFlags
    {
        None = 0,
        Quad = 1,
        ObjectOnly = 4,
        CameraOnly = 8
    }

    public enum PhysicsMaterial
    {
        Default = 0,
        Marble = 1,
        Stone = 2,
        Wood = 3,
        Sand = 4,
        Plastic = 5,
        Carpettile = 6,
        Carpetshag = 7,
        Boundary = 8,
        Glass = 9,
        Ice1 = 10,
        Metal = 11,
        Grass = 12,
        Bumpmetal = 13,
        Pebbles = 14,
        Gravel = 15,
        Conveyor1 = 16,
        Conveyor2 = 17,
        Dirt1 = 18,
        Dirt2 = 19,
        Dirt3 = 20,
        Ice2 = 21,
        Ice3 = 22,
        Wood2 = 23,
        Conveyor_Market1 = 24,
        Conveyor_Market2 = 25,
        Paving = 26
    }

    public class LookupGrid : IBinSerializable
    {
        public Vector2 Origin;
        public Vector2Int Size;
        public float CellSize;
        public List<int>[] PolyIndices;

        public void ReadBinary(BinaryReader reader)
        {
            Origin = reader.ReadVector2();

            var floatSize = reader.ReadVector2();
            Size = new Vector2Int((int)floatSize.x, (int)floatSize.y);
            CellSize = reader.ReadSingle();

            PolyIndices = new List<int>[Size.x * Size.y];
            for(int i=0; i < PolyIndices.Length; i++)
            {
                int count = reader.ReadInt32();
                var list = new List<int>();
                
                PolyIndices[i] = list;
                for (int j = 0; j < count; j++)
                    list.Add(reader.ReadInt32()); 
            }
        }

        public void WriteBinary(BinaryWriter writer)
        {
            writer.WriteVector2(Origin);
            writer.Write((float)Size.x);
            writer.Write((float)Size.y);
            writer.Write(CellSize);

            for(int i=0; i < PolyIndices.Length; i++)
            {
                writer.Write(PolyIndices[i].Count);
                for (int j = 0; j < PolyIndices[i].Count; j++)
                    writer.Write(PolyIndices[i][j]);
            }
        }

        public LookupGrid(int width, int height)
        {
            Size = new Vector2Int(width, height);
            PolyIndices = new List<int>[Size.x * Size.y];
            for (int i = 0; i < PolyIndices.Length; i++)
            {
                PolyIndices[i] = new List<int>();
            }
        }

        public LookupGrid() { }
    }

    public struct CollisionPolyPlane
    {
        public Vector3 Normal;
        public float Distance;

        const float PLANE_TOLERANCE = 0.01f;

        public bool ApproximatelyEquals(CollisionPolyPlane other)
        {
            // use individual vector component comparison as it's ever so slightly faster than a dot product
            return Mathf.Abs(Distance - other.Distance) <= PLANE_TOLERANCE
                   && Mathf.Abs(Normal.x - other.Normal.x) <= PLANE_TOLERANCE 
                   && Mathf.Abs(Normal.y - other.Normal.y) <= PLANE_TOLERANCE 
                   && Mathf.Abs(Normal.z - other.Normal.z) <= PLANE_TOLERANCE;
        }

        public bool ApproximatelyInvEquals(CollisionPolyPlane other)
        {
            // use individual vector component comparison as it's ever so slightly faster than a dot product
            return Mathf.Abs(Distance - -other.Distance) <= PLANE_TOLERANCE
                   && Mathf.Abs(Normal.x - -other.Normal.x) <= PLANE_TOLERANCE 
                   && Mathf.Abs(Normal.y - -other.Normal.y) <= PLANE_TOLERANCE 
                   && Mathf.Abs(Normal.z - -other.Normal.z) <= PLANE_TOLERANCE;
        }
    }

    public struct CollisionPoly : IBinSerializable
    {
        public CollisionPolyFlags Type;
        public PhysicsMaterial Material;
        public CollisionPolyPlane[] Planes; // [5], index 0 is the main plane, 1-4 are the side planes
        public Vector3 Min;
        public Vector3 Max;

        public bool Contains(CollisionPolyPlane plane)
        {
            int checkPlanes = ((this.Type & CollisionPolyFlags.Quad) != 0) ? 4 : 3;
            for (int i = 0; i < checkPlanes; i++)
            {
                if (Planes[i+1].ApproximatelyEquals(plane))
                    return true;
            }
            return false;
        }

        public void ReadBinary(BinaryReader reader)
        {
            Type = (CollisionPolyFlags)reader.ReadUInt32();
            Material = (PhysicsMaterial)reader.ReadInt32();

            Planes = new CollisionPolyPlane[5];
            for(int i=0; i < Planes.Length; i++)
            {
                var plane = Planes[i];
                plane.Normal = reader.ReadVector3();
                plane.Distance = reader.ReadSingle();
            }

            //
            Min.x = reader.ReadSingle();
            Max.x = reader.ReadSingle();

            Min.y = reader.ReadSingle();
            Max.y = reader.ReadSingle();

            Min.z = reader.ReadSingle();
            Max.z = reader.ReadSingle();
        }

        public void WriteBinary(BinaryWriter writer)
        {
            writer.Write((int)Type);
            writer.Write((int)Material);
            
            for(int i=0; i < 5; i++)
            {
                var plane = Planes[i];
                writer.WriteVector3(plane.Normal);
                writer.Write(plane.Distance);
            }

            //
            writer.Write(Min.x);
            writer.Write(Max.x);

            writer.Write(Min.y);
            writer.Write(Max.y);

            writer.Write(Min.z);
            writer.Write(Max.z);
        }
    }

    public class CollisionFile : IBinSerializable, ISaveLoad
    {
        public readonly List<CollisionPoly> Polyhedrons = new List<CollisionPoly>();
        public LookupGrid Grid = null;

        public void Scale(float scale)
        {
            for(int i=0; i < Polyhedrons.Count; i++)
            {
                var poly = Polyhedrons[i];
                poly.Min *= scale;
                poly.Max *= scale;

                var planes = poly.Planes;
                for(int j=0; j < 5; j++)
                {
                    planes[j].Distance *= scale;
                }

                Polyhedrons[i] = poly;
            }

            if(Grid != null)
            {
                Grid.CellSize *= scale;
                Grid.Origin *= scale;
            }
        }

        public void CreateGrid(float cellSize, float fudge = 150f)
        {
            /*
                Adaption of gridding code from Huki's Blender addon
                https://gitlab.com/re-volt/re-volt-addon/-/blob/master/io_revolt/rvstruct.py#L1005
            */

            Vector2 min = new Vector2(9999999f, 9999999f);
            Vector2 max = -min;
            foreach (var poly in Polyhedrons)
            {
                min = Vector2.Min(poly.Min.ToVec2XZ(), min);
                max = Vector2.Max(poly.Max.ToVec2XZ(), max);
            }

            Rect bounds = Rect.MinMaxRect(min.x, min.y, max.x, max.y);
            Vector2Int size = new Vector2Int(Mathf.CeilToInt((bounds.xMax - bounds.xMin) / cellSize),
                                             Mathf.CeilToInt((bounds.yMax - bounds.yMin) / cellSize));
            Vector2 origin = new Vector2((bounds.xMin + bounds.xMax - size.x * cellSize) / 2f,
                                         (bounds.yMin + bounds.yMax - size.y * cellSize) / 2f);

            Grid = new LookupGrid(size.x, size.y)
            {
                CellSize = cellSize,
                Origin = origin
            };

            for(int y=0; y < size.y; y++)
            {
                float ylo = origin.y + (y * cellSize) - fudge;
                float yhi = origin.y + ((y + 1) * cellSize) + fudge;

                for(int x=0; x < size.x; x++)
                {
                    float xlo = origin.x + (x * cellSize) - fudge;
                    float xhi = origin.x + ((x + 1) * cellSize) + fudge;

                    var list = Grid.PolyIndices[(y * size.x) + x];
                    for(int i=0; i < Polyhedrons.Count; i++)
                    {
                        var poly = Polyhedrons[i];
                        if (poly.Max.z > ylo && poly.Min.z < yhi && poly.Max.x > xlo && poly.Min.x < xhi)
                        {
                            list.Add(i);
                        }
                    }
                }
            }
        }

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

        public void ReadBinary(BinaryReader reader)
        {
            Polyhedrons.Clear();

            int polyCount = reader.ReadUInt16();
            for(int i=0; i < polyCount; i++)
            {
                var poly = new CollisionPoly();
                poly.ReadBinary(reader);
                Polyhedrons.Add(poly);
            }

            if (reader.BaseStream.Position == reader.BaseStream.Length)
                return;

            Grid = new LookupGrid();
            Grid.ReadBinary(reader);
        }

        public void WriteBinary(BinaryWriter writer)
        {
            writer.Write((ushort)Polyhedrons.Count);

            foreach(var poly in Polyhedrons)
            {
                poly.WriteBinary(writer);
            }

            if (Grid != null)
                Grid.WriteBinary(writer);
        }

        // constructors
        public CollisionFile(BinaryReader reader)
        {
            ReadBinary(reader);
        }

        public CollisionFile(string filePath)
        {
            using var br = new BinaryReader(File.OpenRead(filePath));
            ReadBinary(br);
        }

        public CollisionFile() { }
    }
}