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

public class PauseMenu : MonoBehaviour
{
    private int selectedIndex = 0;
    
    public PauseMenuItem[] MenuItems = new PauseMenuItem[0];
    public RectTransform Content;
    public float AnimateTime = 0.25f;

    const float SLIDE_IN_POS = -116f;
    const float SLIDE_OUT_POS = 125f;

    private bool animating = false;
    private float animateTarget = 0f;
    private float animationOrigin = 0f;
    private float currentAnimationTime = 0f;
    private bool menuItemClicked = false;
    private bool closing = false;

    private void Update()
    {
        if (animating)
        {
            currentAnimationTime += Time.deltaTime;
            if(currentAnimationTime >= AnimateTime)
            {
                // animation just finished
                currentAnimationTime = AnimateTime;
                animating = false;

                // activate selected menu item
                if(closing)
                {
                    this.gameObject.SetActive(false);
                    if(menuItemClicked)
                        MenuItems[selectedIndex].Click();
                }
            }

            float position = Mathf.Lerp(animationOrigin, animateTarget, currentAnimationTime / AnimateTime);
            Content.anchoredPosition = new Vector2(position, Content.anchoredPosition.y);
        }
    }

    private void SetInitialState()
    {
        for(int i = 0; i < MenuItems.Length; i++)
        {
            MenuItems[i].StopAnimation();
            if (i == selectedIndex)
                MenuItems[i].ActivateImmediate();
            else
                MenuItems[i].DeactivateImmediate();
        }
    }

    public void UpdateInput()
    {
        if (closing)
            return;

        int prevSelected = selectedIndex;
        int newSelected = selectedIndex;

        if (Controls.DirectionalMovement.DownPressed())
        {
            newSelected++;
            if (newSelected >= MenuItems.Length)
                newSelected = 0;
        }
        else if (Controls.DirectionalMovement.UpPressed())
        {
            newSelected--;
            if (newSelected < 0)
                newSelected = MenuItems.Length - 1;
        }

        if (Controls.UI.ConfirmPressed())
        {
            Close();
            menuItemClicked = true;
        }

        if (Controls.UI.BackPressed() || Controls.PausePressed())
        {
            Close();
        }

        if(prevSelected != newSelected)
        {
            TrackEditor.PlaySound(TrackEditor.SndMenuMove);
            MenuItems[prevSelected].Deactivate();
            MenuItems[newSelected].Activate();
            selectedIndex = newSelected;
        }
    }

    // Opening and closing
    public void Open()
    {
        TrackEditor.PlaySound(TrackEditor.SndMenuOpen);
        selectedIndex = 0;
        SetInitialState();

        this.gameObject.SetActive(true);
        animateTarget = SLIDE_IN_POS;
        animationOrigin = SLIDE_OUT_POS;
        currentAnimationTime = animating ? (AnimateTime - currentAnimationTime) : 0f;
        animating = true;
        closing = false;
        menuItemClicked = false;
        Update();
    }

    public void Close()
    {
        TrackEditor.PlaySound(TrackEditor.SndMenuClose);

        animateTarget = SLIDE_OUT_POS;
        animationOrigin = SLIDE_IN_POS;
        currentAnimationTime = animating ? (AnimateTime - currentAnimationTime) : 0f;
        animating = true;
        closing = true;
    }

    public void ToggleOpen()
    {
        if (this.gameObject.activeInHierarchy)
            Close();
        else
            Open();
    }
}
