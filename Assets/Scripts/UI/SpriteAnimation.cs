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
using UnityEngine.UI;

public class SpriteAnimation : MonoBehaviour
{
    public float FrameRate = 16f; // Frame rate
    public bool IsPlaying => playing;
    public float Time => time;

    public Image Image;
    public SpriteList Sprites;

    private bool playing = false;
    
    private float rate => 1f / FrameRate;
    private float time = 0f;

    public void Play()
    {
        time = 0f;
        playing = true;
        Update();
    }

    public void Stop()
    {
        time = 0f;
        playing = false;
        Update();
    }

    public void Pause()
    {
        playing = false;
        Update();
    }

    private void Awake()
    {
        if(Image == null)
            Image = GetComponent<Image>();
    }

    private void Update()
    {
        if (!playing)
            return;
        if (Image == null || Sprites == null || Sprites.sprites == null || Sprites.sprites.Length == 0)
            return;

        // advance time
        time += UnityEngine.Time.deltaTime;
        int frame = Mathf.FloorToInt(time * FrameRate);
        
        // check if done
        if(frame >= Sprites.sprites.Length)
        {
            frame = Sprites.sprites.Length - 1;
            playing = false;
        }

        // assign image
        Image.sprite = Sprites.sprites[frame];
    }

}
