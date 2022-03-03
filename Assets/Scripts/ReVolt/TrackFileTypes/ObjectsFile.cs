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
    public enum ObjectType
    {
        None = 0,
        Pickup = 30
    }

    public struct Object : IBinSerializable
    {
        public ObjectType Type;
        public int Flags1;
        public int Flags2;
        public int Flags3;
        public int Flags4;
        public Vector3 Position;
        public Matrix4x4 DirectionMatrix;

        public void ReadBinary(BinaryReader reader)
        {
            Type = (ObjectType)reader.ReadInt32();
            Flags1 = reader.ReadInt32();
            Flags2 = reader.ReadInt32();
            Flags3 = reader.ReadInt32();
            Flags4 = reader.ReadInt32();

            Position = reader.ReadVector3();

            Vector3 up = reader.ReadVector3();
            Vector3 forward = reader.ReadVector3();
            Vector3 sideways = Vector3.Cross(forward, up);

            DirectionMatrix = Matrix4x4.identity;
            DirectionMatrix.SetColumn(0, sideways);
            DirectionMatrix.SetColumn(1, up);
            DirectionMatrix.SetColumn(2, forward);
        }

        public void WriteBinary(BinaryWriter writer)
        {
            writer.Write((int)Type);
            writer.Write(Flags1);
            writer.Write(Flags2);
            writer.Write(Flags3);
            writer.Write(Flags4);

            writer.WriteVector3(Position);

            writer.WriteVector3(DirectionMatrix.GetColumn(1));
            writer.WriteVector3(DirectionMatrix.GetColumn(2));
        }
    }

    public class ObjectsFile : IBinSerializable, ISaveLoad
    {
        public readonly List<Object> Objects = new List<Object>();

        public void Scale(float scale)
        {
            for(int i=0; i < Objects.Count; i++)
            {
                var @object = Objects[i];
                @object.Position *= scale;
                Objects[i] = @object;
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
            int objectsCount = reader.ReadInt32();
            for (int i = 0; i < objectsCount; i++)
            {
                var @object = new Object();
                @object.ReadBinary(reader);
                Objects.Add(@object);
            }
        }

        public void WriteBinary(BinaryWriter writer)
        {
            writer.Write(Objects.Count);
            foreach (var @object in Objects)
            {
                @object.WriteBinary(writer);
            }
        }

        // constructors
        public ObjectsFile(BinaryReader reader)
        {
            ReadBinary(reader);
        }

        public ObjectsFile(string filePath)
        {
            using var br = new BinaryReader(File.OpenRead(filePath));
            ReadBinary(br);
        }

        public ObjectsFile() { }
    }
}