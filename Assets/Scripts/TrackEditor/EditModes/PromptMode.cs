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

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PromptMode : EditorMode
{
    public struct PromptItem
    {
        public string Text;
        public Action Action;

        public PromptItem(string text, Action action)
        {
            this.Text = text;
            this.Action = action;
        }

        public PromptItem(LocString text, Action action) : this(Localization.Lookup(text), action)
        {
        }
    }

    private enum PromptType
    {
        Question,
        Information,
        Warning
    }

    // Virtual properties
    public override string ModeName
    {
        get
        {
            switch (this.promptType)
            {
                case PromptType.Question:
                    return Localization.Lookup(LocString.PROMPT_QUESTION);
                default:
                case PromptType.Information:
                    return Localization.Lookup(LocString.PROMPT_INFO);
                case PromptType.Warning:
                    return Localization.Lookup(LocString.PROMPT_WARNING);
            }
        }
    }

    // Scene / Asset References
    public Sprite InfoIcon;
    public Sprite QuestionIcon;
    public TMP_Text AnswerText;
    public TMP_Text PromptText;
    public Image IconImage;

    //
    private int promptAnswerCursorPosition = 0;
    private PromptType promptType;
    private float remainingTime = 0f;

    private PromptItem[] questionItems;
    private Action cancelAction;
    private Action infoAction;

    // Other
    private void OnInfoAction()
    {
        infoAction?.Invoke();
    }

    private void OnConfirmPressed()
    {
        var item = questionItems[promptAnswerCursorPosition];
        item.Action?.Invoke();
    }

    private void OnCancelAction()
    {
        cancelAction?.Invoke();
    }
    
    private void OnCursorPosChanged()
    {
        string answerText = string.Empty;
        for(int i=0; i < questionItems.Length; i++)
        {
            string color = (promptAnswerCursorPosition == i) ? "red" : "white";
            answerText += $"<color={color}>{questionItems[i].Text}</color> / ";
        }
        if(answerText.Length > 0)
            answerText = answerText.Substring(0, answerText.Length - 3); // remove trailing space and slash
        AnswerText.text = answerText;
    }

    // Public interface
    public void InitQuestionPrompt(string text, int defaultItem, Action onCancel = null, params PromptItem[] items)
    {
        remainingTime = 0f;
        promptType = PromptType.Question;
        IconImage.sprite = QuestionIcon;
        promptAnswerCursorPosition = Mathf.Clamp(defaultItem, 0, items.Length - 1);
        AnswerText.gameObject.SetActive(true);
        PromptText.text = text;
        cancelAction = onCancel;
        questionItems = items;
        OnCursorPosChanged();
    }

    public void InitQuestionPrompt(string text, Action onCancel = null, params PromptItem[] items)
    {
        InitQuestionPrompt(text, 0, onCancel, items);
    }

    public void InitQuestionPrompt(LocString text, Action onCancel = null, params PromptItem[] items)
    {
        InitQuestionPrompt(Localization.Lookup(text), onCancel, items);
    }

    public void InitQuestionPrompt(LocString text, int defaultItem, Action onCancel = null, params PromptItem[] items)
    {
        InitQuestionPrompt(Localization.Lookup(text), defaultItem, onCancel, items);
    }

    public void InitQuestionPrompt(LocString text, Action onYes = null, Action onNo = null)
    {
        InitQuestionPrompt(Localization.Lookup(text), onYes, onNo);
    }

    public void InitQuestionPrompt(LocString text, Action onCancel, Action onYes = null, Action onNo = null)
    {
        InitQuestionPrompt(Localization.Lookup(text), onCancel, onYes, onNo);
    }

    public void InitQuestionPrompt(LocString text, int defaultItem, Action onYes = null, Action onNo = null)
    {
        InitQuestionPrompt(Localization.Lookup(text), defaultItem, onYes, onNo);
    }

    public void InitQuestionPrompt(LocString text, int defaultItem, Action onCancel, Action onYes = null, Action onNo = null)
    {
        InitQuestionPrompt(Localization.Lookup(text), defaultItem, onCancel, onYes, onNo);
    }

    public void InitQuestionPrompt(string text, Action onYes = null, Action onNo = null)
    {
        InitQuestionPrompt(text, 0, onYes, onNo);
    }

    public void InitQuestionPrompt(string text, Action onCancel, Action onYes = null, Action onNo = null)
    {
        InitQuestionPrompt(text, 0, onCancel, onYes, onNo);
    }

    public void InitQuestionPrompt(string text, int defaultItem, Action onYes = null, Action onNo = null)
    {
        InitQuestionPrompt(text, defaultItem, onNo, onYes, onNo);
    }

    public void InitQuestionPrompt(string text, int defaultItem, Action onCancel, Action onYes = null, Action onNo = null)
    {
        InitQuestionPrompt(text, defaultItem, onCancel, new PromptItem(LocString.ANSWER_YES, onYes), new PromptItem(LocString.ANSWER_NO, onNo));
    }

    public void InitInfoPrompt(string text, Action onClose = null)
    {
        remainingTime = 0f;
        promptType = PromptType.Information;
        IconImage.sprite = InfoIcon;
        AnswerText.gameObject.SetActive(false);
        PromptText.text = text;
        infoAction = onClose;
    }

    public void InitInfoPrompt(LocString text, Action onClose = null)
    {
        InitInfoPrompt(Localization.Lookup(text), onClose);
    }

    public void InitInfoPrompt(string text, float autoCloseTime, Action onClose = null)
    {
        InitInfoPrompt(text, onClose);
        remainingTime = autoCloseTime;
    }

    public void InitInfoPrompt(LocString text, float autoCloseTime, Action onClose = null)
    {
        InitInfoPrompt(Localization.Lookup(text), autoCloseTime, onClose);
    }

    public void InitWarningPrompt(string text, Action onClose = null)
    {
        InitInfoPrompt(text, onClose);
        promptType = PromptType.Warning;
    }

    public void InitWarningPrompt(LocString text, Action onClose = null)
    {
        InitWarningPrompt(Localization.Lookup(text), onClose);
    }

    public void InitWarningPrompt(string text, float autoCloseTime, Action onClose = null)
    {
        InitWarningPrompt(text, onClose);
        remainingTime = autoCloseTime;
    }

    public void InitWarningPrompt(LocString text, float autoCloseTime, Action onClose = null)
    {
        InitWarningPrompt(Localization.Lookup(text), autoCloseTime, onClose);
    }

    // Virtuals
    public override void OnEnterMode()
    {
        base.OnEnterMode();
        TrackEditor.Instance.Camera.enabled = false;
    }

    public override void OnExitMode()
    {
        base.OnExitMode();
        TrackEditor.Instance.Camera.enabled = true;
    }

    public override void Update()
    {
        base.Update();

        // timed screen
        if((promptType == PromptType.Information || promptType == PromptType.Warning) && remainingTime > 0f)
        {
            remainingTime -= Time.deltaTime;
            if(remainingTime < 0f)
            {
                OnInfoAction();
            }
        }
    }

    public override void UpdateInput()
    {
        // move cursor
        if (promptType == PromptType.Question)
        {
            if (Controls.DirectionalMovement.LeftPressed())
            {
                TrackEditor.PlaySound(TrackEditor.SndTypeText);
                promptAnswerCursorPosition -= 1;
                if (promptAnswerCursorPosition < 0) promptAnswerCursorPosition = questionItems.Length - 1;
                OnCursorPosChanged();
            }
            if (Controls.DirectionalMovement.RightPressed())
            {
                TrackEditor.PlaySound(TrackEditor.SndTypeText);
                promptAnswerCursorPosition += 1;
                if (promptAnswerCursorPosition >= questionItems.Length) promptAnswerCursorPosition = 0;
                OnCursorPosChanged();
            }
        }

        // confirm/back
        if (promptType == PromptType.Information || promptType == PromptType.Warning)
        {
            if(Controls.UI.ConfirmPressed() || Controls.UI.BackPressed())
            {
                OnInfoAction();
            }
        }
        else
        {
            if (Controls.UI.ConfirmPressed())
            {
                TrackEditor.PlaySound(TrackEditor.SndTypeText);
                OnConfirmPressed();
            }
            else if (Controls.UI.BackPressed())
            {
                OnCancelAction();
            }
        }

        base.UpdateInput();
    }

    // Mono
    private void OnEnable()
    {
        if (promptType == PromptType.Warning)
            TrackEditor.PlaySound(TrackEditor.SndWarning);
        else if(promptType == PromptType.Information)
            TrackEditor.PlaySound(TrackEditor.SndPlacePickup);
        else
            TrackEditor.PlaySound(TrackEditor.SndChoice);
    }
}
