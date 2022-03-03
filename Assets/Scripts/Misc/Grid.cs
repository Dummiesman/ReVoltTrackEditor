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

public class Grid : MonoBehaviour {
    public bool Worldspace = false;
    public float SquareSize = 1.0f;
    public int NumHorizontalSquares = 16;
    public int NumVerticalSquares = 16;
    public Color Color = new Color(0.2f, 0.2f, 0.2f);
    public Vector2 Origin = new Vector2(-0.5f, -0.5f);

    private void Update()
    {
        DebugDraw.PushMatrix();
        DebugDraw.Color = Color;

        if (Worldspace)
            DebugDraw.Identity();
        else
            DebugDraw.Matrix = transform.localToWorldMatrix;

        // draw grid
        float sizeX = NumHorizontalSquares * SquareSize;
        float sizeY = NumVerticalSquares * SquareSize;

        float extentsX = (NumHorizontalSquares / 2f) * SquareSize;
        float extentsZ = (NumVerticalSquares / 2f) * SquareSize;

        float originX = sizeX * Origin.x;
        float originY = sizeY * Origin.y;

        float currentOffsetX = originX;
        float currentOffsetZ = originY;

        DebugDraw.Begin(DrawType.LINES);
        for(int i=0; i <= NumVerticalSquares; i++)
        {
            DebugDraw.Vertex(originX, 0, currentOffsetZ);
            DebugDraw.Vertex(originX + sizeX, 0, currentOffsetZ);
            currentOffsetZ += SquareSize;
        }

        for(int i=0; i <= NumHorizontalSquares; i++)
        {
            DebugDraw.Vertex(currentOffsetX, 0, originY);
            DebugDraw.Vertex(currentOffsetX, 0, originY + sizeY);
            currentOffsetX += SquareSize;
        }

        // finish off
        DebugDraw.PopMatrix();
        DebugDraw.End(); 
    }
}
