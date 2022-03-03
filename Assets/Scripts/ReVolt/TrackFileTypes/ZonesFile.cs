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
    public struct Zone : IBinSerializable
    {
        public int ID;
        public Vector3 Position;
        public Matrix4x4 Matrix;
        public Vector3 Size;

        public void ReadBinary(BinaryReader reader)
        {
            ID = reader.ReadInt32();
            Position = reader.ReadVector3();
            Matrix = reader.ReadMatrix3x3();
            Size = reader.ReadVector3();
        }

        public void WriteBinary(BinaryWriter writer)
        {
            writer.Write(ID);
            writer.WriteVector3(Position);
            writer.WriteMatrix3x3(Matrix);
            writer.WriteVector3(Size);
        }
    }

    public class ZonesFile : IBinSerializable, ISaveLoad
    {
        public readonly List<Zone> Zones = new List<Zone>();

        public void Scale(float scale)
        {
            for(int i=0; i < Zones.Count; i++)
            {
                var zone = Zones[i];
                zone.Position *= scale;
                zone.Size *= scale;
                Zones[i] = zone;
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
            int zoneCount = reader.ReadInt32();
            for (int i = 0; i < zoneCount; i++)
            {
                var zone = new Zone();
                zone.ReadBinary(reader);
                Zones.Add(zone);
            }
        }

        public void WriteBinary(BinaryWriter writer)
        {
            writer.Write(Zones.Count);
            for (int i = -0; i < Zones.Count; i++)
            {
                Zones[i].WriteBinary(writer);
            }
        }

        // constructors
        public ZonesFile(BinaryReader reader)
        {
            ReadBinary(reader);
        }

        public ZonesFile(string filePath)
        {
            using var br = new BinaryReader(File.OpenRead(filePath));
            ReadBinary(br);
        }

        public ZonesFile() { }

    }
}