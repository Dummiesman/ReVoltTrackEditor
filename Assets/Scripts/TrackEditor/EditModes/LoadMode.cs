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
    private struct TdfInfo
    {
        public string Name;
        public string FilePath;
    }

    const int TEXT_COUNT = 10;

    private bool fileListEmpty => TdfFiles.Length == 0;

    private TMP_Text[] texts;
    private TdfInfo[] TdfFiles;

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
        DownArrow.Image.enabled = (TdfFiles.Length - topPosition) > TEXT_COUNT;

        // update the texts
        for (int i = topPosition; i < topPosition + TEXT_COUNT; i++)
        {
            int textIndex = i - topPosition;
            var text = texts[textIndex];

            text.color = (i == cursorPosition) ? Color.red : Color.white;
            text.text = (i < TdfFiles.Length) ? TdfFiles[i].Name : string.Empty;
        }
    }

    private void DeleteSelectedTrack()
    {
        var fileInfo = TdfFiles[cursorPosition];
        string question = string.Format(Localization.Lookup(LocString.PROMPT_REALLY_DELETE), fileInfo.Name);

        var postDeleteCursorPos = Mathf.Max(0, cursorPosition - 1);
        int preDeleteCursorPos = cursorPosition;

        TrackEditor.Prompt.InitQuestionPrompt(question, () =>
        {
            File.Delete(fileInfo.FilePath);
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
        try
        {
            string trackPath = TdfFiles[cursorPosition].FilePath;
            TrackEditor.Track.Clear(); // clear all the old modules
            TrackEditor.Track.Load(trackPath);
            TrackEditor.Instance.SetTrackEditingMode();
        }
        catch (System.Exception ex)
        {
            // something happened, clear the track
            Debug.LogError($"Failed to load track {new FileInfo(TdfFiles[cursorPosition].FilePath).Name}: {ex}");
            TrackEditor.Track.Clear();
            TrackEditor.PlaySound(TrackEditor.SndWarning);
            TrackEditor.Instance.SetTrackEditingMode();
        }
    }

    private void LoadFileList()
    {
        string[] files = FileHelper.GetSavedTrackFileList();
        List<TdfInfo> tempFileList = new List<TdfInfo>();

        // parse file headers
        for(int i=0; i < files.Length; i++)
        {
            string file = files[i];
            var editorTrack = new EditorTrack();
            try
            {
                using (var reader = new BinaryReader(File.OpenRead(file)))
                {
                    editorTrack.ReadBinaryHeader(reader);
                }

                tempFileList.Add(new TdfInfo()
                {
                    FilePath = file,
                    Name = editorTrack.Name
                });
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"LoadMode: Failed to parse file header for {new FileInfo(file).Name}: {ex}");
            }
        }

        // then, order by name
        TdfFiles = tempFileList.OrderBy(x => x.Name).ToArray();
        Debug.Log($"LoadMode: found {TdfFiles.Length} files");
    }

    // Virtuals
    public override void OnEnterMode()
    {
        base.OnEnterMode();

        TrackEditor.Instance.Camera.enabled = false;

        topPosition = 0;
        cursorPosition = 0;
        LoadFileList();
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

        //move
        if (!fileListEmpty)
        {
            if (Controls.DirectionalMovement.DownPressed())
            {
                if (cursorPosition == TdfFiles.Length - 1)
                {
                    //at end already
                    TrackEditor.PlaySound(TrackEditor.SndWarning);
                }
                else
                {
                    TrackEditor.PlaySound(TrackEditor.SndMenuMove);
                    cursorPosition++;
                    OnCursorPosChanged();
                }

            }
            else if (Controls.DirectionalMovement.UpPressed())
            {
                if (cursorPosition == 0)
                {
                    //at top already
                    TrackEditor.PlaySound(TrackEditor.SndWarning);
                }
                else
                {
                    TrackEditor.PlaySound(TrackEditor.SndMenuMove);
                    cursorPosition--;
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
