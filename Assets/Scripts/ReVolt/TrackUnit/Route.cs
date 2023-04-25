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

namespace ReVolt.TrackUnit
{
    public class Route : IBinSerializable
    {
        public readonly List<AINode> Nodes = new List<AINode>();

        public void ReadBinary(BinaryReader reader)
        {
            Nodes.Clear();

            int nodeCount = reader.ReadUInt16();
            for (int i = 0; i < nodeCount; i++)
            {
                var node = new AINode();
                node.ReadBinary(reader);
                Nodes.Add(node);
            }
        }

        public void WriteBinary(BinaryWriter writer)
        {
            writer.Write((ushort)Nodes.Count);
            for(int i=0; i < Nodes.Count; i++)
            {
                Nodes[i].WriteBinary(writer);
            }
        }
    }
}