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
using UnityEngine.Events;
using UnityEngine.UI;

public class PauseMenuItem : MonoBehaviour
{
    public Image Image;

    public Sprite ActiveSprite;
    public Sprite InactiveSprite;

    public SpriteAnimation FlipBackAnim;
    public SpriteAnimation FlipAnim;

    public UnityEvent OnClicked;

    public void Click()
    {
        OnClicked?.Invoke();
    }

    public void StopAnimation()
    {
        if(FlipBackAnim.IsPlaying)
            FlipBackAnim.Stop();
        if (FlipAnim.IsPlaying)
            FlipAnim.Stop();
    }

    public void ActivateImmediate()
    {
        Image.sprite = ActiveSprite;
    }

    public void DeactivateImmediate()
    {
        Image.sprite = InactiveSprite;
    }

    public void Activate()
    {
        if (FlipBackAnim.IsPlaying)
            FlipBackAnim.Pause();
        FlipAnim.Play();
    }

    public void Deactivate()
    {
        if (FlipAnim.IsPlaying)
            FlipAnim.Pause();
        FlipBackAnim.Play();
    }
}
