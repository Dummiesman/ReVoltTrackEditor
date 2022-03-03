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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Unity does letterboxing as a default, hardcoded behaviour
/// Nobody who tested the application wants this behaviour, and neither do I
/// </summary>
public class FullscreenFix : MonoBehaviour
{
#if !UNITY_ANDROID
    private bool prevFullScreen;
    private Vector2Int lastWindowedResolution;
    private bool hasEverBeenFullscreen;

    private void Awake()
    {
        prevFullScreen = Screen.fullScreen;
        hasEverBeenFullscreen |= prevFullScreen;
    }

    void Update()
    {
        if (Screen.fullScreen != prevFullScreen)
        {
            if (Screen.fullScreen)
            {
                hasEverBeenFullscreen = true;
                lastWindowedResolution = new Vector2Int(Screen.width, Screen.height);
                
                // We just entered fullscreen
                var lastResolution = Screen.resolutions.Last();
                Screen.SetResolution(lastResolution.width, lastResolution.height, true);
            }
            else if (hasEverBeenFullscreen)
            {
                // Restore windowed resolution
                Screen.SetResolution(lastWindowedResolution.x, lastWindowedResolution.y, false);
            }

            prevFullScreen = Screen.fullScreen;
        }
    }
#endif
}
