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

public abstract class EditorMode : MonoBehaviour
{
    public virtual string ModeName => this.GetType().Name;
    public virtual string HelpPath => null;
    public GameObject UI;

    public virtual void Update() {}
    public virtual void UpdateInput() { }
    public virtual void OnEnterMode() 
    {
        if (UI != null)
            UI.SetActive(true);
        this.gameObject.SetActive(true);
    }

    public virtual void OnExitMode() 
    {
        if (UI != null)
            UI.SetActive(false);
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Called on application start
    /// </summary>
    public virtual void Init() { }
}
