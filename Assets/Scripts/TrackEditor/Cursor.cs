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

using Dummiesman.DebugDraw;
using UnityEngine;

public class Cursor 
{
    public Vector3 Size = Vector3.one;
    public Vector2Int Position = Vector2Int.zero;
    public Color Color = Color.white;

    public void Draw()
    {
        DebugDraw.PushMatrix();
        DebugDraw.Identity();
        DebugDraw.Color = Color;
        DebugDraw.DrawBox(Position.ToVec3XZ(), Size);
        DebugDraw.PopMatrix();
    }
}
