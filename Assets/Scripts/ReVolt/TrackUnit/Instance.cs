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
    public class Instance : IBinSerializable
    {
        public int UnitID;
        public int Direction;
        public Vector2Int Position;
        public int Elevation;

        public void ReadBinary(BinaryReader reader)
        {
            this.UnitID = reader.ReadUInt16();
            this.Direction = reader.ReadUInt16();
            this.Position = new Vector2Int(reader.ReadInt16(), reader.ReadInt16());
            this.Elevation = reader.ReadInt16();
        }

        public void WriteBinary(BinaryWriter writer)
        {
            writer.Write((ushort)UnitID);
            writer.Write((ushort)Direction);
            
            writer.Write((short)Position.x);
            writer.Write((short)Position.y);

            writer.Write((short)Elevation);
        }
    }
}