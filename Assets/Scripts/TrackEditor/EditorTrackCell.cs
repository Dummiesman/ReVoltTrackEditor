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

using UnityEngine;

public class EditorTrackCell
{
    public readonly Vector2Int Position = Vector2Int.zero;
    public ModulePlacement Module = null;
    public bool HasPickup = false;

    private bool CheckWall(EditorTrack track, Vector2Int offset)
    {
        //no module, no wall
        if (Module == null)
            return false;

        //no height, no wall to make
        if (Module.Elevation == 0f)
            return false;

        var checkPos = offset + Position;
        
        // edge wall
        if (checkPos.x < 0 || checkPos.x >= track.Width || checkPos.y < 0 || checkPos.y >= track.Height)
            return true;

        var checkCell = track.GetCell(checkPos);

        // if no module, wall is required
        if (checkCell.Module == null)
            return true;

        // if same module, no wall required
        if (checkCell.Module == this.Module)
            return false;

        // finally, the only condition we're left with is if the adjacent module has a lower elevation
        return checkCell.Module.Elevation < this.Module.Elevation;
    }

    /// <summary>
    /// Check if a wall is required to be placed in the direction of <paramref name="direction">
    /// </summary>
    public bool CheckWall(EditorTrack track, Modules.Direction direction)
    {
        switch(direction)
        {
            case Modules.Direction.North:
                return CheckWall(track, new Vector2Int(0, 1));
            case Modules.Direction.East:
                return CheckWall(track, new Vector2Int(1, 0));
            case Modules.Direction.South:
                return CheckWall(track, new Vector2Int(0, -1));
            case Modules.Direction.West:
                return CheckWall(track, new Vector2Int(-1, 0));
        }
        return false;
    }

    public EditorTrackCell(Vector2Int position)
    {
        this.Position = position;
    }
}
