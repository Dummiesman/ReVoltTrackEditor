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

using System.Linq;
using TMPro;
using UI;
using UnityEngine;

public class AdjustMode : EditorMode
{
    // virtual properties
    public override string ModeName => (isMoving) ? $"{Localization.Lookup(LocString.MODE_ADJUST)} {Localization.Lookup(LocString.ADJUST_SUBMODE_POSITION)}"
                                                  : $"{Localization.Lookup(LocString.MODE_ADJUST)} {Localization.Lookup(LocString.ADJUST_SUBMODE_SIZE)}";
    public override string HelpPath => "HelpFiles/AdjustMode";

    // Scene and asset references
    public TrackEditingMode EditingMode;

    [Header("UI Parents")]
    public GameObject SizingUI;
    public GameObject MovementUI;

    [Header("Sizing UI")]
    public HorizontalArrowGroup szHorizontalArrowGroup;
    public VerticalArrowGroup szVerticalArrowGroup;
    public TMP_Text StatusText;

    [Header("Movement UI")]
    public HorizontalArrowGroup mvHorizontalArrowGroup;
    public VerticalArrowGroup mvVerticalArrowGroup;

    // current mode
    private bool isMoving = false;

    // sizing mode
    private bool hasSizeChanged => initialSize != currentSize;
    private Vector2Int initialSize;
    private Vector2Int currentSize;

    // movement mode
    private bool anyModulesPlaced = false;
    private bool hasMoved => movementOffset.x != 0 || movementOffset.y != 0;
    private Vector2Int movementOffset = Vector2Int.zero;
    private Vector3 originalGridPosition;

    // both modes
    private Vector2Int minModulePos;
    private Vector2Int maxModulePos;
    private Vector2Int offsetMinModulePos => minModulePos + movementOffset;
    private Vector2Int offsetMaxModulePos => maxModulePos + movementOffset;

    // Others
    private void AdjustCamera()
    {
        var grid = TrackEditor.Instance.Grid;
        var gridCenter = grid.transform.position + new Vector3(grid.SquareSize * grid.NumHorizontalSquares * 0.5f,
                                                               0f,
                                                               grid.SquareSize * grid.NumVerticalSquares * 0.5f);

        var camera = TrackEditor.Instance.Camera;
        camera.transform.position = gridCenter + (Vector3.up * 100f);
        camera.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);

        camera.orthographic = true;
        camera.orthographicSize = grid.SquareSize * 30f; // about half screen size
    }

    private void OnGridSizeChanged()
    {
        var grid = TrackEditor.Instance.Grid;
        grid.NumHorizontalSquares = currentSize.x;
        grid.NumVerticalSquares = currentSize.y;
    }

    private void OnGridOffsetChanged()
    {
        var grid = TrackEditor.Instance.Grid;
        grid.transform.position = originalGridPosition + new Vector3(-movementOffset.x * grid.SquareSize, 0f, -movementOffset.y * grid.SquareSize);
    }

    private void RestoreGridSize()
    {
        var grid = TrackEditor.Instance.Grid;
        grid.NumHorizontalSquares = initialSize.x;
        grid.NumVerticalSquares = initialSize.y;
    }

    // Input
    private void UpdateMovingInput()
    {
        // if there's nothing placed, disable adjustment
        if (!anyModulesPlaced)
            return;

        // movement
        var movementDelta = Controls.DirectionalMovement.GetMovementDelta();
        if (movementDelta.sqrMagnitude > 0)
        {
            var newOffset = movementOffset + movementDelta;
            newOffset = Vector2Int.Max(newOffset, minModulePos * -1); //bounds check bottom/left
            newOffset = Vector2Int.Min(newOffset, currentSize - maxModulePos); //bounds check top/right

            if(newOffset.x != movementOffset.x || newOffset.y != movementOffset.y)
            {
                if (newOffset.y > movementOffset.y)
                    mvVerticalArrowGroup.FlashUp();
                else if (newOffset.y < movementOffset.y)
                    mvVerticalArrowGroup.FlashDown();

                if (newOffset.x > movementOffset.x)
                    mvHorizontalArrowGroup.FlashRight();
                else if (newOffset.x < movementOffset.x)
                    mvHorizontalArrowGroup.FlashLeft();

                movementOffset = newOffset;
                TrackEditor.PlaySound(TrackEditor.SndAdjust);
                OnGridOffsetChanged();
            }
        }
    }

    private void UpdateSizingInput()
    {
        //movement
        var movementDelta = Controls.DirectionalMovement.GetMovementDelta();
        if (movementDelta.sqrMagnitude > 0)
        {
            Vector2Int newSize = currentSize + movementDelta;
            newSize = Vector2Int.Max(newSize, offsetMaxModulePos); //bounds check top/right

            int minSize = (TrackEditor.UnlimitedMode) ? 3 : EditorConstants.MinGridSize;
            int maxSize = (TrackEditor.UnlimitedMode) ? ushort.MaxValue : EditorConstants.MaxGridSize;

            newSize.x = Mathf.Clamp(newSize.x, minSize, maxSize);
            newSize.y = Mathf.Clamp(newSize.y, minSize, maxSize);

            if (newSize.x != currentSize.x || newSize.y != currentSize.y)
            {
                if (newSize.x < currentSize.x)
                    szHorizontalArrowGroup.FlashLeft();
                else if (newSize.x > currentSize.x)
                    szHorizontalArrowGroup.FlashRight();

                if (newSize.y < currentSize.y)
                    szVerticalArrowGroup.FlashDown();
                else if (newSize.y > currentSize.y)
                    szVerticalArrowGroup.FlashUp();

                currentSize = newSize;
                TrackEditor.PlaySound(TrackEditor.SndAdjust);
                OnGridSizeChanged();
            }
        }
    }

    private void UpdateUI()
    {
        SizingUI.SetActive(!isMoving);
        MovementUI.SetActive(isMoving);

        // update sizing ui
        StatusText.text = string.Format(Localization.Lookup(LocString.ADJUST_WIDTH), currentSize.x) + "\n" +
                          string.Format(Localization.Lookup(LocString.ADJUST_HEIGHT), currentSize.y);
    }

    // Virtuals
    public override void Update()
    {
        base.Update();
        UpdateUI();
    }


    public override void UpdateInput()
    {
        // mode switch
        isMoving = Controls.AdjustmemtMode.MoveButtonHeld();
       
        // ui navigation
        if (Controls.UI.BackPressed())
        {
            // ask to confirm changes
            if (hasSizeChanged || hasMoved)
            {
                TrackEditor.Prompt.InitQuestionPrompt(LocString.PROMPT_KEEP_CHANGES,
                    () =>
                    {
                        EditingMode.CursorPosition += movementOffset;
                        TrackEditor.AdjustTrack(currentSize, movementOffset);
                        TrackEditor.Track.MarkDirty();
                        TrackEditor.Instance.SetTrackEditingMode();
                    },
                    () =>
                    {
                        RestoreGridSize();
                        TrackEditor.Instance.SetTrackEditingMode();
                    });

                TrackEditor.ShowPrompt();
            }
            else
            {
                TrackEditor.Instance.SetTrackEditingMode();
            }
        }

        // sub-mode inputs
        if (isMoving)
            UpdateMovingInput();
        else
            UpdateSizingInput();

        // shared
        base.UpdateInput();
    }

    public override void OnEnterMode()
    {
        base.OnEnterMode();
        UpdateUI();
        
        // setup camera
        AdjustCamera();

        // setup some vars
        var track = TrackEditor.Track;

        movementOffset = Vector2Int.zero;
        anyModulesPlaced = track.GetAllModuleRootPlacements().Count() > 0;

        initialSize = new Vector2Int(track.Width, track.Height);
        originalGridPosition = TrackEditor.Instance.Grid.transform.position;
        currentSize = initialSize;

        // calculate bounds of the placed modules
        minModulePos = new Vector2Int(9999, 9999);
        maxModulePos = new Vector2Int(-9999, -9999);

        foreach (var cell in track.Cells.Where(x => x.Module != null))
        {
            maxModulePos = Vector2Int.Max(cell.Position + Vector2Int.one, maxModulePos);
            minModulePos = Vector2Int.Min(cell.Position, minModulePos);
        }
    }

    public override void OnExitMode()
    {
        base.OnExitMode();

        // put the grid  back
        TrackEditor.Instance.Grid.transform.position = originalGridPosition;
    }

    // Mono
    private void LateUpdate()
    {
        AdjustCamera();
    }
}
