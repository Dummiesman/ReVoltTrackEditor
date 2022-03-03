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

using ReVolt.TrackUnit;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public partial class TrackExporter
{
    private ReVolt.Track.CollisionFile collision;
    private List<ReVolt.Track.CollisionPoly>[] cellCollisionBuckets;

    private void FixUpCollision(int x, int y)
    {
        var srcBucket = cellCollisionBuckets[(y * track.Width) + x];
        List<ReVolt.Track.CollisionPoly>[] adjacentBuckets = new List<ReVolt.Track.CollisionPoly>[4];

        if (x > 0)                 adjacentBuckets[0] = cellCollisionBuckets[(y * track.Width) + (x - 1)];
        if (x < track.Width - 1)   adjacentBuckets[1] = cellCollisionBuckets[(y * track.Width) + (x + 1)];
        if (y > 0)                 adjacentBuckets[2] = cellCollisionBuckets[((y - 1) * track.Width) + x];
        if (y < track.Height - 1)  adjacentBuckets[3] = cellCollisionBuckets[((y + 1) * track.Width) + x];

        for (int adjBucketIndex = 0; adjBucketIndex < 4; adjBucketIndex++)
        {
            if (adjacentBuckets[adjBucketIndex] == null)
                continue;
            
            var adjBucket = adjacentBuckets[adjBucketIndex];
            HashSet<int> removePolyIndicesAdj = new HashSet<int>();
            HashSet<int> removePolyIndices = new HashSet<int>();

            for (int i = 0; i < srcBucket.Count; i++)
            {
                for (int j = 0; j < adjBucket.Count; j++)
                {
                    var polyI = srcBucket[i];
                    var polyJ = adjBucket[j];

                    // only remove facing polys if they're the same vert count and facing each other
                    bool facing = (polyI.Type & ReVolt.Track.CollisionPolyFlags.Quad) == (polyJ.Type & ReVolt.Track.CollisionPolyFlags.Quad) &&
                                  polyI.Planes[0].ApproximatelyInvEquals(polyJ.Planes[0]);

                    if (facing)
                    {
                        bool matching = true;
                        int checkPlanes = ((polyI.Type & ReVolt.Track.CollisionPolyFlags.Quad) != 0) ? 4 : 3;

                        for (int k = 0; k < checkPlanes && matching; k++)
                        {
                            matching = polyJ.Contains(polyI.Planes[k + 1]);
                        }

                        if (matching)
                        {
                            removePolyIndices.Add(i);
                            removePolyIndicesAdj.Add(j);
                        }
                    }
                }
            }

            // remove them
            foreach (int polyIndex in removePolyIndicesAdj.OrderByDescending(x => x))
                adjBucket.RemoveAt(polyIndex);
            foreach (int polyIndex in removePolyIndices.OrderByDescending(x => x))
                srcBucket.RemoveAt(polyIndex);
        }
    }

    private void FixUpCollision()
    {
        // checkerboard search
        for(int y=0; y < track.Height; y++)
        {
            for(int x=y % 2; x < track.Width; x+= 2)
            {
                FixUpCollision(x, y);
            }
        }
    }

    private bool TryMergePolygons(ReVolt.Track.CollisionPoly poly1, ReVolt.Track.CollisionPoly poly2, out ReVolt.Track.CollisionPoly merged)
    {
        merged = default;

        if ((poly1.Type & ReVolt.Track.CollisionPolyFlags.Quad) == 0 || (poly2.Type & ReVolt.Track.CollisionPolyFlags.Quad) == 0)
            return false;
        if (poly1.Material != poly2.Material)
            return false;
        if (!poly1.Planes[0].ApproximatelyEquals(poly2.Planes[0]))
            return false;

        // check if one of the other polygons plane is approximately the inverse of one of our planes
        // if so, verify that both the adjacent planes match. matches if all conditions true.
        // copy the back planes position and normal to the other polygons back plane
        // then recalculate bounds

        bool match = false;
        for(int m1=0; m1 < 4 && !match; m1++)
        {
            for (int m2 = 0; m2 < 4 && !match; m2++)
            {
                // m1 is counting the plane index in poly1
                // m2 is counting the plane index in poly2
                if (poly1.Planes[m1+1].ApproximatelyInvEquals(poly2.Planes[m2+1])) //back-to-back
                {
                    if(poly1.Planes[((m1 + 1) % 4) + 1].ApproximatelyEquals(poly2.Planes[((m2 + 3) % 4) + 1])) //adjacent
                    {
                        if (poly1.Planes[((m1 + 3) % 4) + 1].ApproximatelyEquals(poly2.Planes[((m2 + 1) % 4) + 1])) //adjacent2
                        {
                            merged = new ReVolt.Track.CollisionPoly()
                            {
                                Material = poly1.Material,
                                Min = Vector3.Min(poly1.Min, poly2.Min),
                                Max = Vector3.Max(poly1.Max, poly2.Max),
                                Planes = new ReVolt.Track.CollisionPolyPlane[] { poly1.Planes[0], poly1.Planes[1], poly1.Planes[2], poly1.Planes[3], poly1.Planes[4] },
                                Type = poly1.Type
                            };

                            merged.Planes[m1 + 1].Normal = poly2.Planes[((m2 + 2) % 4) + 1].Normal;
                            merged.Planes[m1 + 1].Distance = poly2.Planes[((m2 + 2) % 4) + 1].Distance;
                            match = true;
                        }
                    }
                }
            }
        }
        return match;
    }

    private void FinalizeCollision()
    {
        // merge each bucket to itself first
        // saves a lot of time later
        bool anyMerged;

        for(int i=0; i < cellCollisionBuckets.Length; i++)
        {
            do
            {
                anyMerged = false;

                var bucket = cellCollisionBuckets[i];
                bool[] mergedArray = new bool[bucket.Count];

                for (int j = 0; j < bucket.Count; j++)
                {
                    if (mergedArray[j])
                        continue;
                    for (int k = j + 1; k < bucket.Count; k++)
                    {
                        if (mergedArray[k])
                            continue;

                        bool merged = TryMergePolygons(bucket[j], bucket[k], out var mergedSelfPoly);
                        if (merged)
                        {
                            bucket[j] = mergedSelfPoly;
                            mergedArray[k] = true;
                        }
                        anyMerged |= merged;
                    }
                }

                for (int j = bucket.Count - 1; j >= 0; j--)
                {
                    if (mergedArray[j])
                        bucket.RemoveAt(j);
                }
            } while (anyMerged);
        }

        // copy buckets into polys list
        foreach (var bucket in cellCollisionBuckets)
            collision.Polyhedrons.AddRange(bucket);

        // merge whole track
        do
        {
            anyMerged = false;

            var bucket = collision.Polyhedrons;
            bool[] mergedArray = new bool[bucket.Count];

            for (int j = 0; j < bucket.Count; j++)
            {
                if (mergedArray[j])
                    continue;
                for (int k = j+1; k < bucket.Count; k++)
                {
                    if (mergedArray[k])
                        continue;

                    bool merged = TryMergePolygons(bucket[j], bucket[k], out var mergedSelfPoly);
                    if (merged)
                    {
                        bucket[j] = mergedSelfPoly;
                        mergedArray[k] = true;
                    }
                    anyMerged |= merged;
                }
            }

            for (int j = bucket.Count - 1; j >= 0; j--)
            {
                if (mergedArray[j])
                    bucket.RemoveAt(j);
            }
        } while (anyMerged);
    }

    private List<ReVolt.Track.CollisionPoly> UnitToCollisionBucket(Unit unit, Matrix4x4 matrix, bool[] wallData = null)
    {
        var bucket = new List<ReVolt.Track.CollisionPoly>(512);

        // export unit
        var mesh = unitFile.Meshes[unit.MeshID];
        Vector3 origin = matrix.GetColumn(3);

        // check if mesh contains hull
        if (RVConstants.HULL_INDEX >= mesh.PolySets.Count)
            return null;

        var polySet = unitFile.PolySets[mesh.PolySets[RVConstants.HULL_INDEX]];
        for (int i = 0; i < polySet.PolygonIndices.Count; i++)
        {
            var poly = unitFile.Polygons[polySet.PolygonIndices[i] & ~RVConstants.GOURAUD_SHIFTED];
            int polyVertCount = (poly.Indices.Length == 3) ? 3 : 4;

            Vector3[] transformedVerts = new Vector3[polyVertCount];
            for (int j = 0; j < polyVertCount; j++)
            {
                // ugly hack to make the hull taller, so when placed near height limit
                // there's no collision holes
                Vector3 vertex = unitFile.Vertices[poly.Indices[j]];
                if (vertex.y > RVConstants.SMALL_CUBE_SIZE)
                    vertex.y += RVConstants.SMALL_CUBE_SIZE;

                transformedVerts[j] = matrix.MultiplyPoint3x4(vertex);
            }

            // if poly is entirely underground, skip it
            // don't check with height zero because modules like dips and pipes go underground
            if (transformedVerts.Count(x => x.y > RVConstants.SMALL_CUBE_SIZE) == polyVertCount)
                continue;

            // get poly normal
            var mainNormal = CalculateNormal(transformedVerts);
            int possibleWallIndex = -1;

            for(int j=0; j < 4 && possibleWallIndex < 0; j++)
            {
                if (mainNormal == rootNormalsArray[j])
                    possibleWallIndex = j;
            }

            // if poly is entirely under the cell, only export it if there's supposed to be a wall there
            if(wallData != null && possibleWallIndex >= 0)
            {
                bool entirePolyUnderCell = transformedVerts.Count(x => x.y >= (origin.y + RVConstants.SMALL_CUBE_SIZE)) == polyVertCount;
                if (entirePolyUnderCell && !wallData[possibleWallIndex])
                    goto NEXT_POLY;
            }

            // clamp wall vertices to the bottom of the world
            // this assists with collision merging and is REQUIRED for collision merging of the Dreamcast trackunit
            if (possibleWallIndex >= 0)
            { 
                for (int j = 0; j < polyVertCount; j++)
                {
                    transformedVerts[j].y = Mathf.Min(transformedVerts[j].y, RVConstants.SMALL_CUBE_SIZE);
                }
            }

            // turn this poly into a collision poly
            var collPoly = new ReVolt.Track.CollisionPoly()
            {
                Type = (polyVertCount == 3) ? ReVolt.Track.CollisionPolyFlags.None
                                            : ReVolt.Track.CollisionPolyFlags.Quad,
                Planes = new ReVolt.Track.CollisionPolyPlane[5],
                Material = (ReVolt.Track.PhysicsMaterial)unit.Surfaces[i]
            };

            //create planes
            var mainDistance = -Vector3.Dot(mainNormal, transformedVerts[0]);

            collPoly.Planes[0] = new ReVolt.Track.CollisionPolyPlane()
            {
                Normal = mainNormal,
                Distance = mainDistance
            };

            for (int j = polyVertCount - 1; j >= 0; j--)
            {
                var vertA = transformedVerts[j];
                var vertB = transformedVerts[(j + 1) % polyVertCount];

                Vector3 planeNormal = Vector3.Cross(mainNormal, (vertA - vertB)).normalized;
                float planeDistance = -Vector3.Dot(planeNormal, vertA);

                collPoly.Planes[polyVertCount - j] = new ReVolt.Track.CollisionPolyPlane()
                {
                    Distance = planeDistance,
                    Normal = planeNormal
                };
            }

            // calculate bounds
            Bounds polyBounds = new Bounds(transformedVerts[0], Vector3.zero);
            for (int j = 1; j < polyVertCount; j++)
                polyBounds.Encapsulate(transformedVerts[j]);
            collPoly.Min = polyBounds.min;
            collPoly.Max = polyBounds.max;

            // finally, add to the bucket
            bucket.Add(collPoly);

            NEXT_POLY:;
        }

        return bucket;
    }

    private List<ReVolt.Track.CollisionPoly> CellToCollisionBucket(EditorTrackCell cell)
    {
        var unit = unitFile.Units[GetUnitInCell(cell)];
        var cellMatrix = MakeCellMatrix(cell);
        bool[] wallData = new bool[] { unit.CheckRootEdge(mod(0 - cell.Module.Rotation, 4)) && cell.CheckWall(track, Modules.Direction.North),
                                       unit.CheckRootEdge(mod(1 - cell.Module.Rotation, 4)) && cell.CheckWall(track, Modules.Direction.East),
                                       unit.CheckRootEdge(mod(2 - cell.Module.Rotation, 4)) && cell.CheckWall(track, Modules.Direction.South),
                                       unit.CheckRootEdge(mod(3 - cell.Module.Rotation, 4)) && cell.CheckWall(track, Modules.Direction.West) };
        
        return UnitToCollisionBucket(unit, cellMatrix, wallData);
    }

    public void CreateCollision()
    {
        collision = new ReVolt.Track.CollisionFile();
        var perfLogger = new PerfTimeLogger("Export:Collision");

        // create user track collision
        cellCollisionBuckets = new List<ReVolt.Track.CollisionPoly>[track.Width * track.Height];
        for(int i=0; i < cellCollisionBuckets.Length; i++)
        {
            int x = i % track.Width;
            int y = i / track.Width;
            var cell = track.GetCell(x, y);
            cellCollisionBuckets[i] = CellToCollisionBucket(cell);
        }
        perfLogger.Log("Create collision buckets");

        // remove intersecting collision
        FixUpCollision();
        perfLogger.Log("Remove extra polygons");

        // add external walls
        for (int i = 0; i < 6; i++)
        {
            var wallUnit = unitFile.Units[i + unitFile.WallIndex];
            collision.Polyhedrons.AddRange(UnitToCollisionBucket(wallUnit, wallMatrices[i]));
        }
        perfLogger.Log("Add walls");

        // finalize collision (merges polygons and adds the final results to the collision file)
        FinalizeCollision();
        perfLogger.Log("Merge");

        collision.CreateGrid(RVConstants.SMALL_CUBE_SIZE);

        // scale
        if (exportScale != 1f)
        {
            collision.Scale(exportScale);
            perfLogger.Log("Scale");
        }
    }

    public void WriteCollision()
    {
        string collisionFilePath = Path.Combine(exportPath, $"{trackFolderName}.ncp");
        collision.Save(collisionFilePath);
    }
}
