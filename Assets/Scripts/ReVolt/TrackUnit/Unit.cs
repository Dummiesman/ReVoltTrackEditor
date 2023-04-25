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
    public class Unit : IBinSerializable
    {
        public int MeshID;
        public readonly List<UVPolyInstance> UVPolys = new List<UVPolyInstance>();
        public readonly List<int> Surfaces = new List<int>();
        public readonly List<int> RGBs = new List<int>();
        public Vector3 PickupPosition;
        public int RootEdges;

        public  bool CheckRootEdge(int direction)
        {
            return (RootEdges & (0x0008 >> direction)) != 0;
        }
       
        public void TrimExcessSurfaces(TrackUnitFile parentFile)
        {
            var mesh = parentFile.Meshes[MeshID];
            if (RVConstants.HULL_INDEX >= mesh.PolySets.Count)
            {
                return;
            }

            var polySet = parentFile.PolySets[mesh.PolySets[RVConstants.HULL_INDEX]];
            while(Surfaces.Count > polySet.PolygonIndices.Count)
            {
                Surfaces.RemoveAt(0);
            }
        }

        public void ReadBinary(BinaryReader reader)
        {
            this.MeshID = reader.ReadUInt16();

            UVPolys.Clear();
            Surfaces.Clear();
            RGBs.Clear();

            int uvPolyCount = reader.ReadUInt16();
            for(int i=0; i < uvPolyCount; i++)
            {
                var uvPoly = new UVPolyInstance();
                uvPoly.ReadBinary(reader);
                UVPolys.Add(uvPoly);
            }

            int surfaceCount = reader.ReadUInt16();
            for(int i=0; i < surfaceCount; i++)
            {
                int surface = reader.ReadByte();
                Surfaces.Add(surface);
            }

            PickupPosition = reader.ReadVector3();

            int rgbCount = reader.ReadUInt16();
            for(int i=0; i < rgbCount; i++)
            {
                int rgb = reader.ReadUInt16();
                RGBs.Add(rgb);
            }

            this.RootEdges = reader.ReadUInt16();
        }

        public void WriteBinary(BinaryWriter writer)
        {
            writer.Write((ushort)MeshID);

            writer.Write((ushort)UVPolys.Count);
            for(int i=0; i < UVPolys.Count; i++)
            {
                UVPolys[i].WriteBinary(writer);
            }

            writer.Write((ushort)Surfaces.Count);
            for(int i=0; i < Surfaces.Count; i++)
            {
                writer.Write((byte)Surfaces[i]);
            }

            writer.WriteVector3(PickupPosition);

            writer.Write((ushort)RGBs.Count);
            for(int i=0; i < RGBs.Count; i++)
            {
                writer.Write((ushort)RGBs[i]);
            }

            writer.Write((ushort)RootEdges);
        }
    }
}