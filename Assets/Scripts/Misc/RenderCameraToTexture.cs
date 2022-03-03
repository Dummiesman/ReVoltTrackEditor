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

public class RenderCameraToTexture 
{
    public static Texture2D Render(Camera cam, int width, int height)
    {
        var origTargetTexture = cam.targetTexture;
        var origClearFlags = cam.clearFlags;
        var origActiveRT = RenderTexture.active;

        var texTransparent = new Texture2D(width, height, TextureFormat.ARGB32, false);
        
        // Must use 24-bit depth buffer to be able to fill background.
        var tempRT = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32);
        var grabArea = new Rect(0, 0, width, height);

        RenderTexture.active = tempRT;
        cam.targetTexture = tempRT;
        cam.clearFlags = CameraClearFlags.SolidColor;

        // Simple: use a clear background
        cam.backgroundColor = Color.clear;
        cam.Render();
        texTransparent.ReadPixels(grabArea, 0, 0);
        texTransparent.Apply();

        // Cleanup
        cam.clearFlags = origClearFlags;
        cam.targetTexture = origTargetTexture;
        RenderTexture.active = origActiveRT;
        RenderTexture.ReleaseTemporary(tempRT);

        return texTransparent;
    }
}
