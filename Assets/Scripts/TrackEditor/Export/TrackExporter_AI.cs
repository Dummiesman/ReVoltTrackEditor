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
using System.IO;
using UnityEngine;

public partial class TrackExporter
{
    public bool AIIsValid { get; private set; } = false;
    public int AINodeCount => processedAiNodes.Count;
    public Vector2Int LastAICell { get; private set; }

    const float AIMergeThreshold =  20f;
    const float AIHeightMergeThreshold = 250f;

    private readonly List<ReVolt.TrackUnit.AINode> processedAiNodes = new List<ReVolt.TrackUnit.AINode>(1024);

    private IEnumerable<ReVolt.TrackUnit.AINode> EnumRoute(ReVolt.TrackUnit.Route route, int direction, bool flipped)
    {
        for (int i = 0; i < route.Nodes.Count; i++)
        {
            int index = (direction < 0) ? route.Nodes.Count - i - 1 : i;
            var node = route.Nodes[index];

            yield return new ReVolt.TrackUnit.AINode()
            {
                RedPosition = (flipped) ? node.GreenPosition : node.RedPosition,
                GreenPosition = (flipped) ? node.RedPosition : node.GreenPosition,
                Priority = node.Priority,
                RacingLine = (flipped) ? 1f - node.RacingLine : node.RacingLine
            };
        }
    }

    private bool CheckNodeMatch(Vector3 next, Vector3 current)
    {
        return next.y >= current.y && Vector2.Distance(next.ToVec2XZ(), current.ToVec2XZ()) <= AIMergeThreshold;
    }

    private bool CheckNodeMatch(Vector3 lastGreenPos, Vector3 lastRedPos, ModulePlacement nextModule, 
                                out int routeIndex, out bool flipped, out int direction, out float heightDelta)
    {
        direction = 0;
        routeIndex = -1;
        flipped = false;
        heightDelta = 0f;

        var module = unitFile.Modules[nextModule.ModuleIndex];
        var modMatrix = MakeModuleMatrix(nextModule);

        for (int i = 0; i < module.Routes.Count; i++)
        {
            routeIndex = i;
            var route = module.Routes[i];
            for (int j = 0; j < 2; j++)
            { 
                var checkNode = route.Nodes[(j == 0) ? 0 : route.Nodes.Count - 1];
                Vector3 greenPos = modMatrix.MultiplyPoint3x4(checkNode.GreenPosition);
                Vector3 redPos = modMatrix.MultiplyPoint3x4(checkNode.RedPosition);

                if (CheckNodeMatch(greenPos, lastRedPos) || CheckNodeMatch(redPos, lastGreenPos))
                {
                    heightDelta = ((lastRedPos.y - greenPos.y) + (lastGreenPos.y - redPos.y)) / 2f;
                    direction = (j == 0) ? 1 : -1;
                    flipped = true;
                    return true;
                }
                else if (CheckNodeMatch(redPos, lastRedPos) || CheckNodeMatch(greenPos, lastGreenPos))
                {
                    heightDelta = ((lastRedPos.y - redPos.y) + (lastGreenPos.y - greenPos.y)) / 2f;
                    direction = (j == 0) ? 1 : -1;
                    flipped = false;
                    return true;
                }
             }
        }
        return false;
    }

    public void CreateAINodes()
    {
        var perfLogger = new PerfTimeLogger("Export:AI");
        LastAICell = Vector2Int.zero;
        processedAiNodes.Clear();

        if (zoneSequence.Count == 0 || this.startModule == null)
        {
            AIIsValid = false;
            return;
        }

        Vector3 lastGreenPos = Vector3.zero;
        Vector3 lastRedPos = Vector3.zero;
        
        var startZone = zones[zoneSequence[0].ZoneID];
        var startCell = track.GetCell(startZone.CellCoordinate);
        var startModule = unitFile.Modules[startCell.Module.ModuleIndex];
        var startModMatrix = MakeModuleMatrix(startCell.Module);

        //add the start nodes
        if (startModule.Routes.Count == 0) // this shouldn't happen
        {
            AIIsValid = false;
            return;
        }

        foreach (var node in EnumRoute(startModule.Routes[0], 1, false))
        {
            lastRedPos = startModMatrix.MultiplyPoint3x4(node.RedPosition);
            lastGreenPos = startModMatrix.MultiplyPoint3x4(node.GreenPosition);
            processedAiNodes.Add(new ReVolt.TrackUnit.AINode() { RedPosition = lastRedPos, GreenPosition = lastGreenPos, 
                                                                 RacingLine = node.RacingLine, Priority = node.Priority });
        }

        //now go through the rest of the modules
        foreach(var placement in EnumModules())
        {
            LastAICell = placement.Position;

            // if this fails, there has been a continuity error
            if (!CheckNodeMatch(lastGreenPos, lastRedPos, placement, out int routeIndex, out bool flipped, out int direction, out float heightDelta) )
            {
                Debug.LogError($"AI Continuity Error");
                AIIsValid = false;
                return;
            }

            var module = unitFile.Modules[placement.ModuleIndex];
            var modMatrix = MakeModuleMatrix(placement);

            bool first = true;
            foreach(var node in EnumRoute(module.Routes[routeIndex], direction, flipped))
            {
                //merge and blend if first
                if (first)
                {
                    first = false;
                    processedAiNodes[AINodeCount - 1].RacingLine = (processedAiNodes[AINodeCount - 1].RacingLine + node.RacingLine) / 2f;
                    processedAiNodes[AINodeCount - 1].Priority = node.Priority;
                    continue;
                }

                // add the rest of the nodes to the list
                lastRedPos = modMatrix.MultiplyPoint3x4(node.RedPosition);
                lastGreenPos = modMatrix.MultiplyPoint3x4(node.GreenPosition);
                processedAiNodes.Add(new ReVolt.TrackUnit.AINode() { GreenPosition = lastGreenPos, RedPosition = lastRedPos, 
                                                                     RacingLine = node.RacingLine, Priority = node.Priority });
            }
        }

        // check if a valid loop has been made
        // count > start module node count && end == start
        AIIsValid = processedAiNodes.Count > 2 && CheckNodeMatch(processedAiNodes[0].RedPosition, processedAiNodes[processedAiNodes.Count - 1].RedPosition)
                                               && CheckNodeMatch(processedAiNodes[0].GreenPosition, processedAiNodes[processedAiNodes.Count - 1].GreenPosition);

        // remove last node, it's on top of the first node
        processedAiNodes.RemoveAt(processedAiNodes.Count - 1);

        perfLogger.Log("Create");
    }

    public void WriteAINodes()
    {
        var aiNodesFile = new ReVolt.Track.AINodesFile()
        {
            TotalDistance = lapDistance,
            StartNode = 1
        };

        for(int i=0; i < AINodeCount; i++)
        {
            var node = processedAiNodes[i];

            var fileNode = new ReVolt.Track.AINode()
            {
                StartNode = 1,
                RacingLine = 1f - node.RacingLine,
                OvertakingLine = 1f - node.RacingLine,
                RacingLineSpeed = 30,
                CenterSpeed = 30,
                PreviousLinkIDs = new int[2] { (i + (AINodeCount - 1)) % AINodeCount, -1 },
                NextLinkIDs = new int[2] { (i + 1) % AINodeCount, -1 },
                Priority = node.Priority,
                RedEnd = new ReVolt.Track.AINodeEnd()
                {
                    Speed = 30,
                    Position = node.RedPosition
                },
                GreenEnd = new ReVolt.Track.AINodeEnd()
                {
                    Speed = 30,
                    Position = node.GreenPosition
                }
            };

            aiNodesFile.Nodes.Add(fileNode);
        }

        if(exportScale != 1f)
        {
            aiNodesFile.Scale(exportScale);
        }

        string aiFilePath = Path.Combine(exportPath, $"{trackFolderName}.fan");
        aiNodesFile.Save(aiFilePath);
    }
}
