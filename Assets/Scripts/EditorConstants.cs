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

public class EditorConstants 
{
    public static readonly Vector2 ModuleBoundsPivot = new Vector2(0.5f, 0.5f);

    public const float ElevationStep = 0.125f;
    public const int MaxElevationSteps = 20;
    public const float MaxElevation = (ElevationStep * MaxElevationSteps);

    public const int MaxGridSize = 20;
    public const int MinGridSize = 10;

    public static readonly Color RootColor = Color.grey;
    public static readonly Color RootColorInGameMax = new Color32(100, 220, 230, 255);
    public static readonly Color RootColorInGameMin = new Color32(10, 40, 50, 255);
}
