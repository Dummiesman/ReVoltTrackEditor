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
using UnityEngine;

namespace Dummiesman.DebugDraw
{
    public enum DrawType
    {
        INVALID = -1,
        LINES = 0,
        LINE_STRIP = 1,
        QUADS = 2,
        TRIANGLES = 3,
        TRIANGLE_STRIP = 4
    }

    /// <summary>
    /// Immediate style debug drawing that works in runtime and the editor
    /// Run debug draw commands in Update()
    /// Add DebugDrawSceneCallback to show them
    /// (DebugDrawSceneCallback must be first in update order)
    /// </summary>
    public static class DebugDraw
    {
        private class DrawBatch
        {
            public DrawType DrawMode;
            public Material Material;
            public bool Transparent = false;
            public bool Queued = false;

            public List<Vector3> vertexCommands = new List<Vector3>(2048);
            public List<Vector2> texCoordCommands = new List<Vector2>(2048);
            public List<Color> colorCommands = new List<Color>(2048);

            public bool IsEmpty => vertexCommands.Count == 0;

            private Plane lastCameraPlane;

            public Bounds CalculateBounds()
            {
                var bounds = new Bounds(Vector3.zero, Vector3.zero);
                foreach (var pt in vertexCommands)
                {
                    bounds.Encapsulate(pt);
                }

                return bounds;
            }

            /// <summary>
            /// Align tex coord count with vertex counts
            /// </summary>
            public void Align()
            {
                while (vertexCommands.Count > texCoordCommands.Count)
                    texCoordCommands.Add(Vector2.zero);
                Debug.Assert(vertexCommands.Count == texCoordCommands.Count, "VertexCommands.Count == TexCoordCommands.Count");
            }

            public void Clear()
            {
                vertexCommands.Clear();
                texCoordCommands.Clear();
                colorCommands.Clear();
            }

            /// <summary>
            /// Make last TRIANGLE_STRIP or LINE_STRIP degenerate
            /// </summary>
            public void StripEnd()
            {
                if (vertexCommands.Count > 0)
                {
                    vertexCommands.Add(vertexCommands[vertexCommands.Count - 1]);
                    colorCommands.Add(Color.clear);
                }
            }

            private void DrawLineSegment(int startIndex)
            {
                var vertA = vertexCommands[startIndex];
                var vertB = vertexCommands[startIndex + 1];

                // cullline
                if (DebugDraw.PlaneCull)
                {
                    if (!lastCameraPlane.GetSide(vertA) && !lastCameraPlane.GetSide(vertB))
                        return;
                }

                GL.Color(colorCommands[startIndex]);
                if (texCoordCommands.Count == vertexCommands.Count)
                    GL.TexCoord(texCoordCommands[startIndex]);
                GL.Vertex(vertA);

                GL.Color(colorCommands[startIndex + 1]);
                if (texCoordCommands.Count == vertexCommands.Count)
                    GL.TexCoord(texCoordCommands[startIndex + 1]);
                GL.Vertex(vertB);
            }

            private void DrawTriangleStripped(int startIndex, bool flip)
            {
                int indexOffsetB = (flip) ? 1 : 2;
                int indexOffsetC = (flip) ? 2 : 1;

                var vertA = vertexCommands[startIndex];
                var vertB = vertexCommands[startIndex + indexOffsetB];
                var vertC = vertexCommands[startIndex + indexOffsetC];

                // culltri
                if (DebugDraw.PlaneCull)
                {
                    if (!lastCameraPlane.GetSide(vertA) && !lastCameraPlane.GetSide(vertB) && !lastCameraPlane.GetSide(vertC))
                        return;
                }

                GL.Color(colorCommands[startIndex]);
                if (texCoordCommands.Count == vertexCommands.Count)
                    GL.TexCoord(texCoordCommands[startIndex]);
                GL.Vertex(vertA);

                GL.Color(colorCommands[startIndex + indexOffsetB]);
                if (texCoordCommands.Count == vertexCommands.Count)
                    GL.TexCoord(texCoordCommands[startIndex + indexOffsetB]);
                GL.Vertex(vertB);

                GL.Color(colorCommands[startIndex + indexOffsetC]);
                if (texCoordCommands.Count == vertexCommands.Count)
                    GL.TexCoord(texCoordCommands[startIndex + indexOffsetC]);
                GL.Vertex(vertC);
            }

            private void DrawTriangle(int startIndex)
            {
                var vertA = vertexCommands[startIndex];
                var vertB = vertexCommands[startIndex + 1];
                var vertC = vertexCommands[startIndex + 2];

                // culltri
                if (DebugDraw.PlaneCull)
                {
                    if (!lastCameraPlane.GetSide(vertA) && !lastCameraPlane.GetSide(vertB) && !lastCameraPlane.GetSide(vertC))
                        return;
                }

                GL.Color(colorCommands[startIndex]);
                if (texCoordCommands.Count == vertexCommands.Count)
                    GL.TexCoord(texCoordCommands[startIndex]);
                GL.Vertex(vertA);

                GL.Color(colorCommands[startIndex + 1]);
                if (texCoordCommands.Count == vertexCommands.Count)
                    GL.TexCoord(texCoordCommands[startIndex + 1]);
                GL.Vertex(vertB);

                GL.Color(colorCommands[startIndex + 2]);
                if (texCoordCommands.Count == vertexCommands.Count)
                    GL.TexCoord(texCoordCommands[startIndex + 2]);
                GL.Vertex(vertC);
            }

            private void DrawQuad(int startIndex)
            {
                var vertA = vertexCommands[startIndex];
                var vertB = vertexCommands[startIndex + 1];
                var vertC = vertexCommands[startIndex + 2];
                var vertD = vertexCommands[startIndex + 3];

                // cullquad
                if (DebugDraw.PlaneCull)
                {
                    if (!lastCameraPlane.GetSide(vertA) && !lastCameraPlane.GetSide(vertB) &&
                        !lastCameraPlane.GetSide(vertC) && !lastCameraPlane.GetSide(vertD))
                        return;
                }

                // 012
                GL.Color(colorCommands[startIndex]);
                if (texCoordCommands.Count == vertexCommands.Count)
                    GL.TexCoord(texCoordCommands[startIndex]);
                GL.Vertex(vertA);

                GL.Color(colorCommands[startIndex + 1]);
                if (texCoordCommands.Count == vertexCommands.Count)
                    GL.TexCoord(texCoordCommands[startIndex + 1]);
                GL.Vertex(vertB);

                GL.Color(colorCommands[startIndex + 2]);
                if (texCoordCommands.Count == vertexCommands.Count)
                    GL.TexCoord(texCoordCommands[startIndex + 2]);
                GL.Vertex(vertC);

                // 213
                GL.Color(colorCommands[startIndex]);
                if (texCoordCommands.Count == vertexCommands.Count)
                    GL.TexCoord(texCoordCommands[startIndex + 0]);
                GL.Vertex(vertA);

                GL.Color(colorCommands[startIndex + 2]);
                if (texCoordCommands.Count == vertexCommands.Count)
                    GL.TexCoord(texCoordCommands[startIndex + 2]);
                GL.Vertex(vertC);

                GL.Color(colorCommands[startIndex + 3]);
                if (texCoordCommands.Count == vertexCommands.Count)
                    GL.TexCoord(texCoordCommands[startIndex + 3]);
                GL.Vertex(vertD);
            }

            private void DrawLineStrip()
            {
                for (int i = 0; i < vertexCommands.Count - 1; i++)
                {
                    var vertStart = vertexCommands[i];
                    var vertEnd = vertexCommands[i + 1];

                    if (vertStart.Equals(vertEnd))
                    {
                        // degenerate
                        i++;
                        continue;
                    }

                    if ((vertexCommands.Count - i) >= 2)
                        DrawLineSegment(i);
                }
            }

            private void DrawLines()
            {
                for (int i = 0; i < vertexCommands.Count - 1; i += 2)
                {
                    if ((vertexCommands.Count - i) >= 2)
                        DrawLineSegment(i);
                }
            }

            private void DrawQuads()
            {
                int validCount = vertexCommands.Count;
                while ((validCount % 4) != 0)
                    validCount--;

                for (int i = 0; i < validCount; i += 4)
                {
                    DrawQuad(i);
                }
            }

            private void DrawTriangles()
            {
                int validCount = vertexCommands.Count;
                while ((validCount % 3) != 0)
                    validCount--;

                for (int i = 0; i < validCount; i += 3)
                {
                    DrawTriangle(i);
                }
            }

            private void DrawTriangleStrip()
            {
                int validCount = vertexCommands.Count;
                bool flip = false;

                for (int i = 0; i < validCount - 2; i++)
                {
                    if (i > 0)
                    {
                        var vertA = vertexCommands[i];
                        var vertB = vertexCommands[i - 1];
                        if (vertA.Equals(vertB)) // degenerate
                        {
                            // degenerate strip, move on to next line
                            flip = false;
                            i++;
                            continue;
                        }
                    }
                    DrawTriangleStripped(i, flip);
                    flip = !flip;
                }
            }

            public void Draw()
            {
                // set matrix
                GL.PushMatrix();
                GL.MultMatrix(Matrix4x4.identity);


                // begin draw
                switch (DrawMode)
                {
                    case DrawType.LINES:
                    case DrawType.LINE_STRIP:
                        GL.Begin(GL.LINES);
                        break;
                    case DrawType.QUADS:
                    case DrawType.TRIANGLES:
                        GL.Begin(GL.TRIANGLES);
                        break;
                    case DrawType.TRIANGLE_STRIP:
                        GL.Begin(GL.TRIANGLE_STRIP);
                        break;
                }

                // set material pass
                Material.SetPass(0);

                // get camera plane
                if (DebugDraw.PlaneCull)
                {
                    var camera = Camera.current;
                    lastCameraPlane = new Plane(camera.transform.forward, camera.transform.position);
                }

                //go through vert commands
                switch (DrawMode)
                {
                    case DrawType.LINE_STRIP:
                        DrawLineStrip();
                        break;
                    case DrawType.LINES:
                        DrawLines();
                        break;
                    case DrawType.QUADS:
                        DrawQuads();
                        break;
                    case DrawType.TRIANGLES:
                        DrawTriangles();
                        break;
                    case DrawType.TRIANGLE_STRIP:
                        DrawTriangleStrip();
                        break;
                }

                // flush and pop matrix
                GL.End();
                GL.PopMatrix();
            }

            public override bool Equals(object obj)
            {
                if (obj is DrawBatch db)
                {
                    return db.DrawMode == DrawMode &&
                           db.Material?.GetInstanceID() == Material?.GetInstanceID();
                }
                return base.Equals(obj);
            }

            public static int GetHashCodeStatic(DrawType drawMode, Material material, bool transparent)
            {
                int hash = 1009;
                hash = (hash * 9176) + drawMode.GetHashCode();
                hash = (hash * 9176) + material.GetInstanceID();
                hash = (hash * 9176) + (transparent ? 1024 : 128);
                return hash;
            }

            public override int GetHashCode()
            {
                return GetHashCodeStatic(this.DrawMode, this.Material, this.Transparent);
            }

            public DrawBatch(DrawType drawMode, Material material, bool transparent)
            {
                this.Transparent = transparent;
                this.DrawMode = drawMode;
                this.Material = material;
            }
        }

        // Begin DM DebugDraw v2.1
        const int DRAWMODES_SIZE = 5;
        private static DrawBatch[] opaqueBatches = new DrawBatch[DRAWMODES_SIZE]; //5 1+ the enum size for batch type
        private static DrawBatch[] transparentBatches = new DrawBatch[DRAWMODES_SIZE]; //5 1+ the enum size for batch type
        private static Dictionary<Texture, DrawBatch>[] opaqueTextureBatchDicts = new Dictionary<Texture, DrawBatch>[DRAWMODES_SIZE];
        private static Dictionary<Texture, DrawBatch>[] transparentTextureBatchDicts = new Dictionary<Texture, DrawBatch>[DRAWMODES_SIZE];

        private static DrawBatch currentBatch;
        private static bool drawBegan = false;

        // base shaders
        private static readonly Shader unlitShader = Shader.Find("Unlit/Color VC");
        private static readonly Shader unlitTransShader = Shader.Find("Unlit/Color Transparent VC");

        private static readonly Shader unlitTextureShader = Shader.Find("Unlit/Texture Colored VC");
        private static readonly Shader unlitTransTextureShader = Shader.Find("Unlit/Texture Transparent Colored VC");

        // state variables
        const int MAX_MATRIX_STACK = 32;
        private static bool _matrixIsIdentity = true;
        private static Matrix4x4 _matrix = Matrix4x4.identity;
        private static Stack<Matrix4x4> _matrixStack = new Stack<Matrix4x4>(MAX_MATRIX_STACK);
        private static Color _color;

        // state variables
        private static List<DrawBatch> batchQueue = new List<DrawBatch>();
        private static DrawType lastDrawType = DrawType.LINES;
        private static int lastDrawFunc = -1; //Draw=0, DrawTransparent=1, DrawTextured=2, DrawTexturedTransparent=3
        private static Texture lastTexture = null;

        public static bool PlaneCull = true;

        public static Matrix4x4 Matrix
        {
            get
            {
                return _matrix;
            }
            set
            {
                _matrixIsIdentity = value.isIdentity;
                _matrix = value;
            }
        }

        // material factories
        private static Material CreateMaterial()
        {
            return new Material(unlitShader) { name = "PrimDrawMaterial", color = Color.white };
        }

        private static Material CreateTransparentMaterial()
        {
            return new Material(unlitTransShader) { name = "PrimDrawMaterial", color = Color.white };
        }

        private static Material CreateTexturedMaterial(Texture texture)
        {
            return new Material(unlitTextureShader) { mainTexture = texture, color = Color.white, name = "PrimDrawMaterial" };
        }

        private static Material CreateTransparentTexturedMaterial(Texture texture)
        {
            return new Material(unlitTransTextureShader) { mainTexture = texture, color = Color.white, name = "PrimDrawMaterial" };
        }

        // public interface
        public static Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        public static void PushMatrix()
        {
            if (_matrixStack.Count >= MAX_MATRIX_STACK)
            {
                Debug.LogError($"DebugDraw.PushMatrix stack depth >= {MAX_MATRIX_STACK}. There's a mismatch somewhere!");
                return;
            }
            _matrixStack.Push(_matrix);
        }

        public static void PopMatrix()
        {
            if (_matrixStack.Count == 0)
            {
                Debug.LogError($"DebugDraw.PopMatrix stack depth == 0. There's a mismatch somewhere!");
                return;
            }
            _matrix = _matrixStack.Pop();
        }

        /// <summary>
        /// Set matrix to identity
        /// </summary>
        public static void Identity()
        {
            Matrix = Matrix4x4.identity;
        }

        public static void Clear()
        {
            foreach (var batch in batchQueue)
            {
                batch.Queued = false;
                batch.Clear();
            }
            batchQueue.Clear();
        }

        public static void Render()
        {
            // PASS 1 : NON TRANSPARENT
            foreach (var batch in batchQueue)
            {
                if (batch.Transparent || batch.IsEmpty)
                    continue;
                batch.Draw();
            }

            // PASS 2 : TRANSPARENT
            foreach (var batch in batchQueue)
            {
                if (!batch.Transparent || batch.IsEmpty)
                    continue;
                batch.Draw();
            }
        }

        public static void Cleanup()
        {
            for (int i = 0; i < DRAWMODES_SIZE; i++)
            {
                if (opaqueBatches[i] != null)
                    UnityEngine.Object.Destroy(opaqueBatches[i].Material);
                opaqueBatches[i] = null;

                if (transparentBatches[i] != null)
                    UnityEngine.Object.Destroy(transparentBatches[i].Material);
                transparentBatches[i] = null;


                var oD = opaqueTextureBatchDicts[i];
                if (oD != null)
                {
                    foreach (var batch in oD.Values)
                    {
                        UnityEngine.Object.Destroy(batch.Material);
                    }
                }
                opaqueTextureBatchDicts[i] = null;


                var tD = transparentTextureBatchDicts[i];
                if (tD != null)
                {
                    foreach (var batch in tD.Values)
                    {
                        UnityEngine.Object.Destroy(batch.Material);
                    }
                }
                transparentTextureBatchDicts[i] = null;
            }
            lastTexture = null;
        }

        // DRAW THINGS
        static void batchLog(DrawType mode, string tag, DrawBatch batch)
        {
#if UNITY_EDITOR
            if (batch.Material.HasProperty("_MainTex"))
                Debug.Log($"NewBatch ({tag}): tex={batch.Material.mainTexture}");
            else
                Debug.Log($"NewBatch ({tag})");
#endif
        }

        public static void BeginOpaque(DrawType mode)
        {
            Debug.Assert(!drawBegan, "!drawBegan");
            Debug.Assert(mode != DrawType.INVALID, "mode != INVALID");

            // get currentBatch
            if (lastDrawFunc != 0 || lastDrawType != mode)
            {
                int drawModeint = (int)mode;

                currentBatch = opaqueBatches[drawModeint];
                if (currentBatch == null)
                {
                    currentBatch = new DrawBatch(mode, CreateMaterial(), false);
                    batchLog(mode, "Begin", currentBatch);
                    opaqueBatches[drawModeint] = currentBatch;
                }
            }

            lastDrawType = mode;
            lastDrawFunc = 0;
            drawBegan = true;
        }

        public static void BeginTransparent(DrawType mode)
        {
            Debug.Assert(!drawBegan, "!drawBegan");
            Debug.Assert(mode != DrawType.INVALID, "mode != INVALID");

            // get currentBatch
            if (lastDrawFunc != 1 || lastDrawType != mode)
            {
                int drawModeint = (int)mode;

                currentBatch = transparentBatches[drawModeint];
                if (currentBatch == null)
                {
                    currentBatch = new DrawBatch(mode, CreateTransparentMaterial(), true);
                    batchLog(mode, "BeginTransparent", currentBatch);
                    transparentBatches[drawModeint] = currentBatch;
                }
            }

            lastDrawType = mode;
            lastDrawFunc = 1;
            drawBegan = true;
        }

        public static void BeginTextured(DrawType mode, Texture texture)
        {
            Debug.Assert(!drawBegan, "!drawBegan");
            Debug.Assert(mode != DrawType.INVALID, "mode != INVALID");

            // get currentBatch
            if (lastDrawFunc != 2 || lastDrawType != mode || lastTexture != texture)
            {
                int drawModeint = (int)mode;

                if (opaqueTextureBatchDicts[drawModeint] == null)
                {
                    opaqueTextureBatchDicts[drawModeint] = new Dictionary<Texture, DrawBatch>(64);
                }

                var batchSource = opaqueTextureBatchDicts[drawModeint];
                if (!batchSource.TryGetValue(texture, out currentBatch))
                {
                    currentBatch = new DrawBatch(mode, CreateTexturedMaterial(texture), false);
                    batchLog(mode, "BeginTextured", currentBatch);
                    batchSource[texture] = currentBatch;
                }
            }

            lastTexture = texture;
            lastDrawType = mode;
            lastDrawFunc = 2;
            drawBegan = true;
        }

        public static void BeginTexturedTransparent(DrawType mode, Texture texture)
        {
            Debug.Assert(!drawBegan, "!drawBegan");
            Debug.Assert(mode != DrawType.INVALID, "mode != INVALID");

            // get currentBatch
            if (lastDrawFunc != 3 || lastDrawType != mode || lastTexture != texture)
            {
                int drawModeint = (int)mode;

                if (transparentTextureBatchDicts[drawModeint] == null)
                {
                    transparentTextureBatchDicts[drawModeint] = new Dictionary<Texture, DrawBatch>(64);
                }

                var batchSource = transparentTextureBatchDicts[drawModeint];
                if (!batchSource.TryGetValue(texture, out currentBatch))
                {
                    currentBatch = new DrawBatch(mode, CreateTransparentTexturedMaterial(texture), true);
                    batchLog(mode, "BeginTexturedTransparent", currentBatch);
                    batchSource[texture] = currentBatch;
                }
            }

            lastTexture = texture;
            lastDrawType = mode;
            lastDrawFunc = 3;
            drawBegan = true;
        }

        /// <summary>
        /// Begins drawing. Opaque/transparent are automatically chosen based on the alpha of the Color property.
        /// </summary>
        public static void Begin(DrawType mode)
        {
            if (_color.a < 0.99f)
                BeginTransparent(mode);
            else
                BeginOpaque(mode);
        }

        public static void End()
        {
            Debug.Assert(drawBegan && currentBatch != null, "drawBegan && currentBatch != null");

            // align the batch
            if (currentBatch.DrawMode == DrawType.TRIANGLE_STRIP || currentBatch.DrawMode == DrawType.LINE_STRIP)
                currentBatch.StripEnd();
            currentBatch.Align();

            if (!currentBatch.Queued && !currentBatch.IsEmpty)
            {
                currentBatch.Queued = true;
                batchQueue.Add(currentBatch);
            }

            drawBegan = false;
        }

        // VERTEX/TEXCOORD

        public static void Vertex(Vector3 v)
        {
            Debug.Assert(drawBegan && currentBatch != null, "drawBegan && currentBatch != null");

            currentBatch.colorCommands.Add(_color);
            if (_matrixIsIdentity)
                currentBatch.vertexCommands.Add(v);
            else
                currentBatch.vertexCommands.Add(_matrix.MultiplyPoint3x4(v));

            // assume something is leaky in this case
            Debug.Assert(currentBatch.vertexCommands.Count < 65535, "currentBatch.commandCount < 65535");
        }

        public static void Vertex(float x, float y, float z)
        {
            Vertex(new Vector3(x, y, z));
        }

        public static void TexCoord(Vector2 t)
        {
            Debug.Assert(drawBegan && currentBatch != null, "drawBegan && currentBatch != null");

            currentBatch.texCoordCommands.Add(t);
        }

        public static void TexCoord(float u, float v)
        {
            TexCoord(new Vector2(u, v));
        }

        public static void Vertex(Vector3 v, Vector2 t)
        {
            Vertex(v);
            TexCoord(t);
        }

        public static void Vertex(float x, float y, float z, float u, float v)
        {
            Vertex(x, y, z);
            TexCoord(u, v);
        }

        // PRIMITIVES

        public static void DrawLinePlane(Vector3[] points, float height, bool loop = false)
        {
            if (points.Length < 1)
                return;

            Begin(DrawType.QUADS);

            Vector3 heightVector = new Vector3(0, height, 0);
            for (int i = 0; i < points.Length - 1; i++)
            {
                Vertex(points[i]);
                Vertex(points[i + 1]);
                Vertex(points[i + 1] + heightVector);
                Vertex(points[i]     + heightVector);
            }
            if (loop)
            {
                Vertex(points[points.Length - 1]);
                Vertex(points[0]);
                Vertex(points[0] + heightVector);
                Vertex(points[points.Length - 1] + heightVector);
            }

            for (int i = points.Length - 1; i > 0; i--)
            {
                Vertex(points[i]);
                Vertex(points[i - 1]);
                Vertex(points[i - 1] + heightVector);
                Vertex(points[i] + heightVector);
            }
            if (loop)
            {
                Vertex(points[0]);
                Vertex(points[points.Length - 1]);
                Vertex(points[points.Length - 1] + heightVector);
                Vertex(points[0] + heightVector);
            }

            End();
        }

        public static void DrawLine(float x1, float y1, float z1, float x2, float y2, float z2)
        {
            DrawLine(new Vector3(x1, y1, z1), new Vector3(x2, y2, z2));
        }

        public static void DrawLine(Vector3 start, Vector3 end)
        {
            Begin(DrawType.LINES);
            Vertex(start);
            TexCoord(Vector2.zero);
            Vertex(end);
            TexCoord(Vector2.zero);
            End();
        }

        public static void DrawLineStrip(Vector3[] points)
        {
            Begin(DrawType.LINE_STRIP);
            for (int i = 0; i < points.Length; i++)
                Vertex(points[i]);
            End();
        }

        public static void DrawLineStrip(List<Vector3> points)
        {
            Begin(DrawType.LINE_STRIP);
            for (int i = 0; i < points.Count; i++)
                Vertex(points[i]);
            End();
        }

        public static void DrawRayNormalized(Ray ray)
        {
            DrawRayNormalized(ray.origin, ray.direction);
        }

        public static void DrawRayNormalized(Vector3 origin, Vector3 direction)
        {
            DrawLine(origin, origin + direction.normalized);
        }

        public static void DrawRay(Ray ray)
        {
            DrawRay(ray.origin, ray.direction);
        }

        public static void DrawRay(Vector3 origin, Vector3 direction)
        {
            DrawLine(origin, origin + direction);
        }

        public static void DrawRayNormalized(Vector3 origin, Vector3 direction, float scale)
        {
            DrawLine(origin, origin + (direction.normalized * scale));
        }

        public static void DrawRay(Vector3 origin, Vector3 direction, float scale)
        {
            DrawLine(origin, origin + (direction * scale));
        }

        public static void DrawAxis(Transform transform)
        {
            DrawAxis(transform, Vector3.zero);
        }

        public static void DrawAxis(Transform transform, float scale)
        {
            DrawAxis(transform, Vector3.zero, scale);
        }

        public static void DrawAxis(Transform transform, Vector3 offset)
        {
            DrawAxis(transform, offset, 1f);
        }

        public static void DrawAxis(Transform transform, Vector3 offset, float scale)
        {
            var oldColor = _color;

            _color = Color.red;
            DrawRay(transform.position + offset, transform.right * scale);
            _color = Color.blue;
            DrawRay(transform.position + offset, transform.forward * scale);
            _color = Color.green;
            DrawRay(transform.position + offset, transform.up * scale);

            _color = oldColor;
        }

        public static void DrawFlatTorus(float x, float y, float z, float outerRadius, float innerRadius, int segments = 60, bool raycast = false, int layerMask = int.MaxValue)
        {
            DrawFlatTorus(new Vector3(x, y, z), outerRadius, innerRadius, segments, raycast, layerMask);
        }

        public static void DrawFlatTorus(Vector3 origin, float outerRadius, float innerRadius, int segments = 60, bool raycast = false, int layerMask = int.MaxValue)
        {
            Begin(DrawType.TRIANGLE_STRIP);
            float xi, yi, zi, xo, yo, zo, i;
            float inc = Mathf.PI / segments;
            for (i = 0; i < (2 * Mathf.PI) + inc; i += inc)
            {
                xi = innerRadius * Mathf.Cos(i);
                zi = innerRadius * Mathf.Sin(i);
                xo = outerRadius * Mathf.Cos(i);
                zo = outerRadius * Mathf.Sin(i);
                if (raycast)
                {
                    if (Physics.Raycast(new Ray(new Vector3(origin.x + xi, origin.y, origin.z + zi), Vector3.down), out var hitInfo, 9999f, layerMask))
                    {
                        yi = hitInfo.point.y + 0.001f;
                    }
                    else
                    {
                        yi = origin.y;
                    }
                    if (Physics.Raycast(new Ray(new Vector3(origin.x + xo, origin.y, origin.z + zo), Vector3.down), out hitInfo, 9999f, layerMask))
                    {
                        yo = hitInfo.point.y + 0.001f;
                    }
                    else
                    {
                        yo = origin.y;
                    }
                }
                else
                {
                    yi = origin.y;
                    yo = yi;
                }
                Vertex(new Vector3(origin.x + xi, yi, origin.z + zi));
                Vertex(new Vector3(origin.x + xo, yo, origin.z + zo));
            }
            End();
        }

        public static void DrawCircle(float x, float y, float z, float radius, bool raycast = false, int layerMask = int.MaxValue)
        {
            DrawCircle(new Vector3(x, y, z), radius, raycast, layerMask);
        }

        public static void DrawCircle(Vector3 origin, float radius, bool raycast = false, int layerMask = int.MaxValue)
        {
            int segmentCount = Mathf.RoundToInt((radius / 5f) * 60);
            DrawCircle(origin, radius, segmentCount, raycast, layerMask);
        }

        public static void DrawCircle(float x, float y, float z, float radius, int segments = 60, bool raycast = false, int layerMask = int.MaxValue)
        {
            DrawCircle(new Vector3(x, y, z), radius, segments, raycast, layerMask);
        }

        public static void DrawCircle(Vector3 origin, float radius, int segments = 60, bool raycast = false, int layerMask = int.MaxValue)
        {
            Begin(DrawType.LINE_STRIP);

            float x, y, z, i;
            float inc = Mathf.PI / segments;
            for (i = 0; i < (2 * Mathf.PI) + inc; i += inc)
            {
                x = radius * Mathf.Cos(i);
                z = radius * Mathf.Sin(i);
                if (raycast)
                {
                    if (Physics.Raycast(new Ray(new Vector3(origin.x + x, origin.y, origin.z + z), Vector3.down), out var hitInfo, 9999f, layerMask))
                    {
                        y = hitInfo.point.y + 0.001f;
                    }
                    else
                    {
                        y = origin.y;
                    }
                }
                else
                {
                    y = origin.y;
                }
                Vertex(new Vector3(origin.x + x, y, origin.z + z));
            }

            End();
        }

        public static void DrawSphere(float x, float y, float z, float radius, int nParal = 4, int nMerid = 4, float density = 8)
        {
            DrawSphere(new Vector3(x, y, z), radius, nParal, nMerid, density);
        }

        public static void DrawSphere(Vector3 origin, float radius, int nParal = 4, int nMerid = 4, float density = 8)
        {
            float x, y, z, i, j;
            float pi2 = (Mathf.PI * 2f) + 0.01f;

            for (j = 0; j < Mathf.PI; j += Mathf.PI / (nParal + 1))
            {
                Begin(DrawType.LINE_STRIP);
                y = (float)(radius * Mathf.Cos(j));
                for (i = 0; i <= pi2; i += (Mathf.PI / density))
                {
                    x = radius * Mathf.Cos(i) * Mathf.Sin(j);
                    z = radius * Mathf.Sin(i) * Mathf.Sin(j);
                    Vertex(origin.x + x, origin.y + y, origin.z + z);
                }
                End();
            }

            for (j = 0; j < Mathf.PI; j += Mathf.PI / nMerid)
            {
                Begin(DrawType.LINE_STRIP);
                for (i = 0; i <= pi2; i += (Mathf.PI / density))
                {
                    x = radius * Mathf.Sin(i) * Mathf.Cos(j);
                    y = radius * Mathf.Cos(i);
                    z = radius * Mathf.Sin(j) * Mathf.Sin(i);
                    Vertex(origin.x + x, origin.y + y, origin.z + z);
                }
                End();
            }
        }


        public static void DrawAlignedBox(Transform origin, Vector3 size)
        {
            PushMatrix();
            Matrix = origin.localToWorldMatrix;
            DrawBox(Vector3.zero, size);
            PopMatrix();
        }

        public static void DrawAlignedSolidBox(Transform origin, Vector3 size)
        {
            PushMatrix();
            Matrix = origin.localToWorldMatrix;
            DrawSolidBox(Vector3.zero, size);
            PopMatrix();
        }

        public static void DrawSolidBox(Vector3 origin, Vector3 size)
        {
            Vector3 halfSize = size / 2;
            Begin(DrawType.QUADS);

            // front
            Vertex(origin.x + halfSize.x, origin.y + halfSize.y, origin.z - halfSize.z);
            Vertex(origin.x + halfSize.x, origin.y + halfSize.y, origin.z + halfSize.z);
            Vertex(origin.x + halfSize.x, origin.y - halfSize.y, origin.z + halfSize.z);
            Vertex(origin.x + halfSize.x, origin.y - halfSize.y, origin.z - halfSize.z);

            // back
            Vertex(origin.x - halfSize.x, origin.y - halfSize.y, origin.z - halfSize.z);
            Vertex(origin.x - halfSize.x, origin.y - halfSize.y, origin.z + halfSize.z);
            Vertex(origin.x - halfSize.x, origin.y + halfSize.y, origin.z + halfSize.z);
            Vertex(origin.x - halfSize.x, origin.y + halfSize.y, origin.z - halfSize.z);

            // top
            Vertex(origin.x - halfSize.x, origin.y + halfSize.y, origin.z - halfSize.z);
            Vertex(origin.x - halfSize.x, origin.y + halfSize.y, origin.z + halfSize.z);
            Vertex(origin.x + halfSize.x, origin.y + halfSize.y, origin.z + halfSize.z);
            Vertex(origin.x + halfSize.x, origin.y + halfSize.y, origin.z - halfSize.z);

            // bottom
            Vertex(origin.x + halfSize.x, origin.y - halfSize.y, origin.z - halfSize.z);
            Vertex(origin.x + halfSize.x, origin.y - halfSize.y, origin.z + halfSize.z);
            Vertex(origin.x - halfSize.x, origin.y - halfSize.y, origin.z + halfSize.z);
            Vertex(origin.x - halfSize.x, origin.y - halfSize.y, origin.z - halfSize.z);

            // right
            Vertex(origin.x - halfSize.x, origin.y + halfSize.y, origin.z - halfSize.z);
            Vertex(origin.x + halfSize.x, origin.y + halfSize.y, origin.z - halfSize.z);
            Vertex(origin.x + halfSize.x, origin.y - halfSize.y, origin.z - halfSize.z);
            Vertex(origin.x - halfSize.x, origin.y - halfSize.y, origin.z - halfSize.z);

            // left
            Vertex(origin.x - halfSize.x, origin.y - halfSize.y, origin.z + halfSize.z);
            Vertex(origin.x + halfSize.x, origin.y - halfSize.y, origin.z + halfSize.z);
            Vertex(origin.x + halfSize.x, origin.y + halfSize.y, origin.z + halfSize.z);
            Vertex(origin.x - halfSize.x, origin.y + halfSize.y, origin.z + halfSize.z);

            End();
        }

        public static void DrawText(string text, Vector3 position, float meterSize = 1)
        {
            AtlasFont.SystemFont.DrawWorldspace(position, text, meterSize, Color.white);
        }

        public static void DrawTextWithBg(string text, Vector3 position, Color bgColor, float meterSize = 1)
        {
            AtlasFont.SystemFont.DrawWorldspace(position, text, meterSize, Color.white, bgColor);
        }


        public static void DrawTextWithBg(string text, Vector3 position, float meterSize = 1)
        {
            DrawTextWithBg(text, position, Color.black, meterSize);
        }

        public static void DrawBox()
        {
            DrawBox(Vector3.zero, Vector3.one);
        }

        public static void DrawBox(Vector3 origin, Vector3 size)
        {
            Vector3 halfSize = size / 2;
            Begin(DrawType.LINE_STRIP);

            // draw top, bottom, and one of the side lines all in one shot
            Vertex(origin.x + halfSize.x, origin.y + halfSize.y, origin.z + halfSize.z);
            Vertex(origin.x - halfSize.x, origin.y + halfSize.y, origin.z + halfSize.z);
            Vertex(origin.x - halfSize.x, origin.y + halfSize.y, origin.z - halfSize.z);
            Vertex(origin.x + halfSize.x, origin.y + halfSize.y, origin.z - halfSize.z);
            Vertex(origin.x + halfSize.x, origin.y + halfSize.y, origin.z + halfSize.z);
            Vertex(origin.x + halfSize.x, origin.y - halfSize.y, origin.z + halfSize.z);
            Vertex(origin.x + halfSize.x, origin.y - halfSize.y, origin.z - halfSize.z);
            Vertex(origin.x - halfSize.x, origin.y - halfSize.y, origin.z - halfSize.z);
            Vertex(origin.x - halfSize.x, origin.y - halfSize.y, origin.z + halfSize.z);
            Vertex(origin.x + halfSize.x, origin.y - halfSize.y, origin.z + halfSize.z);

            End();

            // draw the remaining 3 lines
            Begin(DrawType.LINES);

            Vertex(origin.x - halfSize.x, origin.y - halfSize.y, origin.z + halfSize.z);
            Vertex(origin.x - halfSize.x, origin.y + halfSize.y, origin.z + halfSize.z);

            Vertex(origin.x - halfSize.x, origin.y - halfSize.y, origin.z - halfSize.z);
            Vertex(origin.x - halfSize.x, origin.y + halfSize.y, origin.z - halfSize.z);

            Vertex(origin.x + halfSize.x, origin.y - halfSize.y, origin.z - halfSize.z);
            Vertex(origin.x + halfSize.x, origin.y + halfSize.y, origin.z - halfSize.z);

            End();
        }

        public static void DrawArrow(Vector3 start, Vector3 end, float arrowAngle = 20f, float arrowSize = 0.45f)
        {
            Vector3 direction = end - start;
            if (direction.magnitude <= Mathf.Epsilon)
                return;
            var tipPoint = start + direction;

            Begin(DrawType.LINES);

            Vertex(start);
            Vertex(tipPoint);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowAngle, 0) * new Vector3(0, 0, 1);

            Vertex(tipPoint);
            Vertex(tipPoint + (right * arrowSize));

            Vertex(tipPoint);
            Vertex(tipPoint + (left * arrowSize));

            End();
        }

        public static void DrawCrosshair(Vector3 position, float size = 1f)
        {
            Begin(DrawType.LINES);

            Vertex(position.x + size, position.y, position.z);
            Vertex(position.x - size, position.y, position.z);
            Vertex(position.x, position.y + size, position.z);
            Vertex(position.x, position.y - size, position.z);
            Vertex(position.x, position.y, position.z + size);
            Vertex(position.x, position.y, position.z - size);

            End();
        }

    }
}
