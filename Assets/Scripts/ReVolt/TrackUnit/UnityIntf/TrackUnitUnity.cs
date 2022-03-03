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
using System.Linq;
using UnityEngine;

namespace ReVolt.TrackUnit.Unity
{
    public class SubmeshBuilder
    {
        public IReadOnlyList<Vector3> vertices => verticesList;
        public IReadOnlyList<Vector2> uvs => uvsList;
        public IReadOnlyList<int> indices => indicesList;

        private List<Vector3> verticesList = new List<Vector3>(256);
        private List<Vector2> uvsList = new List<Vector2>(256);
        private List<int> indicesList = new List<int>(768);

        public void AddTri(Vector3 v0, Vector3 v1, Vector3 v2, Vector2 uv0, Vector2 uv1, Vector2 uv2)
        {
            int baseIndex = verticesList.Count;
            indicesList.Add(baseIndex + 2);
            indicesList.Add(baseIndex + 1);
            indicesList.Add(baseIndex + 0);

            verticesList.Add(new Vector3(v0.x, -v0.y, v0.z));
            verticesList.Add(new Vector3(v1.x, -v1.y, v1.z));
            verticesList.Add(new Vector3(v2.x, -v2.y, v2.z));

            uvsList.Add(new Vector2(uv0.x, 1f - uv0.y));
            uvsList.Add(new Vector2(uv1.x, 1f - uv1.y));
            uvsList.Add(new Vector2(uv2.x, 1f - uv2.y));
        }

        public void AddQuad(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Vector2 uv0, Vector2 uv1, Vector2 uv2, Vector2 uv3)
        {
            AddTri(v0, v1, v2, uv0, uv1, uv2);
            AddTri(v0, v2, v3, uv0, uv2, uv3);
        }
    }

    public class UnitMeshBuilder
    {
        public IReadOnlyDictionary<int, int> SubmeshToTPageMap => submeshToTPageMap;
        public int SubmeshCount => currentSubmesh;

        private TrackUnitFile unitFile;
        private Unit unit;
        
        private int polySetIndex = 0;

        private List<SubmeshBuilder> submeshBuilders = new List<SubmeshBuilder>();
        private Dictionary<int, int> submeshToTPageMap = new Dictionary<int, int>();
        private Dictionary<int, int> tpageToSubmeshMap = new Dictionary<int, int>();
        private int currentSubmesh = 0;

        private void AddPoly(Polygon poly, Polygon uvPoly, UVPolyInstance uvPolyInstance)
        {
            int tpage = (uvPolyInstance != null) ? (uvPolyInstance.TPage & ~RVConstants.UV_REVERSED) : 0;

            int uvIndex = 0;
            int uvStep = -1;
            int uModValue = 4;

            if(!tpageToSubmeshMap.TryGetValue(tpage, out int subBuilderIndex))
            {
                subBuilderIndex = currentSubmesh;
                tpageToSubmeshMap[tpage] = currentSubmesh;
                submeshToTPageMap[currentSubmesh] = tpage;

                var builder = new SubmeshBuilder();
                submeshBuilders.Add(builder);

                currentSubmesh++;
            }
            
            if (uvPolyInstance != null)
            {
                uModValue = uvPoly.Indices.Length;
                uvIndex = uvPolyInstance.Rotation;
                uvStep = ((uvPolyInstance.TPage & RVConstants.UV_REVERSED) != 0) ? (uModValue - 1) : 1;
            }

            var subBuilder = submeshBuilders[subBuilderIndex];
            Vector3[] positions = new Vector3[poly.Indices.Length];
            Vector2[] uvs = new Vector2[poly.Indices.Length];
            for (int i = 0; i < poly.Indices.Length; i++)
            {
                positions[i] = unitFile.Vertices[poly.Indices[i]];
                if (uvPoly != null)
                    uvs[i] = unitFile.UVs[uvPoly.Indices[uvIndex]];
                else
                    uvs[i] = Vector2.zero;

                uvIndex = (uvIndex + uvStep) % uModValue;
            }

            if (poly.Indices.Length == 4)
            {
                subBuilder.AddQuad(positions[0], positions[1], positions[2], positions[3],
                                   uvs[0], uvs[1], uvs[2], uvs[3]);
            }
            else
            {
                subBuilder.AddTri(positions[0], positions[1], positions[2],
                                   uvs[0], uvs[1], uvs[2]);
            }
        }

        public UnityEngine.Mesh Build()
        {
            var tuMesh = unitFile.Meshes[unit.MeshID];
            var polySet = unitFile.PolySets[tuMesh.PolySets[polySetIndex]];

            for (int i = 0; i < polySet.PolygonIndices.Count; i++)
            {
                int polyIndex = polySet.PolygonIndices[i] & ~RVConstants.GOURAUD_SHIFTED;
                var poly = unitFile.Polygons[polyIndex];

                UVPolyInstance uvPolyInstance = null;
                Polygon uvPoly = null;

                if (polySetIndex == RVConstants.PAN_INDEX)
                {
                    uvPolyInstance = unit.UVPolys[i];
                    uvPoly = unitFile.UVPolygons[uvPolyInstance.PolyID];
                }

                AddPoly(poly, uvPoly, uvPolyInstance);
            }

            // combine submeshes
            List<Vector3> combinedVertsList = new List<Vector3>();
            List<Vector2> combinedUvsList = new List<Vector2>();
            List<List<int>> remappedIndicesLists = new List<List<int>>();
            for(int i=0; i < submeshBuilders.Count; i++)
            {
                int offset = combinedVertsList.Count;

                combinedVertsList.AddRange(submeshBuilders[i].vertices);
                combinedUvsList.AddRange(submeshBuilders[i].uvs);

                var submeshIndices = submeshBuilders[i].indices;
                List<int> offsetSubmeshIndices = new List<int>();
                
                for(int j=0; j < submeshIndices.Count; j++)
                {
                    int index = submeshIndices[j];
                    offsetSubmeshIndices.Add(index + offset);
                }
                remappedIndicesLists.Add(offsetSubmeshIndices);
            }

            // create unity mesh and return
            var mesh = new UnityEngine.Mesh();
            mesh.subMeshCount = submeshBuilders.Count;

            mesh.vertices = combinedVertsList.ToArray();
            mesh.uv = combinedUvsList.ToArray();
            
            for(int i=0; i < mesh.subMeshCount; i++)
            {
                mesh.SetTriangles(remappedIndicesLists[i], i);
            }
            
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        public UnitMeshBuilder(TrackUnitFile unitFile, Unit unit, int polySetIndex)
        {
            this.unitFile = unitFile;
            this.unit = unit;
            this.polySetIndex = polySetIndex;
        }
    }

    public class UnitObjectBuilder
    {
        private readonly TrackUnitFile unitFile;
        private readonly Unit unit;
        private readonly UnityEngine.Mesh rootMesh;
        private readonly Material[] tpageMaterials;

        public GameObject Build()
        {
            GameObject meshObj = new GameObject("Unit");
            var tuMesh = unitFile.Meshes[unit.MeshID];

            // create geometry
            for (int i = 0; i < tuMesh.PolySets.Count; i++)
            {
                var childChildObj = new GameObject();
                childChildObj.transform.parent = meshObj.transform;

                if (i == RVConstants.PEG_INDEX)
                    childChildObj.name = "Peg";
                else if (i == RVConstants.PAN_INDEX)
                    childChildObj.name = "Pan";
                else if (i == RVConstants.HULL_INDEX)
                    childChildObj.name = "Hull";

                var mr = childChildObj.AddComponent<MeshRenderer>();
                var mf = childChildObj.AddComponent<MeshFilter>();

                var meshBuilder = new UnitMeshBuilder(unitFile, unit, i);
                mf.sharedMesh = meshBuilder.Build();

                //setup materials
                Material[] materials = new Material[meshBuilder.SubmeshCount];
                for(int j=0; j < meshBuilder.SubmeshCount; j++)
                {
                    int tpage = meshBuilder.SubmeshToTPageMap[j];
                    if (tpage < tpageMaterials.Length)
                        materials[j] = tpageMaterials[tpage];
                }
                mr.sharedMaterials = materials;
            }

            // create root
            var rootObj = new GameObject("Root");
            rootObj.transform.parent = meshObj.transform;

            var rootMf = rootObj.AddComponent<MeshFilter>();
            var rootMr = rootObj.AddComponent<MeshRenderer>();

            rootMf.sharedMesh = this.rootMesh;
            rootMr.sharedMaterial = tpageMaterials[0];

            return meshObj;
        }

        public UnitObjectBuilder(TrackUnitFile unitFile, Unit unit, UnityEngine.Mesh rootMesh, Material[] trackUnitMaterials)
        {
            this.tpageMaterials = trackUnitMaterials;
            this.unitFile = unitFile;
            this.unit = unit;
            this.rootMesh = rootMesh;
        }
    }

    public class TrackUnitUnity
    {
        public TrackUnitFile UnitFile { get; private set; }

        public readonly List<GameObject> UnityUnits = new List<GameObject>();
        public readonly List<GameObject> UnityModules = new List<GameObject>();

        private Material pegMaterial;
        private Material[] trackUnitMaterials = new Material[RVConstants.TPAGE_COUNT + 1];

        private UnityEngine.Mesh rootMesh;

        public void CreateUnits()
        {
            GameObject unitRoot = new GameObject("CachedUnits");
            for (int i=0; i < UnitFile.Units.Count; i++)
            {
                var unit = UnitFile.Units[i];

                var unitBuilder = new UnitObjectBuilder(UnitFile, unit, rootMesh, trackUnitMaterials);
                var unitObj = unitBuilder.Build();
                var unitInst = unitObj.AddComponent<UnitInstance>();
                unitInst.IndexInFile = i;

                unitInst.Init(unitObj);
                unitObj.transform.SetParent(unitRoot.transform, false);
                unitObj.SetActive(false);
                UnityUnits.Add(unitObj);
            }
        }

        public void CreateModules()
        {
            GameObject modRoot = new GameObject("CachedModules");
            for (int i=0; i < UnitFile.Modules.Count; i++)
            {
                var module = UnitFile.Modules[i];

                GameObject modObj = new GameObject("Module");
                var modInst = modObj.AddComponent<ModuleInstance>();
                modInst.IndexInFile = i;
                
                foreach (var instance in module.Instances)
                {
                    Vector3 unitOffset = new Vector3(instance.Position.x * RVConstants.SMALL_CUBE_SIZE, 0f, instance.Position.y * RVConstants.SMALL_CUBE_SIZE);

                    GameObject childObj = Object.Instantiate(UnityUnits[instance.UnitID]);
                    childObj.SetActive(true);
                    childObj.transform.parent = modObj.transform;
                    childObj.transform.localPosition = unitOffset;
                }

                modInst.Init(modObj);
                modObj.transform.SetParent(modRoot.transform, false);
                modObj.transform.localScale = Vector3.one / RVConstants.SMALL_CUBE_SIZE;
                modObj.SetActive(false);
                UnityModules.Add(modObj);
            }
        }

        public TrackUnitUnity(TrackUnitFile unitFile)
        {
            this.UnitFile = unitFile;

            // create materials
            Shader d3dShader = Shader.Find("Custom/D3D7");
            pegMaterial = new Material(d3dShader) { 
                name = "PegMaterial", 
                color = EditorConstants.RootColor 
            };

            trackUnitMaterials[0] = pegMaterial;
            for(int i=1; i <= RVConstants.TPAGE_COUNT; i++)
            {
                var tpageMat = new Material(d3dShader)
                {
                    name = $"TPAGE{i}",
                    mainTexture = TextureCache.GetTPage(i)
                };
                trackUnitMaterials[i] = tpageMat;
            }

            // create root mesh
            rootMesh = new UnityEngine.Mesh
            {
                name = "Root",
                subMeshCount = 1
            };

            Vector3[] rootVerts = new Vector3[16];
            float rootExtents = RVConstants.SMALL_CUBE_HALF;

            rootVerts[00] = new Vector3(-rootExtents, -1, rootExtents);
            rootVerts[01] = new Vector3(rootExtents, -1, rootExtents);
            rootVerts[02] = new Vector3(rootExtents, -1, rootExtents);
            rootVerts[03] = new Vector3(rootExtents, -1, -rootExtents);
            rootVerts[04] = new Vector3(rootExtents, -1, -rootExtents);
            rootVerts[05] = new Vector3(-rootExtents, -1, -rootExtents);
            rootVerts[06] = new Vector3(-rootExtents, -1, -rootExtents);
            rootVerts[07] = new Vector3(-rootExtents, -1, rootExtents);
            rootVerts[08] = new Vector3(-rootExtents, 0, rootExtents);
            rootVerts[09] = new Vector3(rootExtents, 0, rootExtents);
            rootVerts[10] = new Vector3(rootExtents, 0, rootExtents);
            rootVerts[11] = new Vector3(rootExtents, 0, -rootExtents);
            rootVerts[12] = new Vector3(rootExtents, 0, -rootExtents);
            rootVerts[13] = new Vector3(-rootExtents, 0, -rootExtents);
            rootVerts[14] = new Vector3(-rootExtents, 0, -rootExtents);
            rootVerts[15] = new Vector3(-rootExtents, 0, rootExtents);

            int[] rootIndices =  new int[] { 0, 1, 8, 1, 9, 8,
                                             2, 3, 10, 3, 11, 10,
                                             4, 5, 12, 5, 13, 12,
                                             6, 7, 14, 7, 15, 14 };

            rootMesh.vertices = rootVerts.ToArray();
            rootMesh.triangles = rootIndices;

            rootMesh.RecalculateBounds();
            rootMesh.RecalculateNormals();
        }
    }
}