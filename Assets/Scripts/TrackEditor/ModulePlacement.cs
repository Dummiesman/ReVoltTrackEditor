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

public class ModulePlacement 
{
    /// <summary>
    /// Rotation expressed in # of 90 degree clockwise turns
    /// </summary>
    public int Rotation;
    public Vector2Int Position;
    public float Elevation;
    public int ModuleIndex;
    public GameObject Object;
    public readonly List<EditorTrackCell> TouchedCells = new List<EditorTrackCell>();
    public EditorTrackCell RootCell;

    public void PositionObject()
    {
        if (Object == null)
            return;
        Object.transform.position = new Vector3(Position.x, Elevation, Position.y);
        Object.transform.rotation = Quaternion.Euler(0f, 90f * Rotation, 0f);
    }

    public bool IsValid(EditorTrack track)
    {
        if (Position.x < 0 || Position.y < 0 || Position.x >= track.Width || Position.y >= track.Height)
            return false;

        int minCellX = int.MaxValue;
        int minCellY = int.MaxValue;
        int maxCellX = int.MinValue;
        int maxCellY = int.MinValue;

        foreach(var cell in TouchedCells)
        {
            minCellX = Mathf.Min(minCellX, cell.Position.x);
            minCellY = Mathf.Min(minCellY, cell.Position.y);
            maxCellX = Mathf.Max(maxCellX, cell.Position.x);
            maxCellY = Mathf.Max(maxCellY, cell.Position.y);
        }

        return (minCellX >= 0 && minCellY >= 0 && maxCellX < track.Width && maxCellY < track.Height);
    }

    public void Delete()
    {
        foreach(var cell in TouchedCells)
        {
            cell.HasPickup = false;
            cell.Module = null;
        }
        if(Object != null)
            UnityEngine.Object.Destroy(Object);
    }
}
