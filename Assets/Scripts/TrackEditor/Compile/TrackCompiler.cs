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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;
using ReVolt.TrackUnit;

public struct ZoneSequenceEntry
{
    public int ZoneID;
    public bool Forwards;
}

public struct TrackZone
{
    public Vector3 Center;
    public Vector3 Size;
    public Vector3[] Links;
    public bool IsPipe;

    public Vector2Int CellCoordinate =>  new Vector2Int(Mathf.RoundToInt(Center.x / RVConstants.SMALL_CUBE_SIZE), 
                                                        Mathf.RoundToInt(Center.z / RVConstants.SMALL_CUBE_SIZE));
}

struct PipeWeld
{
    public int NoSides;
    public int NoExit;
    public int NoEntry;

    public PipeWeld(int noSides, int noExit, int noEntry)
    {
        this.NoSides = noSides;
        this.NoExit = noExit;
        this.NoEntry = noEntry;
    }
};

[System.Flags]
enum BridgeFlags
{
    None,
    LowerDeckReversed,
    UpperDeckReversed
}

public partial class TrackCompiler
{
    public bool LastZoneIsPipe => zoneSequence.Count != 0 && zones[zoneSequence.Last().ZoneID].IsPipe;
    public bool TrackFormsLoop { get; private set; }
    public float TrackLength => lapDistance;
    public IReadOnlyList<ZoneSequenceEntry> ZoneSequence => zoneSequence;
    public IReadOnlyList<TrackZone> Zones => zones;

    private readonly TrackUnitFile unitFile;
    private EditorTrack originalTrack; // the reference to the original track, do not modify!
    public EditorTrack track; // our processed track with non user-accessible modules

    private float exportScale = 1f;
    private bool reversed = false;

    private const float maxElevation = EditorConstants.MaxElevationSteps * -RVConstants.ElevationStep;

    private List<TrackZone> zones = new List<TrackZone>();
    private List<ZoneSequenceEntry> zoneSequence = new List<ZoneSequenceEntry>();

    private List<Vector3> posNodes = new List<Vector3>();
    private List<float> posNodeDistances = new List<float>();
    private float lapDistance;

    private ModulePlacement startModule;

    /*
        Compiled Data
    */
    public ReVolt.Track.AINodesFile CompiledAINodes { get; private set; } = null;
    public ReVolt.Track.WorldFile CompiledWorld { get; private set; } = null;
    public ReVolt.Track.CollisionFile CompiledCollision { get; private set; } = null;
    public ReVolt.Track.ObjectsFile CompiledObjects { get; private set; } = null;
    public ReVolt.Track.LightsFile CompiledLights { get; private set; } = null;
    public ReVolt.Track.ZonesFile CompiledZones { get; private set; } = null;
    public ReVolt.Track.POSNodesFile CompiledPOSNodes { get; private set; } = null;

    /* 
        Common Data 
    */

    // tolerance used for fix functions (ex. removing excess colliision and world polys)
    private const float FIX_TOLERANCE = 0.01f; 

    private readonly Dictionary<int, PipeWeld> pipeWelds = new Dictionary<int, PipeWeld>()
    {
        {(int)Modules.ID.TWM_PIPE_2, new PipeWeld((int)Modules.ID.TWM_PIPE_0, (int)Modules.ID.TWM_PIPE_1, (int)Modules.ID.TWM_PIPE_1A)},
        {(int)Modules.ID.TWM_PIPEC_2, new PipeWeld((int)Modules.ID.TWM_PIPEC_0, (int)Modules.ID.TWM_PIPEC_1, (int)Modules.ID.TWM_PIPEC_1A)},
        {(int)Modules.ID.TWM_PIPE_20_2, new PipeWeld((int)Modules.ID.TWM_PIPE_20_0, (int)Modules.ID.TWM_PIPE_20_1_BOT, (int)Modules.ID.TWM_PIPE_20_1_TOP)},
    };

    private readonly Vector3[][] rootVertsArray = new Vector3[][]
    {
        new []{ new Vector3(-RVConstants.SMALL_CUBE_HALF, 0f, RVConstants.SMALL_CUBE_HALF), new Vector3(RVConstants.SMALL_CUBE_HALF, 0f, RVConstants.SMALL_CUBE_HALF)},
        new []{ new Vector3( RVConstants.SMALL_CUBE_HALF, 0f,  RVConstants.SMALL_CUBE_HALF), new Vector3(RVConstants.SMALL_CUBE_HALF, 0f, -RVConstants.SMALL_CUBE_HALF) },
        new []{ new Vector3( RVConstants.SMALL_CUBE_HALF, 0f, -RVConstants.SMALL_CUBE_HALF), new Vector3(-RVConstants.SMALL_CUBE_HALF, 0f, -RVConstants.SMALL_CUBE_HALF) },
        new []{ new Vector3(-RVConstants.SMALL_CUBE_HALF, 0f, -RVConstants.SMALL_CUBE_HALF), new Vector3(-RVConstants.SMALL_CUBE_HALF, 0f, RVConstants.SMALL_CUBE_HALF) }
    };

    private readonly Vector3[] rootNormalsArray = new Vector3[]
    {
        Vector3.forward,
        Vector3.right,
        Vector3.back,
        Vector3.left
    };

    private Matrix4x4[] wallMatrices;

    /*
        Utility Functions 
    */
    int mod(int x, int m)
    {
        //modulo supporting negative
        return (x % m + m) % m;
    }

    private static Vector3 CalculateNormal(Vector3 a, Vector3 b, Vector3 c)
    {
        return Vector3.Cross(c - b, a - b).normalized;
    }

    private static Vector3 CalculateNormal(Vector3[] verts)
    {
        return CalculateNormal(verts[0], verts[1], verts[2]);
    }

    private static int ReverseDirection(int direction)
    {
        return (direction + 2) % 4;
    }

    private Matrix4x4 MakeCellMatrix(int xPos, int elevation, int yPos, int rotation)
    {
        var rotMtx = Matrix4x4.Rotate(Quaternion.Euler(0f, rotation * 90f, 0f));
        var translateMtx = Matrix4x4.Translate(new Vector3(xPos * RVConstants.SMALL_CUBE_SIZE,
                                                           elevation * -RVConstants.ElevationStep,
                                                           yPos * RVConstants.SMALL_CUBE_SIZE));
        return translateMtx * rotMtx;
    }

    private Matrix4x4 MakeCellMatrix(EditorTrackCell cell)
    {
        float elevation = (cell.Module != null) ? cell.Module.Elevation : 0f;
        int rotation = (cell.Module != null) ? cell.Module.Rotation : 0;

        return MakeCellMatrix(cell.Position.x, Mathf.RoundToInt(elevation / EditorConstants.ElevationStep),
                              cell.Position.y, rotation);
    }

    private Matrix4x4 MakeModuleMatrix(ModulePlacement placement)
    {
        return MakeCellMatrix(placement.Position.x, Mathf.RoundToInt(placement.Elevation / EditorConstants.ElevationStep),
                              placement.Position.y, placement.Rotation);
    }

    private static Quaternion MakeModuleQuat(ModulePlacement placement)
    {
        return Quaternion.Euler(0f, 90f * placement.Rotation, 0f);
    }

    private static bool ModuleIsPipeBase(int modId)
    {
        return (modId == (int)Modules.ID.TWM_PIPE_2 || modId == (int)Modules.ID.TWM_PIPEC_2 || modId == (int)Modules.ID.TWM_PIPE_20_2);
    }

    private int GetUnitInCell(EditorTrackCell cell)
    {
        if (cell.Module == null)
            return -1;

        // not the nicest way to handle this
        var root = cell.Module.RootCell;
        var module = unitFile.Modules[cell.Module.ModuleIndex];

        foreach(var instance in module.Instances)
        {
            var instanceCellPos = root.Position + instance.Position.Rotate(cell.Module.Rotation);
            if(instanceCellPos == cell.Position)
                return instance.UnitID;
        }
        return -1;
    }

    /// <summary>
    /// Enumerate zone sequences
    /// Returns tuples of Prev, Current, Next
    /// </summary>
    private IEnumerable<Tuple<ZoneSequenceEntry?, ZoneSequenceEntry, ZoneSequenceEntry?>> EnumZoneSequence()
    {
        for (int i = 0; i < zoneSequence.Count; i++)
        {
            ZoneSequenceEntry seqEntry = zoneSequence[i];
            ZoneSequenceEntry? prevSeqEntry = null;
            ZoneSequenceEntry? nextSeqEntry = null;

            if (i > 0)
                prevSeqEntry = zoneSequence[i - 1];
            if (i < zoneSequence.Count - 1)
                nextSeqEntry = zoneSequence[i + 1];

            yield return new Tuple<ZoneSequenceEntry?, ZoneSequenceEntry, ZoneSequenceEntry?>(prevSeqEntry, seqEntry, nextSeqEntry);
        }
    }

    /// <summary>
    /// Enumerate zones in order
    /// Returns tuples of Prev, Current, Next
    /// </summary>
    private IEnumerable<Tuple<TrackZone?, TrackZone, TrackZone?>> EnumZones()
    {
        foreach((ZoneSequenceEntry? prevSeqEntry, ZoneSequenceEntry seqEntry, ZoneSequenceEntry? nextSeqEntry) in EnumZoneSequence())
        {
            TrackZone zone = zones[seqEntry.ZoneID];
            TrackZone? prevZone = null;
            TrackZone? nextZone = null;

            if(prevSeqEntry.HasValue)
                prevZone = zones[prevSeqEntry.Value.ZoneID];
            if (nextSeqEntry.HasValue)
                nextZone = zones[nextSeqEntry.Value.ZoneID];

            yield return new Tuple<TrackZone?, TrackZone, TrackZone?>(prevZone, zone, nextZone);
        }
    }

    /// <summary>
    /// Enumerate modules in order
    /// </summary>
    private IEnumerable<ModulePlacement> EnumModules()
    { 
        Vector2Int lastCheckedModulePos = new Vector2Int(-1, -1); //start with an invalid zone

        for (int i = 1; i < zoneSequence.Count; i++)
        {
            var zone = zones[zoneSequence[i].ZoneID];
            var cell = track.GetCell(zone.CellCoordinate);

            if (cell.Module == null || cell.Module.RootCell.Position == lastCheckedModulePos)
                continue;

            lastCheckedModulePos = cell.Module.RootCell.Position;
            yield return cell.Module;
        }
    }

    /*
        Export Functions 
    */
    public string CreateInfoFile()
    {
        string formatFloat(float f) => f.ToString("F6", CultureInfo.InvariantCulture);

        var startModulePos = MakeModuleMatrix(startModule).GetColumn(3) * exportScale;

        string infoTemplate = Resources.Load<TextAsset>("InfoTemplate").text;
        infoTemplate = infoTemplate.Replace("%TRACKNAME%", track.Name);
        infoTemplate = infoTemplate.Replace("%STARTROT%", formatFloat(startModule.Rotation * 0.25f));
        infoTemplate = infoTemplate.Replace("%INVSTARTROT%", formatFloat(ReverseDirection(startModule.Rotation) * 0.25f));
        infoTemplate = infoTemplate.Replace("%STARTPOS%", $"{formatFloat(startModulePos.x)} {formatFloat(startModulePos.y)} {formatFloat(startModulePos.z)}");

        return infoTemplate;
    }

    private void CompileZones()
    {
        CompiledZones = new ReVolt.Track.ZonesFile();
        
        for(int i=0; i < zoneSequence.Count; i++)
        {
            var zone = zones[zoneSequence[(i + 1) % zoneSequence.Count].ZoneID];
            CompiledZones.Zones.Add(new ReVolt.Track.Zone()
            {
                ID = i,
                Matrix = Matrix4x4.identity,
                Position = zone.Center,
                Size = zone.Size 
            });
        }

        if(exportScale != 1f)
            CompiledZones.Scale(exportScale);
    }

    private void CompilePosNodes()
    {
        CompiledPOSNodes = new ReVolt.Track.POSNodesFile()
        {
            StartNodeIndex = 0,
            TotalLength = lapDistance
        };

        for(int i=0; i < posNodes.Count; i++)
        {
            var node = new ReVolt.Track.POSNode()
            {
                Position = posNodes[i],
                Distance = posNodeDistances[i],
                PreviousNodeIDs = new int[] { (i + (posNodes.Count - 1)) % posNodes.Count, -1, -1, -1 },
                NextNodeIDs = new int[] { (i + 1) % posNodes.Count, -1, -1, -1 },
            };
            CompiledPOSNodes.Nodes.Add(node);
        }

        if(exportScale != 1f)
            CompiledPOSNodes.Scale(exportScale);
    }

    public void CompileObjects()
    {
        CompiledObjects = new ReVolt.Track.ObjectsFile();

        foreach (var cell in track.Cells.Where(x => x.HasPickup))
        {
            Vector3 unitOffset = new Vector3((cell.Position.x - cell.Module.RootCell.Position.x) * RVConstants.SMALL_CUBE_SIZE,
                                 0f,
                                 (cell.Position.y - cell.Module.RootCell.Position.y) * RVConstants.SMALL_CUBE_SIZE);

            var unit = unitFile.Units[GetUnitInCell(cell)];
            var modMatrix = MakeModuleMatrix(cell.Module);

            Vector3 transformedPickupPosition = modMatrix.MultiplyPoint3x4(unit.PickupPosition) + unitOffset;

            CompiledObjects.Objects.Add(new ReVolt.Track.Object()
            {
                DirectionMatrix = Matrix4x4.identity,
                Position = transformedPickupPosition,
                Type = ReVolt.Track.ObjectType.Pickup
            });
        }

        if (exportScale != 1f)
            CompiledObjects.Scale(exportScale);
    }

    public void CompileLights()
    {
        var lightsFile = new ReVolt.Track.LightsFile();

        foreach (var moduleRootPlacement in track.GetAllModuleRootPlacements())
        {
            var module = unitFile.Modules[moduleRootPlacement.ModuleIndex];
            var cell = moduleRootPlacement.RootCell;

            foreach (var modLight in module.Lights)
            {
                var light = new ReVolt.Track.Light
                {
                    Color = modLight.Color,
                    Cone = modLight.Cone,
                    Reach = modLight.Reach,
                    Type = (byte)modLight.Type,
                    Flags = ReVolt.Track.LightFlags.File | ReVolt.Track.LightFlags.Fixed | ReVolt.Track.LightFlags.Moving
                };

                Matrix4x4 directionMatrix = Matrix4x4.LookAt(Vector3.down, Vector3.up, Vector3.forward);
                if (modLight.UpDirection.sqrMagnitude > 0f && modLight.ForwardDirection.sqrMagnitude > 0f)
                {
                    // untested
                    var moduleQuat = MakeModuleQuat(cell.Module);
                    Vector3 sideDirection = Vector3.Cross(modLight.ForwardDirection, modLight.UpDirection);
                    directionMatrix.SetColumn(0, moduleQuat * sideDirection);
                    directionMatrix.SetColumn(1, moduleQuat * modLight.UpDirection);
                    directionMatrix.SetColumn(2, moduleQuat * modLight.ForwardDirection);
                }

                light.Position = MakeModuleMatrix(moduleRootPlacement).MultiplyPoint3x4(modLight.Position);
                light.DirectionMatrix = directionMatrix;

                lightsFile.Lights.Add(light);
            }
        }

        if (exportScale != 1f)
            lightsFile.Scale(exportScale);
    }

    BridgeFlags GetBridgeFlags(ModulePlacement placement)
    {
        int index = 0;
        bool[] reversed = new bool[] { false, false };
        float[] elevation = new float[] { 0f, 0f };

        for (int i = 0; i < zoneSequence.Count; i++) 
        {
            var zone = zones[zoneSequence[i].ZoneID];
            if (zone.CellCoordinate == placement.Position)
            {
                elevation[index] = zone.Center.y;
                reversed[index] = !zoneSequence[i].Forwards;
                index = Mathf.Min(index + 1, 1);
            }
        }

        BridgeFlags flags = BridgeFlags.None;
        int lowerDeckIndex = (elevation[0] < elevation[1]) ? 0 : 1;
        int upperDeckIndex = 1 - lowerDeckIndex;

        if(reversed[lowerDeckIndex])
            flags |= BridgeFlags.LowerDeckReversed;
        if (reversed[upperDeckIndex])
            flags |= BridgeFlags.UpperDeckReversed;

        return flags;
    }

    void CorrectBridgeDirections()
    {
       foreach(var placement in track.GetAllModuleRootPlacements())
        {
            if(placement.ModuleIndex >= (int)Modules.ID.TWM_BRIDGE_10_2_N && placement.ModuleIndex <= (int)Modules.ID.TWM_BRIDGE_80_2_N)
            {
                int moduleid = placement.ModuleIndex - ((int)Modules.ID.TWM_BRIDGE_10_2_N - (int)Modules.ID.TWM_BRIDGE_10_2);
                var flags = GetBridgeFlags(placement);

                if((flags & BridgeFlags.LowerDeckReversed) != 0)
                {
                    placement.Rotation = ReverseDirection(placement.Rotation);
                }
                if(flags == BridgeFlags.LowerDeckReversed || flags == BridgeFlags.UpperDeckReversed)
                {
                    moduleid += ((int)Modules.ID.TWM_BRIDGE_10_2_LH - (int)Modules.ID.TWM_BRIDGE_10_2);
                }
                placement.ModuleIndex = moduleid;
            }
        }
    }

    void FillFloorWithSpacers()
    {
        foreach (var cell in track.Cells.Where(x => x.Module == null))
        {
            cell.Module = new ModulePlacement()
            {
                Elevation = 0f,
                ModuleIndex = (int)Modules.ID.TWM_SPACE_B,
                Position = cell.Position,
                RootCell = cell,
            };
            cell.Module.TouchedCells.Add(cell);
        }
    }

    private void WeldPipes()
    {
        foreach((ZoneSequenceEntry? prevSeqEntry, ZoneSequenceEntry seqEntry, ZoneSequenceEntry? nextSeqEntry) in EnumZoneSequence())
        {
            TrackZone zone = zones[seqEntry.ZoneID];
            ModulePlacement originalPlacement = originalTrack.GetCell(zone.CellCoordinate).Module;
            bool currentIsPipe = ModuleIsPipeBase(originalPlacement.ModuleIndex);

            if (!currentIsPipe)
                continue;

            TrackZone prevZone, nextZone;
            ModulePlacement prevPlacement = null, nextPlacement = null;

            if(prevSeqEntry.HasValue)
            {
                prevZone = zones[prevSeqEntry.Value.ZoneID];
                prevPlacement = originalTrack.GetCell(prevZone.CellCoordinate).Module;
            }
            if (nextSeqEntry.HasValue)
            {
                nextZone = zones[nextSeqEntry.Value.ZoneID];
                nextPlacement = originalTrack.GetCell(nextZone.CellCoordinate).Module;
            }

            // check if the previous/next modules are pipes, and not the same pipe
            // custom trackunits may have >1 unit pipe modules
            bool previousIsPipe = prevPlacement != null && prevPlacement.Position != originalPlacement.Position && ModuleIsPipeBase(prevPlacement.ModuleIndex);
            bool nextIsPipe = nextPlacement != null && nextPlacement.Position != originalPlacement.Position && ModuleIsPipeBase(nextPlacement.ModuleIndex);

            if (previousIsPipe || nextIsPipe)
            {
                // perfect, now we need to see how to weld things
                var weldData = pipeWelds[originalPlacement.ModuleIndex];
                ModulePlacement placement = track.GetCell(originalPlacement.Position).Module;
                
                if (previousIsPipe && nextIsPipe)
                    placement.ModuleIndex = weldData.NoSides; //remove both walls
                else if (previousIsPipe)
                    placement.ModuleIndex = (seqEntry.Forwards) ? weldData.NoEntry : weldData.NoExit; //remove front wall
                else if (nextIsPipe)
                    placement.ModuleIndex = (seqEntry.Forwards) ? weldData.NoExit : weldData.NoEntry; //remove back wall
            }
        }
    }

    private void MakeModulesDirectional()
    {
        foreach ((_, ZoneSequenceEntry seqEntry, _) in EnumZoneSequence())
        {
            var zone = zones[seqEntry.ZoneID];
            var placement = track.GetCell(zone.CellCoordinate).Module;

            if (seqEntry.Forwards)
            {
                placement.ModuleIndex = Modules.Lookup.Changes[placement.ModuleIndex].Forward;
            }
            else
            {
                placement.ModuleIndex = Modules.Lookup.Changes[placement.ModuleIndex].Reverse;

                if ((placement.ModuleIndex >= (int)Modules.ID.TWM_DIP_R && placement.ModuleIndex <= (int)Modules.ID.TWM_HUMP_XT_2)
                    || placement.ModuleIndex == (int)Modules.ID.TWM_RAMP_00_2)
                {
                    placement.Rotation = ReverseDirection(placement.Rotation);
                }

            }
        }
    }

    private void CreateZoneList()
    {
        foreach (var moduleRootPlacement in track.GetAllModuleRootPlacements())
        {
            var module = unitFile.Modules[moduleRootPlacement.ModuleIndex];
            var modMatrix = MakeModuleMatrix(moduleRootPlacement);

            foreach (var modZone in module.Zones)
            {
                var zone = new TrackZone
                {
                    Center = modMatrix.MultiplyPoint3x4(modZone.Center),
                    Size = modZone.Size,
                    Links = new Vector3[]
                    {
                        modMatrix.MultiplyPoint3x4(modZone.LinkPositions[0]),
                        modMatrix.MultiplyPoint3x4(modZone.LinkPositions[1])
                    }
                };

                zone.IsPipe = ModuleIsPipeBase(moduleRootPlacement.ModuleIndex);

                if (moduleRootPlacement.Rotation == (int)Modules.Direction.East
                    || moduleRootPlacement.Rotation == (int)Modules.Direction.West)
                {
                    zone.Size = new Vector3(zone.Size.z, zone.Size.y, zone.Size.x);
                }

                zones.Add(zone);
            }
        }
    }

    bool FindNextZone(int currentZoneIndex, int currentLink, out int nextZoneIndex, out int nextLinkIndex)
    {
        bool found = false;
        nextZoneIndex = -1;
        nextLinkIndex = -1;

        //
        var currentZone = zones[currentZoneIndex];
        var currentLinkPos = currentZone.Links[currentLink];

        float distanceThreshold = 0.25f;

        for (int i = 0; i < zones.Count; i++)
        {
            if (i == currentZoneIndex)
                continue;

            var otherZone = zones[i];
            for (int link = 0; link < 2; link++)
            {
                var linkPos = otherZone.Links[link];
                if (Mathf.Abs(linkPos.x - currentLinkPos.x) < distanceThreshold && Mathf.Abs(linkPos.z - currentLinkPos.z) < distanceThreshold)
                {
                    // must match Y coordinate if in a pipe
                    if (currentZone.IsPipe && otherZone.IsPipe)
                    {
                        if (Mathf.Abs(linkPos.y - currentLinkPos.y) < distanceThreshold)
                        {
                            nextZoneIndex = i;
                            nextLinkIndex = link;
                            found = true;
                        }
                    }
                    else
                    {
                        if (currentLinkPos.y <= linkPos.y)
                        {
                            nextZoneIndex = i;
                            nextLinkIndex = link;
                            found = true;
                        }
                    }
                }
            }
        }

        return found;
    }

    private void DetermineZoneSequence()
    {
        //find the start zone
        int startZoneIndex = -1;
        if(startModule != null)
        {
            var startModulePos = MakeModuleMatrix(startModule).GetColumn(3);
            for(int i=0; i < zones.Count; i++)
            {
                var zone = zones[i];
                float xD = Mathf.Abs(startModulePos.x - zone.Center.x);
                float zD = Mathf.Abs(startModulePos.z - zone.Center.z);
                if(xD < zone.Size.x && zD < zone.Size.z)
                {
                    startZoneIndex = i;
                    break;
                }
            }
        }

        //cannot create zone information, no start
        if (startZoneIndex < 0)
            return;

        var startZone = zones[startZoneIndex];

        var firstPosNode = startZone.Links[1] + ((startZone.Links[0] - startZone.Links[1]) / 100f);
        posNodes.Add(firstPosNode);

        //try and create a loop!
        int currentZoneIndex = startZoneIndex;
        int currentLink = 1;

        int nextZoneIndex = -1;
        int nextLink = 0;

        bool jumped = false;

        for(int i=0; i < zones.Count; i++)
        {
            var seqEntry = new ZoneSequenceEntry
            {
                ZoneID = currentZoneIndex,
                Forwards = (currentLink == 1)
            };
            zoneSequence.Add(seqEntry);

            var currentZone = zones[currentZoneIndex];
            if (jumped)
            {
                posNodes[posNodes.Count - 1] = currentZone.Links[1 - currentLink];
            }

            float slope = currentZone.Links[1-currentLink].y - currentZone.Links[currentLink].y;
            jumped = (slope > (10f * RVConstants.GameScale));
            posNodes.Add(currentZone.Links[currentLink]);

            if(FindNextZone(currentZoneIndex, currentLink, out nextZoneIndex, out nextLink))
            {
                currentZoneIndex = nextZoneIndex;
                currentLink = (1 - nextLink);
            }
            else
            {
                // track doesn't form a loop
                break;
            }

            // check if we've looped around
            if (zoneSequence.Count > 0 && startZoneIndex == nextZoneIndex)
                break;
        }

        if (jumped)
        {
            posNodes[posNodes.Count - 1] = startZone.Links[1 - currentLink];
        }

        TrackFormsLoop = (nextZoneIndex == startZoneIndex);
    }

    public int GetStartGridCount()
    {
        return track.GetAllModuleRootPlacements().Count(x => x.ModuleIndex == (int)Modules.ID.TWM_START);
    }

    /// <summary>
    /// Step one, before Validate()
    /// </summary>
    public virtual void Initialize()
    {
        var perfLogger = new PerfTimeLogger("Export");

        // locate the start module
        startModule = track.GetAllModuleRootPlacements().FirstOrDefault(x => x.ModuleIndex == (int)Modules.ID.TWM_START);
        if (startModule != null && reversed)
        {
            startModule.Rotation = ReverseDirection(startModule.Rotation);
        }

        // create wall matrices which will be shared between mesh/collision exporting
        Vector2Int[] wallCells = new Vector2Int[6]
        {
            Vector2Int.zero,                                        //0 is roof, leave it where it is                                
            new Vector2Int(0, -20 + track.Height),                  //1 is top left, move it by (-20 + track height) units on Y axis          
            Vector2Int.zero,                                        //2 is bottom left, leave it where it is
            new Vector2Int(-20 + track.Width, -20 + track.Height),  //3 is top right, move it by (-20 + track height) units on Y and (-20 + track width) units X axis   
            new Vector2Int(-20 + track.Width, -20 + track.Height),  //4 is top right (boxes), move it just like 3     
            new Vector2Int(-20 + track.Width, 0)                    //5 is bottom right, move it by (-20 + track width) units X axis          
        };

        wallMatrices = new Matrix4x4[6];
        for(int i=0; i < 6; i++)
        {
            wallMatrices[i] = MakeCellMatrix(wallCells[i].x, 0, wallCells[i].y, 0);
        }

        // init zoning stuff
        CreateZoneList();
        DetermineZoneSequence();

        // calculate lap length and pos node distances
        lapDistance = 0f;
        for (int i = 0; i < posNodes.Count; i++)
        {
            posNodeDistances.Add(lapDistance);
            lapDistance += Vector3.Distance(posNodes[i], posNodes[(i + 1) % posNodes.Count]);
        }
        for (int i = 1; i < posNodes.Count; i++)
            posNodeDistances[i] = lapDistance - posNodeDistances[i];

        // compile pos nodes and zones we got from the zone sequence
        CompilePosNodes();
        CompileZones();

        // fix up directional stuff
        MakeModulesDirectional();
        CorrectBridgeDirections();
        
        // weld the pipes
        WeldPipes();

        // fill empty cells with space units
        FillFloorWithSpacers();
        
        perfLogger.Log("Initialize");
    }

    public TrackCompiler(EditorTrack track, TrackUnitFile unitFile, float scale = 1f) : this(track, unitFile, false, scale)
    {
    }

    public TrackCompiler(EditorTrack track, TrackUnitFile unitFile, bool reversed, float scale = 1f)
    {
        this.originalTrack = track;
        this.unitFile = unitFile;
        this.track = track.Clone(false);
        this.exportScale = scale;
        this.reversed = reversed;
    }
}
