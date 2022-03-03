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

using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SaveMode : EditorMode
{
    // Virtual properties
    public override string ModeName => Localization.Lookup(LocString.MENU_SAVE);
    public override string HelpPath => "HelpFiles/SaveMode";

    // Scene references
    public TMP_Text InputFieldText;
    
    public RectTransform KeyboardLayout;
    public GameObject KeyPrefab;

    public TMP_Text SaveText;

    //
    public System.Action<string> AfterSaveCallback;

    // cursor stuff
    private Vector2Int cursorPos = Vector2Int.zero;
    private int cursorPosIndex => (cursorPos.y == SAVE_BUTTON_VPOS) ? -1 : (cursorPos.y * KEYS_PER_LINE) + cursorPos.x;
    private TMP_Text[] keyTexts;

    // text stuff
    private string currentText = string.Empty;

    //
    private const int MAX_TEXT_LEN = 45;
    private const string KEYBOARD = "!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~«¬";
    private const int KEYS_PER_LINE = 16;
    private const int SAVE_BUTTON_VPOS = 6;

    // Other
    private bool IsSpecialChar(char obj)
    {
        return obj != '\b' && (obj == '\x1B' || obj == '\r' || obj == '\n' || obj < '\x20');            
    }

    private void OnTextChanged()
    {
        InputFieldText.text = currentText;
    }

    private void OnTextInput(char obj)
    {
        if (IsSpecialChar(obj))
            return;

        if (obj == '\xAC') // space key stand-in
            obj = ' ';
        if (obj == '\xAB') // backspace key stand in
            obj = '\b';

        if (obj == '\b') // backspace key
        {
            if (currentText.Length > 0)
                currentText = currentText.Substring(0, currentText.Length - 1);
        }
        else
        {
            if (currentText.Length < MAX_TEXT_LEN)
                currentText += obj;
        }

        OnTextChanged();
    }

    private void Keyboard_OnTextInput(char obj)
    {
        if(!IsSpecialChar(obj))
            TrackEditor.PlaySound(TrackEditor.SndTypeText);
        OnTextInput(obj);
    }

    private void OnCurorPosChanged()
    {
        for(int i=0; i < keyTexts.Length; i++)
        {
            keyTexts[i].color = (i == cursorPosIndex) ? Color.red : Color.white;
        }
        SaveText.color = (cursorPos.y == SAVE_BUTTON_VPOS) ? Color.red : Color.white;
    }

    private void SaveTrack()
    {
        string newTrackName = currentText.Trim();
        string fullPath = FileHelper.GetTrackSavePath(newTrackName);
        FileHelper.EnsureSaveDirectoryExists();

        // set new track name
        TrackEditor.Track.Name = newTrackName;

        // save
        void SaveAction()
        {
            TrackEditor.Track.Save(fullPath);
            TrackEditor.Track.ClearDirtyFlag();

            TrackEditor.Prompt.InitInfoPrompt(LocString.PROMPT_TRACK_SAVED, 3f, TrackEditor.Instance.SetTrackEditingMode);
            TrackEditor.ShowPrompt();
            
            AfterSaveCallback?.Invoke(fullPath);
            AfterSaveCallback = null;
        }

        if (File.Exists(fullPath))
        {
            TrackEditor.Prompt.InitQuestionPrompt(LocString.PROMPT_OVERWRITE_TRACK,
            () =>
            {
                File.Delete(fullPath);
                SaveAction();
            },
            () =>
            {
                string oldName = currentText; // save this
                TrackEditor.Instance.OpenSaveScreen();
                this.SetSaveName(oldName);
            });
            TrackEditor.ShowPrompt();
        }
        else
        {
            SaveAction();
        }
    }

    // Public interface
    public void SetSaveName(string name)
    {
        currentText = name;
        OnTextChanged();
    }

    // Virtuals
    public override void Init()
    {
        // init the keyboard layout
        RectTransform curLineTransform = null;
        keyTexts = new TMP_Text[KEYBOARD.Length];

        for(int i=0; i < KEYBOARD.Length; i++)
        {
            // start new layout row
            if(i % KEYS_PER_LINE == 0)
            {
                var keyRow = new GameObject("KeyRow", typeof(RectTransform), typeof(HorizontalLayoutGroup));
                keyRow.transform.SetParent(KeyboardLayout, false);
                curLineTransform = keyRow.transform as RectTransform;
            }

            // add key to row
            var key = Instantiate(KeyPrefab, curLineTransform, false);
            var keyText = key.GetComponentInChildren<TMP_Text>();
            
            keyText.text = KEYBOARD[i].ToString();
            keyTexts[i] = keyText;
        }
    }

    public override void UpdateInput()
    {
        // keyboard navigation
        var movementDelta = Controls.DirectionalMovement.GetMovementDelta();

        // don't allow x position movements while on save button
        if (cursorPos.y == SAVE_BUTTON_VPOS)
            movementDelta.x = 0;

        if (movementDelta.sqrMagnitude > 0)
        {
            TrackEditor.PlaySound(TrackEditor.SndTypeText);

            movementDelta.y *= -1;
            cursorPos += movementDelta;
            cursorPos.y = (cursorPos.y < 0) ? SAVE_BUTTON_VPOS : cursorPos.y % 7;
            cursorPos.x = (cursorPos.x < 0) ? KEYS_PER_LINE - 1 : cursorPos.x % KEYS_PER_LINE;
            OnCurorPosChanged();
        }

        // gamepad space and backspace
        if (Controls.SaveScreen.GamepadBackspacePressed())
        {
            TrackEditor.PlaySound(TrackEditor.SndTypeText);
            OnTextInput('\b');
        }

        if (Controls.SaveScreen.GamepadSpacePressed())
        {
            TrackEditor.PlaySound(TrackEditor.SndTypeText);
            OnTextInput(' ');
        }

        // ui navigation
        if (Controls.UI.ConfirmPressed())
        {
            if (cursorPos.y == SAVE_BUTTON_VPOS)
            {
                SaveTrack();
            }
            else
            {
                TrackEditor.PlaySound(TrackEditor.SndTypeText);
                OnTextInput(KEYBOARD[cursorPosIndex]);
            }
        }

        if (Controls.UI.BackPressed())
        {
            TrackEditor.SetEditMode<TrackEditingMode>();
        }

        base.UpdateInput();
    }

    public override void OnEnterMode()
    {
        base.OnEnterMode();

        // disable rendering
        TrackEditor.Instance.Camera.enabled = false;

        // get the name from the track
        currentText = TrackEditor.Track.Name;
        OnTextChanged();

        // set default cursor pos
        cursorPos = new Vector2Int(KEYS_PER_LINE / 2, SAVE_BUTTON_VPOS);
        OnCurorPosChanged();

        // hook text input
        if (Keyboard.current is Keyboard keyboard)
        {
            keyboard.onTextInput += Keyboard_OnTextInput;
        }
    }

    public override void OnExitMode()
    {
        base.OnExitMode();

        // enable rendering
        TrackEditor.Instance.Camera.enabled = true;

        // unhook text input
        if (Keyboard.current is Keyboard keyboard)
        {
            keyboard.onTextInput -= Keyboard_OnTextInput;
        }
    }
}