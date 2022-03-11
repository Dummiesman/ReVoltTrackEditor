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
using System.IO;
using System.Linq;
using TMPro;
using UI;
using UnityEngine;

public class LoadMode : EditorMode
{
    // Virtual properties
    public override string ModeName => Localization.Lookup(LocString.MENU_LOAD);
    public override string HelpPath => "HelpFiles/LoadMode";

    // Scene references
    public RectTransform TextLayoutContainer;
    public GameObject TextPrefab;
    public GameObject NoTracksText;

    public Arrow UpArrow;
    public Arrow DownArrow;

    //
    const int TEXT_COUNT = 10;

    private bool fileListEmpty => fileCount == 0;

    private TMP_Text[] texts;
    private int fileCount = 0;

    private int cursorPosition;
    private int topPosition;

    // Other
    private void OnCursorPosChanged()
    {
        // set top position
        if (cursorPosition < topPosition)
        {
            topPosition = cursorPosition;
            UpArrow.Flash();
        }
        if (cursorPosition >= (topPosition + TEXT_COUNT))
        {
            topPosition = cursorPosition - TEXT_COUNT + 1;
            DownArrow.Flash();
        }

        // update the arrow object states
        UpArrow.Image.enabled = topPosition > 0;
        DownArrow.Image.enabled = (fileCount - topPosition) > TEXT_COUNT;

        // update the texts
        for (int i = topPosition; i < topPosition + TEXT_COUNT; i++)
        {
            var text = texts[i - topPosition];
            text.color = (i == cursorPosition) ? Color.red : Color.white;
            text.text = (i < fileCount) ? SavedFilesList.SavedFiles[i].Name : string.Empty;
        }
    }

    private void DeleteSelectedTrack()
    {
        var savedFile = SavedFilesList.SavedFiles[cursorPosition];
        string question = string.Format(Localization.Lookup(LocString.PROMPT_REALLY_DELETE), savedFile.Name);

        var postDeleteCursorPos = Mathf.Max(0, cursorPosition - 1);
        int preDeleteCursorPos = cursorPosition;

        TrackEditor.Prompt.InitQuestionPrompt(question, () =>
        {
            File.Delete(savedFile.Path);
            TrackEditor.Instance.OpenLoadScreenNoAskSave();
            this.cursorPosition = postDeleteCursorPos;
            OnCursorPosChanged();
        },
        () =>
        {
            TrackEditor.Instance.OpenLoadScreenNoAskSave();
            this.cursorPosition = preDeleteCursorPos;
            OnCursorPosChanged();
        });
        TrackEditor.ShowPrompt();
    }

    private void LoadSelectedTrack()
    {
        var savedFile = SavedFilesList.SavedFiles[cursorPosition];
        try
        {
            string trackPath = savedFile.Path;
            TrackEditor.Track.Clear(); // clear all the old modules
            TrackEditor.Track.Load(trackPath);
            TrackEditor.Instance.SetTrackEditingMode();
        }
        catch (System.Exception ex)
        {
            // something happened, clear the track
            Debug.LogError($"Failed to load track file: '{new FileInfo(savedFile.Path).Name}': {ex}");
            TrackEditor.Track.Clear();
            TrackEditor.PlaySound(TrackEditor.SndWarning);
            TrackEditor.Instance.SetTrackEditingMode();
        }
    }

    // Virtuals
    public override void OnEnterMode()
    {
        base.OnEnterMode();

        TrackEditor.Instance.Camera.enabled = false;

        SavedFilesList.Update();
        fileCount = SavedFilesList.SavedFiles.Count;

        topPosition = 0;
        cursorPosition = 0;

        OnCursorPosChanged();

        NoTracksText.SetActive(fileListEmpty);
    }

    public override void OnExitMode()
    {
        base.OnExitMode();
        TrackEditor.Instance.Camera.enabled = true;
    }

    public override void UpdateInput()
    {
        // delete
        if (!fileListEmpty && Controls.LoadScreen.DeletePressed())
        {
            DeleteSelectedTrack();
        }

        // other
        if (!fileListEmpty && Controls.UI.ConfirmPressed())
        {
            LoadSelectedTrack();
        }
        if(Controls.UI.BackPressed())
        {
            TrackEditor.Instance.SetTrackEditingMode();
        }

        // move
        if (!fileListEmpty)
        {
            var movementDelta = Controls.DirectionalMovement.GetMovementDelta();
            if(movementDelta.y != 0)
            {
                int nextCursorPosition = cursorPosition + (movementDelta.y * -1);
                if(nextCursorPosition >= fileCount || nextCursorPosition < 0)
                {
                    // out of bounds
                    TrackEditor.PlaySound(TrackEditor.SndWarning);
                }
                else
                {
                    TrackEditor.PlaySound(TrackEditor.SndMenuMove);
                    cursorPosition = nextCursorPosition;
                    OnCursorPosChanged();
                }
            }
        }

        base.UpdateInput();
    }

    public override void Init()
    {
        base.Init();

        // init ui
        texts = new TMP_Text[TEXT_COUNT];
        for(int i=0; i < TEXT_COUNT; i++)
        {
            var textObj = Instantiate(TextPrefab, TextLayoutContainer, false);
            texts[i] = textObj.GetComponentInChildren<TMP_Text>();
        }
    }
}
