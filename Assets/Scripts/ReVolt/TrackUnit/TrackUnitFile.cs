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

namespace ReVolt.TrackUnit
{
    public class TrackUnitFile : IBinSerializable
    {
        public const int MAX_MODULE_ROUTES = 2;
        private const int MAGIC = 0x20555452;
        private const ushort VERSION = 13;

        public readonly List<Vector3> Vertices = new List<Vector3>();
        public readonly List<Vector2> UVs = new List<Vector2>();

        public readonly List<Mesh> Meshes = new List<Mesh>();
        public readonly List<Polygon> Polygons = new List<Polygon>();
        public readonly List<ColorPolygon> ColorPolygons = new List<ColorPolygon>();
        public readonly List<Polygon> UVPolygons = new List<Polygon>();
        public readonly List<PolySet> PolySets = new List<PolySet>();

        public readonly List<Unit> Units = new List<Unit>();
        public readonly List<Module> Modules = new List<Module>();

        public int TPageCount;
        public int WallIndex;

        public void ReadBinary(BinaryReader reader)
        {
            int magic = reader.ReadInt32();
            ushort version = reader.ReadUInt16();
            if (magic != MAGIC)
                throw new InvalidDataException("Incorrect RTU magic.");
            if (version != VERSION)
                throw new InvalidDataException("Incorrect trackunit version.");

            Vertices.Clear();
            UVs.Clear();
            Meshes.Clear();
            Polygons.Clear();
            ColorPolygons.Clear();
            UVPolygons.Clear();
            PolySets.Clear();
            Units.Clear();
            Modules.Clear();

            reader.BaseStream.Seek(2, SeekOrigin.Current); // unused VALID_TARGETS?

            int vertexCount = reader.ReadUInt16();
            for (int i = 0; i < vertexCount; i++)
            {
                Vertices.Add(reader.ReadVector3());
            }

            int polyCount = reader.ReadUInt16();
            for (int i = 0; i < polyCount; i++)
            {
                var poly = new Polygon();
                poly.ReadBinary(reader);
                Polygons.Add(poly);
            }

            int rgbPolyCount = reader.ReadUInt16();
            for (int i = 0; i < rgbPolyCount; i++)
            {
                var rgbPoly = new ColorPolygon();
                rgbPoly.ReadBinary(reader);
                ColorPolygons.Add(rgbPoly);
            }

            int polySetCount = reader.ReadUInt16();
            for (int i = 0; i < polySetCount; i++)
            {
                var polySet = new PolySet();
                polySet.ReadBinary(reader);
                PolySets.Add(polySet);
            }

            int meshCount = reader.ReadUInt16();
            for (int i = 0; i < meshCount; i++)
            {
                var mesh = new Mesh();
                mesh.ReadBinary(reader);
                Meshes.Add(mesh);
            }

            int uvCount = reader.ReadUInt16();
            for (int i = 0; i < uvCount; i++)
            {
                UVs.Add(reader.ReadVector2());
            }

            int uvPolyCount = reader.ReadUInt16();
            for (int i = 0; i < uvPolyCount; i++)
            {
                var poly = new Polygon();
                poly.ReadBinary(reader);
                UVPolygons.Add(poly);
            }

            int unitCount = reader.ReadUInt16();
            for (int i = 0; i < unitCount; i++)
            {
                var unit = new Unit();
                unit.ReadBinary(reader);
                Units.Add(unit);

                // the original trackunit file had garbage surface indices
                unit.TrimExcessSurfaces(this);
            }

            int moduleCount = reader.ReadUInt16();
            for (int i = 0; i < moduleCount; i++)
            {
                var module = new Module();
                module.ReadBinary(reader);
                Modules.Add(module);

                // set AI node priorities
                // this is really yucky
                foreach(var route in module.Routes)
                {
                    foreach(var node in route.Nodes)
                    {
                        node.Priority = global::Modules.Lookup.ModulePriority[i];
                    }
                }
            }

            this.TPageCount = reader.ReadUInt16();
            this.WallIndex = reader.ReadUInt16();
        }

        public void WriteBinary(BinaryWriter writer)
        {
            writer.Write(MAGIC);
            writer.Write(VERSION);
            
            writer.Write((ushort)1); // unused VALID_TARGETS?

            writer.Write((ushort)Vertices.Count);
            for(int i=0; i < Vertices.Count; i++)
            {
                writer.WriteVector3(Vertices[i]);
            }

            writer.Write((ushort)Polygons.Count);
            for (int i = 0; i < Polygons.Count; i++)
            {
                Polygons[i].WriteBinary(writer);
            }
            
            writer.Write((ushort)ColorPolygons.Count);
            for (int i = 0; i < ColorPolygons.Count; i++)
            {
                ColorPolygons[i].WriteBinary(writer);
            }

            writer.Write((ushort)PolySets.Count);
            for (int i = 0; i < PolySets.Count; i++)
            {
                PolySets[i].WriteBinary(writer);
            }

            writer.Write((ushort)Meshes.Count);
            for (int i = 0; i < Meshes.Count; i++)
            {
                Meshes[i].WriteBinary(writer);
            }

            writer.Write((ushort)UVs.Count);
            for (int i = 0; i < UVs.Count; i++)
            {
                writer.WriteVector2(UVs[i]);
            }

            writer.Write((ushort)UVPolygons.Count);
            for (int i = 0; i < UVPolygons.Count; i++)
            {
                UVPolygons[i].WriteBinary(writer);
            }

            writer.Write((ushort)Units.Count);
            for (int i = 0; i < Units.Count; i++)
            {
                Units[i].WriteBinary(writer);
            }

            writer.Write((ushort)Modules.Count);
            for (int i = 0; i < Modules.Count; i++)
            {
                Modules[i].WriteBinary(writer);
            }

            writer.Write((ushort)TPageCount);
            writer.Write((ushort)WallIndex);
        }

        public TrackUnitFile() { }
        public TrackUnitFile(BinaryReader reader)
        {
            ReadBinary(reader);
        }

        public TrackUnitFile(Stream stream)
        {
            ReadBinary(new BinaryReader(stream));
        }
    }
}



