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

namespace ReVolt.TrackUnit.Unity
{
    public class ModuleInstance : MonoBehaviour
    {
        public int IndexInFile;
        public IReadOnlyCollection<UnitInstance> Units => units;

        [SerializeField]
        private UnitInstance[] units;
        
        public void Init(GameObject root)
        {
            units = root.GetComponentsInChildren<UnitInstance>();
            SetRootHeight(0f);
        }

        public void SetRootHeight(float height)
        {
            foreach (var unit in this.units)
                unit.SetRootHeight(height);
        }

        public void SetModuleScreenConfig()
        {
            foreach (var unit in units)
                unit.SetModuleScreenConfig();
        }

        public void SetEditorConfig()
        {
            foreach (var unit in units)
                unit.SetEditorConfig();
        }
    }
}
