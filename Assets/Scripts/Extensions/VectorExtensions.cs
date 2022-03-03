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

public static class VectorExtensions 
{
    public const int ROTATION_CLOCKWISE = 1;
    public const int ROTATION_COUNTERCLOCKWISE = -1;

    /// <summary>
    /// Rotate this vector by <paramref name="rotations"/>rotations</paramref> amount of 90 degree clockwise rotations
    /// </summary>
    public static Vector2Int Rotate(this Vector2Int vec, int rotations)
    {
        if (vec.x == 0 && vec.y == 0)
            return Vector2Int.zero;

        float rad = 90f * rotations * Mathf.Deg2Rad;
        float s = Mathf.Sin(rad);
        float c = Mathf.Cos(rad);

        return new Vector2Int(
            Mathf.RoundToInt(vec.x * c + vec.y * s),
            Mathf.RoundToInt(vec.y * c - vec.x * s)
        );
    }

    /// <summary>
    /// Rotate this vector by <paramref name="rotations"/>rotations</paramref> amount of 90 degree clockwise rotations
    /// </summary>
    public static Vector2Int Rotate(this Vector2Int vec, Vector2 pivot, int rotations)
    {
        float rad = 90f * rotations * Mathf.Deg2Rad;
        float s = Mathf.Sin(rad);
        float c = Mathf.Cos(rad);

        var offset = vec - pivot;
        if (offset.x == 0 && offset.y == 0)
            return Vector2Int.zero;

        return new Vector2Int(
            Mathf.RoundToInt((offset.x * c + offset.y * s) + pivot.x),
            Mathf.RoundToInt((offset.y * c - offset.x * s) + pivot.y)
        );
    }

    /// <summary>
    /// Rotate this rect by <paramref name="rotations"/>rotations</paramref> amount of 90 degree clockwise rotations
    /// </summary>
    public static RectInt Rotate(this RectInt rect, int rotations)
    {
        var newMin = rect.min.Rotate(rotations);
        var newMax = rect.max.Rotate(rotations);
        
        var newRect = new RectInt(Vector2Int.zero, Vector2Int.zero);
        newRect.SetMinMax(newMin, newMax);
        return newRect;
    }

    /// <summary>
    /// Rotate this rect by <paramref name="rotations"/>rotations</paramref> amount of 90 degree clockwise rotations
    /// </summary>
    public static RectInt Rotate(this RectInt rect, Vector2 pivot, int rotations)
    {
        var newMin = rect.min.Rotate(pivot, rotations);
        var newMax = rect.max.Rotate(pivot, rotations);

        var newRect = new RectInt(Vector2Int.zero, Vector2Int.zero);
        newRect.SetMinMax(newMin, newMax);
        return newRect;
    }

    /// <summary>
    /// Checks if vec is completely horizontal on one of the axes ex. (y = 0, x = 1, z = 0) / (y = 0, x = 0, z = 1)
    /// </summary>
    public static bool IsHorizontalNormal(this Vector3 vec, float tolerance)
    {
        if (Mathf.Abs(vec.y) > tolerance)
            return false;
        if(Mathf.Abs(vec.x) >= (1f - tolerance))
        {
            return Mathf.Abs(vec.z) < tolerance;
        }
        else if(Mathf.Abs(vec.z) >= (1f - tolerance))
        {
            return Mathf.Abs(vec.x) < tolerance;
        }
        return false;
    }

    /// <summary>
    /// Returns new Vector2Int(vec.x, vec.z)
    /// </summary>
    public static Vector2Int ToVec2XZ(this Vector3Int vec)
    {
        return new Vector2Int(vec.x, vec.z);
    }

    /// <summary>
    /// Returns new Vector3Int(vec.x, 0, vec.y)
    /// </summary>
    public static Vector3Int ToVec3XZ(this Vector2Int vec)
    {
        return new Vector3Int(vec.x, 0, vec.y);
    }

    /// <summary>
    /// Returns new Vector2Int(vec.x, vec.z)
    /// </summary>
    public static Vector2 ToVec2XZ(this Vector3 vec)
    {
        return new Vector2(vec.x, vec.z);
    }

    /// <summary>
    /// Returns new Vector3Int(vec.x, 0, vec.y)
    /// </summary>
    public static Vector3 ToVec3XZ(this Vector2 vec)
    {
        return new Vector3(vec.x, 0, vec.y);
    }
}
