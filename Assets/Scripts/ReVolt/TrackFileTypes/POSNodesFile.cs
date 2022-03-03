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
    public struct POSNode : IBinSerializable
    {
        public Vector3 Position;
        public float Distance;

        public int[] PreviousNodeIDs; // [4], -1 for none
        public int[] NextNodeIDs; // [4], -1 for none

        public void ReadBinary(BinaryReader reader)
        {
            Position = reader.ReadVector3();
            Distance = reader.ReadSingle();

            PreviousNodeIDs = new int[4];
            for (int i = 0; i < 4; i++)
                PreviousNodeIDs[i] = reader.ReadInt32();

            NextNodeIDs = new int[4];
            for (int i = 0; i < 4; i++)
                NextNodeIDs[i] = reader.ReadInt32();
        }

        public void WriteBinary(BinaryWriter writer)
        {
            writer.WriteVector3(Position);
            writer.Write(Distance);

            for (int i = 0; i < 4; i++)
                writer.Write(PreviousNodeIDs[i]);
            for (int i = 0; i < 4; i++)
                writer.Write(NextNodeIDs[i]);
        }
    }

    public class POSNodesFile : IBinSerializable, ISaveLoad
    {
        public readonly List<POSNode> Nodes = new List<POSNode>();
        public float TotalLength;
        public int StartNodeIndex;

        public void Scale(float scale)
        {
            TotalLength *= scale;
            for(int i=0; i < Nodes.Count; i++)
            {
                var node = Nodes[i];
                node.Distance *= scale;
                node.Position *= scale;
                Nodes[i] = node;
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
            int nodeCount = reader.ReadInt32();
            StartNodeIndex = reader.ReadInt32();
            TotalLength = reader.ReadSingle();

            for(int i=0; i < nodeCount; i++)
            {
                var node = new POSNode();
                node.ReadBinary(reader);
                Nodes.Add(node);
            }
        }

        public void WriteBinary(BinaryWriter writer)
        {
            writer.Write(Nodes.Count);
            writer.Write(StartNodeIndex);
            writer.Write(TotalLength);

            for (int i = 0; i < Nodes.Count; i++)
                Nodes[i].WriteBinary(writer);
        }

        // constructors
        public POSNodesFile(BinaryReader reader)
        {
            ReadBinary(reader);
        }

        public POSNodesFile(string filePath)
        {
            using var br = new BinaryReader(File.OpenRead(filePath));
            ReadBinary(br);
        }

        public POSNodesFile() { }
    }
}

