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

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ModuleItem : MonoBehaviour
    {
        public TMP_Text Text;
        public Image[] Cursors = new Image[0];
        public GameObject CursorContainer;

        public void Init(string name, bool hasVariant)
        {
            Text.text = name;
            CursorContainer.SetActive(hasVariant);
        }

        public void Init(LocString name, bool hasVariant)
        {
            Init(Localization.Lookup(name), hasVariant);
        }

        public void Init(Modules.ModuleGroup group)
        {
            Init(group.Name, group.HasVariant);
        }
        
        public void Select(int cursorIndex)
        {
            Text.color = Color.red;
            for(int i=0; i < Cursors.Length; i++)
            {
                Cursors[i].enabled = i == cursorIndex;
            }
        }

        public void Deselect()
        {
            Text.color = Color.white;
            foreach(var cursorImage in Cursors)
            {
                cursorImage.enabled = false;
            }
        }
    }
}