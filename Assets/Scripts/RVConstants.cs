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

public class RVConstants
{
    public const float GameScale = 250f;
    public const int GOURAUD_SHIFTED = 0x8000;

    public const int TPAGE_COUNT = 8;

    public const int PEG_INDEX = 0;
    public const int PAN_INDEX = 1;
    public const int HULL_INDEX = 2;

    public const int UV_REVERSED = 0x80;

    public const float  SMALL_CUBE_SIZE = GameScale * 4.0f;
    public const float  SMALL_CUBE_HALF = SMALL_CUBE_SIZE / 2.0f;
    public const float  BIG_CUBE_SIZE = SMALL_CUBE_SIZE * 4.0f;
    
    public const float  ElevationStep = GameScale / 2;
}
