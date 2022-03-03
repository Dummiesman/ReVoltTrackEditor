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

using UnityEngine;

namespace ReVolt.Track
{
    public struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + Position.GetHashCode();
                hash = hash * 31 + Normal.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if(obj is Vertex other)
            {
                return other.Position.Equals(this.Position) && other.Normal.Equals(this.Normal);
            }
            return base.Equals(obj);
        }

        public Vertex(Vector3 position, Vector3 normal)
        {
            Position = position;
            Normal = normal;
        }
    }
}