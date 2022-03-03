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
using Newtonsoft.Json;
using TMPro;

namespace ReVolt
{
    [System.Serializable]
    public struct CharacterMargin
    {
        public int Left;
        public int Right;

        public CharacterMargin(int left, int right)
        {
            Left = left;
            Right = right;
        }
    }

    [System.Serializable]
    public struct FontCharacterData
    {
        public int X;
        public int Y;
        public CharacterMargin Margin;
    }

    [System.Serializable]
    public struct SerializedFont
    {
        public Dictionary<char, FontCharacterData> Characters;
    }

    public class Font 
    {
        public string Name = "RVFont";
        public readonly Dictionary<char, FontCharacterData> Characters = new Dictionary<char, FontCharacterData>();
        public Texture2D Texture { get; private set; }

        public TMP_FontAsset CreateTMPAsset()
        {
            var asset = ScriptableObject.CreateInstance<TMP_FontAsset>();
            asset.atlas = Texture;

            asset.name = Name;

            

            return asset;
        }

        private void Load(TextAsset characterInfo, Texture2D texture)
        {
            this.Texture = texture;

            // load from json
            var sf = JsonConvert.DeserializeObject<SerializedFont>(characterInfo.text);
            Characters.Clear();
            foreach(var kvp in sf.Characters)
            {
                Characters.Add(kvp.Key, kvp.Value);
            }
        }
        public Font(TextAsset characterinfo, Texture2D texture)
        {
            Load(characterinfo, texture);
        }
    }
}