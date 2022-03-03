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

public static class TextureCache
{
    private static readonly Dictionary<string, Texture2D> cache = new Dictionary<string, Texture2D>();

    public static Texture2D Get(string name)
    {
        string key = name.ToLowerInvariant();
        if(!cache.TryGetValue(key, out var texture))
        {
            texture = FileHelper.LoadImage(name);
            cache[key] = texture;
        }
        return texture;
    }

    public static Texture2D GetTPage(int num)
    {
        string name = $"tpage_{num:D2}";
        string key = name;
        if (!cache.TryGetValue(key, out var texture))
        {
            texture = FileHelper.LoadTPage(num);
            cache[key] = texture;
        }
        return texture;
    }
}
