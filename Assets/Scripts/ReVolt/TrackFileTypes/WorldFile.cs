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
using System.Linq;
using UnityEngine;

namespace ReVolt.Track
{
    [System.Flags]
    public enum PolygonFlags
    {
        None = 0x00,
        Quad = 0x01,
        DoubleSided = 0x02,
        Mirror  = 0x04,
        Additive = 0x100,
        EnvMapped = 0x800
    }

    public struct Polygon
    {
        public PolygonFlags Flags;
        public short TextureNum;
        public int[] VertexIndices;
        public Color[] Colors;
        public Vector2[] UVCoordinates;

        public Polygon Clone()
        {
            return new Polygon()
            {
                Flags = this.Flags,
                TextureNum = this.TextureNum,
                VertexIndices = new int[] { this.VertexIndices[0], this.VertexIndices[1], this.VertexIndices[2], this.VertexIndices[3] },
                Colors = new Color[] { this.Colors[0], this.Colors[1], this.Colors[2], this.Colors[3] },
                UVCoordinates = new Vector2[] { this.UVCoordinates[0], this.UVCoordinates[1], this.UVCoordinates[2], this.UVCoordinates[3] }
            };
        }
    }

    public class SmallCube : IBinSerializable
    {
        public Vector3 Center;
        public float Radius;

        public Vector3 Min;
        public Vector3 Max;

        public readonly List<Polygon> Polygons = new List<Polygon>();
        public readonly List<Vertex> Vertices = new List<Vertex>();

        public void CalculateBounds()
        {
            // center
            Vector3 avg = Vector3.zero;
            foreach(var vertex in Vertices)
            {
                avg += vertex.Position;
            }
            this.Center = avg / Vertices.Count;
            this.Min = this.Center;
            this.Max = this.Center;

            // radius, min, max
            this.Radius = 0f;
            foreach(var vertex in this.Vertices)
            {
                float dist = Vector3.Distance(vertex.Position, this.Center);
                this.Min = Vector3.Min(this.Min, vertex.Position);
                this.Max = Vector3.Max(this.Max, vertex.Position);
                this.Radius = Mathf.Max(this.Radius, dist);
            }
        }

        public IEnumerable<SmallCube> SplitCube(float splitSize)
        {
            int numXCells = Mathf.CeilToInt((Max.x - Min.x) / splitSize);
            int numYCells = Mathf.CeilToInt((Max.z - Min.z) / splitSize);

            SmallCube[] splitCubes = new SmallCube[numXCells * numYCells];
            foreach(var poly in Polygons)
            {
                int vertCount = ((poly.Flags & PolygonFlags.Quad) != 0) ? 4 : 3;
                
                Vector3 average = Vector3.zero;
                for(int i=0; i < vertCount; i++)
                {
                    average += Vertices[poly.VertexIndices[i]].Position;
                }
                average /= vertCount;

                int polyBucketX = Mathf.Min(Mathf.FloorToInt((average.x - Min.x) / splitSize), numXCells - 1);
                int polyBucketY = Mathf.Min(Mathf.FloorToInt((average.z - Min.z) / splitSize), numYCells - 1);
                int bucketIndex = (polyBucketY * numXCells) + polyBucketX;

                var cube = splitCubes[bucketIndex];
                if (cube == null)
                {
                    cube = new SmallCube();
                    splitCubes[bucketIndex] = cube;
                }

                int vertBase = cube.Vertices.Count;
                var clonedPoly = poly.Clone();

                for (int i=0; i < vertCount; i++)
                {
                    clonedPoly.VertexIndices[i] = vertBase++;
                    cube.Vertices.Add(Vertices[poly.VertexIndices[i]]);
                }
                cube.Polygons.Add(clonedPoly);
            }


            foreach(var cube in splitCubes.Where(x =>  x != null))
            {
                cube.CalculateBounds();
                yield return cube;
            }
        }

        public void WriteBinary(BinaryWriter writer)
        {
            writer.WriteVector3(Center);
            writer.Write(Radius);

            writer.Write(Min.x);
            writer.Write(Max.x);
            writer.Write(Min.y);
            writer.Write(Max.y);
            writer.Write(Min.z);
            writer.Write(Max.z);

            writer.Write((ushort)Polygons.Count);
            writer.Write((ushort)Vertices.Count);

            foreach(var polygon in Polygons)
            {
                writer.Write((ushort)polygon.Flags);
                writer.Write(polygon.TextureNum);

                for(int i=0; i < 4; i++)
                    writer.Write((ushort)polygon.VertexIndices[i]);
                for (int i = 0; i < 4; i++)
                    writer.WriteColorBGRA32B(polygon.Colors[i]);
                for (int i = 0; i < 4; i++)
                    writer.WriteVector2(polygon.UVCoordinates[i]);
            }

            for(int i=0; i < Vertices.Count; i++)
            {
                writer.WriteVector3(Vertices[i].Position);
                writer.WriteVector3(Vertices[i].Normal);
            }
        }

        public void ReadBinary(BinaryReader reader)
        {
            Center = reader.ReadVector3();
            Radius = reader.ReadSingle();

            Min.x = reader.ReadSingle();
            Max.x = reader.ReadSingle();

            Min.y = reader.ReadSingle();
            Max.y = reader.ReadSingle();

            Min.z = reader.ReadSingle();
            Max.z = reader.ReadSingle();

            int polyCount = reader.ReadUInt16();
            int vertexCount = reader.ReadUInt16();

            for(int i=0; i < polyCount; i++)
            {
                var poly = new Polygon()
                {
                    Flags = (PolygonFlags)reader.ReadUInt16(),
                    TextureNum = reader.ReadInt16(),
                    VertexIndices = new int[4],
                    Colors = new Color[4],
                    UVCoordinates = new Vector2[4]
                };

                for (int j = 0; j < 4; j++)
                    poly.VertexIndices[j] = reader.ReadUInt16();
                for (int j = 0; j < 4; j++)
                    poly.Colors[j] = reader.ReadColorBGRA32B();
                for (int j = 0; j < 4; j++)
                    poly.UVCoordinates[j] = reader.ReadVector2();

                Polygons.Add(poly);
            }

            for(int i=0; i < vertexCount; i++)
            {
                Vertices.Add(new Vertex()
                {
                    Position = reader.ReadVector3(),
                    Normal = reader.ReadVector3()
                });
            }
        }
    }

    public class BigCube : IBinSerializable
    {
        public Vector3 Center;
        public float Radius;
        public readonly List<int> SmallCubeIndices = new List<int>();

        public void ReadBinary(BinaryReader reader)
        {
            Center = reader.ReadVector3();
            Radius = reader.ReadSingle();

            int indexCount = reader.ReadInt32();
            SmallCubeIndices.Clear();
            for (int i = 0; i < indexCount; i++)
                SmallCubeIndices.Add(reader.ReadInt32());
        }

        public void WriteBinary(BinaryWriter writer)
        {
            writer.WriteVector3(Center);
            writer.Write(Radius);

            writer.Write(SmallCubeIndices.Count);
            for (int i = 0; i < SmallCubeIndices.Count; i++)
                writer.Write(SmallCubeIndices[i]);
        }
    }

    public class WorldFile : IBinSerializable, ISaveLoad
    {
        public readonly List<SmallCube> SmallCubes = new List<SmallCube>();
        public readonly List<BigCube> BigCubes = new List<BigCube>();

        public void GenerateBigCubes(float size)
        {
            if (SmallCubes.Count == 0)
                return;

            // find the minimum bounds of the sum of the small cubes
            Vector3 min = SmallCubes[0].Min;
            Vector3 max = SmallCubes[0].Max;
            
            for(int i=1; i < SmallCubes.Count; i++)
            {
                min = Vector3.Min(min, SmallCubes[i].Min);
                max = Vector3.Max(max, SmallCubes[i].Max);
            }

            // create big cubes
            int numYCubes = Mathf.CeilToInt((max.z - min.z) / size);
            int numXCubes = Mathf.CeilToInt((max.x - min.x) / size);
            float yCenter = (min.y + max.y) / 2f;

            BigCubes.Clear();

            for(int y=0; y < numYCubes; y++)
            {
                float zMin = min.z + (y * size);
                float zMax = min.z + ((y + 1) * size);
                float zCenter = (zMin + zMax) / 2f;

                for (int x=0; x < numXCubes; x++)
                {
                    float xMin = min.x + (x * size);
                    float xMax = min.x + ((x + 1) * size);
                    float xCenter = (xMin + xMax) / 2f;

                    BigCubes.Add(new BigCube()
                    {
                        Center = new Vector3(xCenter, yCenter, zCenter),
                        Radius = 0f
                    });
                }
            }

            // map small cubes to big cubes
            bool intersects(Rect r1, Rect r2, out float area)
            {
                area = 0f;
                if (r2.Overlaps(r1))
                {
                    float x1 = Mathf.Min(r1.xMax, r2.xMax);
                    float x2 = Mathf.Max(r1.xMin, r2.xMin);
                    float y1 = Mathf.Min(r1.yMax, r2.yMax);
                    float y2 = Mathf.Max(r1.yMin, r2.yMin);

                    area = Mathf.Max(0.0f, x1 - x2) * Mathf.Max(0.0f, y1 - y2);
                    return true;
                }
                return false;
            }

            for (int i=0; i < SmallCubes.Count; i++)
            {
                int bestCubeMatch = -1;
                float bestIsectAmount = 0f;

                Vector2 smallCubeMin = SmallCubes[i].Min.ToVec2XZ();
                Vector2 smallCubeMax = SmallCubes[i].Max.ToVec2XZ();

                for (int j=0; j < BigCubes.Count; j++)
                {
                    Vector2 bigCubeMin = BigCubes[j].Center.ToVec2XZ() - new Vector2(size / 2f, size / 2f);
                    Vector2 bigCubeMax = bigCubeMin + new Vector2(size, size);

                    var smallCubeRect = Rect.MinMaxRect(smallCubeMin.x, smallCubeMin.y, smallCubeMax.x, smallCubeMax.y);
                    var bigCubeRect = Rect.MinMaxRect(bigCubeMin.x, bigCubeMin.y, bigCubeMax.x, bigCubeMax.y);

                    if (intersects(smallCubeRect, bigCubeRect, out float overlap ))
                    {
                        if (bestCubeMatch < 0 || overlap > bestIsectAmount)
                        {
                            bestIsectAmount = overlap;
                            bestCubeMatch = j;
                        }
                    }
                }

                if(bestCubeMatch >= 0)
                {
                    BigCubes[bestCubeMatch].SmallCubeIndices.Add(i);
                }
                else
                {
                    Debug.LogError($"Failed to map a smallcube to a bigcube!!");
                }
            }

            // finally, adjust bigcubes radius
            foreach (var bigCube in BigCubes.Where(x => x.SmallCubeIndices.Count > 0))
            {
                Vector3 cubesSumMin = SmallCubes[bigCube.SmallCubeIndices[0]].Min;
                Vector3 cubesSumMax = SmallCubes[bigCube.SmallCubeIndices[0]].Max;
                for(int i=1; i < bigCube.SmallCubeIndices.Count; i++)
                {
                    var smallCube = SmallCubes[bigCube.SmallCubeIndices[i]];
                    cubesSumMin = Vector3.Min(cubesSumMin, smallCube.Min);
                    cubesSumMax = Vector3.Max(cubesSumMax, smallCube.Max);
                }
                bigCube.Radius = Mathf.Max(Vector3.Distance(bigCube.Center, cubesSumMin),
                                           Vector3.Distance(bigCube.Center, cubesSumMax));
            }
        }

        public void Scale(float scale)
        {
            foreach (var cube in SmallCubes)
            {
                cube.Center *= scale;
                cube.Max *= scale;
                cube.Min *= scale;
                cube.Radius *= scale;
                for (int i = 0; i < cube.Vertices.Count; i++)
                    cube.Vertices[i] = new ReVolt.Track.Vertex(cube.Vertices[i].Position * scale,
                                                               cube.Vertices[i].Normal);
            }
            foreach(var cube in BigCubes)
            {
                cube.Center *= scale;
                cube.Radius *= scale;
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
            int smallCubesCount = reader.ReadInt32();
            for(int i=0; i < smallCubesCount; i++)
            {
                var cube = new SmallCube();
                cube.ReadBinary(reader);
                SmallCubes.Add(cube);
            }

            int bigCubeCount = reader.ReadInt32();
            for(int i=0; i < bigCubeCount; i++)
            {
                var cube = new BigCube();
                cube.ReadBinary(reader);
                BigCubes.Add(cube);
            }

            if (reader.BaseStream.Position == reader.BaseStream.Length)
                return;

            int numTexAnims = reader.ReadInt32();
        }

        public void WriteBinary(BinaryWriter writer)
        {
            writer.Write(SmallCubes.Count);
            for (int i = 0; i < SmallCubes.Count; i++)
                SmallCubes[i].WriteBinary(writer);

            writer.Write(BigCubes.Count);
            for (int i = 0; i < BigCubes.Count; i++)
                BigCubes[i].WriteBinary(writer);

            writer.Write(0); // texanim count
        }

        // constructors
        public WorldFile(BinaryReader reader)
        {
            ReadBinary(reader);
        }

        public WorldFile(string filePath)
        {
            using var br = new BinaryReader(File.OpenRead(filePath));
            ReadBinary(br);
        }

        public WorldFile() { }
    }
}