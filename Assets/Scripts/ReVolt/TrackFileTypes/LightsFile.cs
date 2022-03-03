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
    public enum LightFlags
    {
        None = 0,
        Fixed = 1,
        Moving = 2,
        File = 4,
        Off = 8,
        Flicker = 16
    }

    public struct Light : IBinSerializable
    {
        public Vector3 Position;
        public float Reach;
        public Matrix4x4 DirectionMatrix;
        public float Cone;
        public Color Color;
        public LightFlags Flags;
        public byte Type;
        public byte Speed;       

        public void ReadBinary(BinaryReader reader)
        {
            Position = reader.ReadVector3();
            Reach = reader.ReadSingle();
            DirectionMatrix = reader.ReadMatrix3x3();
            Cone = reader.ReadSingle();
            Color = reader.ReadColor24FB();
            Flags = (LightFlags)reader.ReadByte();
            Type = reader.ReadByte();
            Speed = reader.ReadByte();
            reader.ReadByte(); // padding byte
        }

        public void WriteBinary(BinaryWriter writer)
        {
            writer.WriteVector3(Position);
            writer.Write(Reach);
            writer.WriteMatrix3x3(DirectionMatrix);
            writer.Write(Cone);
            writer.WriteColor24FB(Color);
            writer.Write((byte)Flags);
            writer.Write(Type);
            writer.Write(Speed);
            writer.Write((byte)0x00); // padding byte
        }
    }

    public class LightsFile : IBinSerializable, ISaveLoad
    {
        public readonly List<Light> Lights = new List<Light>();

        public void Scale(float scale)
        {
            for(int i=0; i < Lights.Count; i++)
            {
                var light = Lights[i];
                light.Reach *= scale;
                light.Position *= scale;
                Lights[i] = light;
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
            int lightsCount = reader.ReadInt32();
            for(int i=0; i < lightsCount; i++)
            {
                var light = new Light();
                light.ReadBinary(reader);
                Lights.Add(light);
            }
        }

        public void WriteBinary(BinaryWriter writer)
        {
            writer.Write(Lights.Count);
            foreach(var light in Lights)
            {
                light.WriteBinary(writer);
            }
        }

        // constructors
        public LightsFile(BinaryReader reader)
        {
            ReadBinary(reader);
        }

        public LightsFile(string filePath)
        {
            using var br = new BinaryReader(File.OpenRead(filePath));
            ReadBinary(br);
        }

        public LightsFile() { }
    }
}
