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

using System.IO;
using UnityEngine;

namespace ReVolt.TrackUnit
{
    public class Light
    {
        public Vector3 Position;
        public float Reach;
        public float Cone;
        public Vector3 UpDirection;
        public Vector3 ForwardDirection;
        public Color Color;
        public ushort Type;

        public void ReadBinary(BinaryReader reader)
        {
            this.Position = reader.ReadVector3();
            this.Reach = reader.ReadSingle();
            this.UpDirection = reader.ReadVector3();
            this.ForwardDirection = reader.ReadVector3();
            this.Cone = reader.ReadSingle();
            this.Color = reader.ReadColor24HB();
            this.Type = reader.ReadUInt16();
        }
    }
}