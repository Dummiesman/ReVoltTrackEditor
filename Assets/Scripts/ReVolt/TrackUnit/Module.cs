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
using System.Collections.Generic;
using UnityEngine;

namespace ReVolt.TrackUnit
{
    public class Module : IBinSerializable
    {
        public readonly List<Instance> Instances = new List<Instance>();
        public readonly List<Zone> Zones = new List<Zone>();
        public readonly List<Route> Routes = new List<Route>();
        public readonly List<Light> Lights = new List<Light>();

        public RectInt CalculateModuleGridSize()
        {
            var rect = new RectInt(Vector2Int.zero, Vector2Int.zero);
            foreach(var instance in Instances)
            {
                var pos = instance.Position;
                rect.min = Vector2Int.Min(rect.min, pos);
                rect.max = Vector2Int.Max(rect.max, pos + Vector2Int.one);
            }

            return rect;   
        }

        public void ReadBinary(BinaryReader reader)
        {
            Instances.Clear();
            Zones.Clear();
            Routes.Clear();
            Lights.Clear();

            int instanceCount = reader.ReadUInt16();
            for (int i = 0; i < instanceCount; i++)
            {
                var instance = new Instance();
                instance.ReadBinary(reader);
                Instances.Add(instance);
            }

            int zoneCount = reader.ReadUInt16();
            for (int i = 0; i < zoneCount; i++)
            {
                var zone = new Zone();
                zone.ReadBinary(reader);
                Zones.Add(zone);
            }

            for (int i = 0; i < TrackUnitFile.MAX_MODULE_ROUTES; i++)
            {
                var route = new Route();
                route.ReadBinary(reader);
                if (route.Nodes.Count > 0)
                    Routes.Add(route);
            }

            int lightCount = reader.ReadUInt16();
            for (int i = 0; i < lightCount; i++)
            {
                var light = new Light();
                light.ReadBinary(reader);
                Lights.Add(light);
            }
        }

        public void WriteBinary(BinaryWriter writer)
        {
            writer.Write((ushort)Instances.Count);
            for(int i=0; i < Instances.Count; i++)
            {
                Instances[i].WriteBinary(writer);
            }

            writer.Write((ushort)Zones.Count);
            for (int i = 0; i < Zones.Count; i++)
            {
                Zones[i].WriteBinary(writer);
            }

            for (int i = 0; i < TrackUnitFile.MAX_MODULE_ROUTES; i++)
            {
                var route = (i < Routes.Count) ? Routes[i] : new Route();
                route.WriteBinary(writer);
            }

            writer.Write((ushort)Lights.Count);
            for (int i = 0; i < Lights.Count; i++)
            {
                Lights[i].WriteBinary(writer);
            }
        }
    }
}