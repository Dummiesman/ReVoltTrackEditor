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

using Dummiesman.DebugDraw;
using System.Linq;
using TMPro;
using UnityEngine;

public class PickupEditingMode : EditorMode
{
    // virtual properties
    public override string ModeName => Localization.Lookup(LocString.MODE_PICKUPS);
    public override string HelpPath => "HelpFiles/PickupEditingMode";

    // Scene and asset references
    public TrackEditingMode EditingMode;
    public TMP_Text PickupPlacedText;
    public Texture2D PickupTexture;

    //
    private Vector2Int cursorPosition = Vector2Int.zero;
    private bool[] originalPickupState;

    private int numPickups = 0;
    private bool modified = false;

    private const float CURSOR_DRAW_HEIGHT = 50f;
    private const int MAX_PICKUPS = 40;

    // Placement
    private bool PlacePickup()
    {
        var cell = TrackEditor.Track.GetCell(cursorPosition);
        if (cell.Module == null)
            return false;

        // have we placed the maximum amount?
        if (!cell.HasPickup && numPickups >= MAX_PICKUPS && !TrackEditor.UnlimitedMode)
            return false;

        if (cell.HasPickup)
            numPickups--;
        else
            numPickups++;

        modified = true;
        cell.HasPickup = !cell.HasPickup;

        return true;
    }

    // Others
    private void DrawPickup(Vector2Int gridPosition)
    {
        DebugDraw.BeginTexturedTransparent(DrawType.QUADS, PickupTexture);
        DebugDraw.Color = Color.white;

        int uvFrame = Mathf.FloorToInt((float)(Time.timeSinceLevelLoadAsDouble % 1d) * 8f);
        Rect uvRect = new Rect(uvFrame * 0.0625f, 0f, 0.0625f, 0.0625f);

        Vector3 center = new Vector3(gridPosition.x, CURSOR_DRAW_HEIGHT + 1f, gridPosition.y);

        DebugDraw.Vertex(center + new Vector3(-0.5f, 0f, -0.5f), new Vector2(uvRect.x, uvRect.y));
        DebugDraw.Vertex(center + new Vector3(-0.5f, 0f, 0.5f), new Vector2(uvRect.x, uvRect.yMax));
        DebugDraw.Vertex(center + new Vector3(0.5f, 0f, 0.5f), new Vector2(uvRect.xMax, uvRect.yMax));
        DebugDraw.Vertex(center + new Vector3(0.5f, 0f, -0.5f), new Vector2(uvRect.xMax, uvRect.y));

        DebugDraw.End();
    }

    private void Draw()
    {
        DebugDraw.PushMatrix();
        DebugDraw.Identity();

        // draw cursor
        var grid = TrackEditor.Instance.Grid;
        float gridHSize = grid.SquareSize * grid.NumHorizontalSquares;
        float gridVSize = grid.SquareSize * grid.NumVerticalSquares;
        var gridOrigin = grid.transform.position;

        Vector3 halfUnitRight = Vector3.right * 0.5f;
        Vector3 halfUnitUp = Vector3.forward * 0.5f;

        DebugDraw.Color = Color.white;
        DebugDraw.DrawLine(new Vector3(gridOrigin.x + halfUnitRight.x + cursorPosition.x, CURSOR_DRAW_HEIGHT, gridOrigin.z - halfUnitUp.z),
                           new Vector3(gridOrigin.x + halfUnitRight.x + cursorPosition.x, CURSOR_DRAW_HEIGHT, gridOrigin.z + halfUnitUp.z + gridVSize));

        DebugDraw.DrawLine(new Vector3(gridOrigin.x - halfUnitRight.x, CURSOR_DRAW_HEIGHT, gridOrigin.z + halfUnitUp.z + cursorPosition.y),
                           new Vector3(gridOrigin.x + halfUnitRight.x + gridHSize, CURSOR_DRAW_HEIGHT, gridOrigin.z + halfUnitUp.z + cursorPosition.y));

        // draw Pickups
        foreach (var cell in TrackEditor.Track.Cells)
        {
            if (cell.HasPickup)
                DrawPickup(cell.Position);
        }

        DebugDraw.PopMatrix();
    }

    private void RevertChanges()
    {
        var track = TrackEditor.Track;
        for (int i = 0; i < track.Cells.Length; i++)
            track.Cells[i].HasPickup = originalPickupState[i];
    }

    // Virtuals
    public override void Update()
    {
        base.Update();
        Draw();

        // update ui text
        if (TrackEditor.UnlimitedMode)
            PickupPlacedText.text = string.Format(Localization.Lookup(LocString.PICKUPS_PLACED), numPickups, "∞");
        else
            PickupPlacedText.text = string.Format(Localization.Lookup(LocString.PICKUPS_PLACED), numPickups, MAX_PICKUPS);
    }


    public override void UpdateInput()
    {
        // movement
        var movementDelta = Controls.DirectionalMovement.GetMovementDelta();
        if(movementDelta.sqrMagnitude > 0)
        {
            TrackEditor.PlaySound(TrackEditor.SndMenuMove);
            cursorPosition += movementDelta;

            cursorPosition.x = Mathf.Clamp(cursorPosition.x, 0, TrackEditor.Track.Width - 1);
            cursorPosition.y = Mathf.Clamp(cursorPosition.y, 0, TrackEditor.Track.Height - 1);
        }                                

        // placement
        if(Controls.PickupEdit.PlacePressed())
        {
            if (PlacePickup())
                TrackEditor.PlaySound(TrackEditor.SndPlacePickup);
            else
                TrackEditor.PlaySound(TrackEditor.SndWarning);
        }

        // ui navigation
        if (Controls.UI.BackPressed())
        {
            if (modified)
            {
                TrackEditor.Prompt.InitQuestionPrompt(LocString.PROMPT_KEEP_CHANGES,
                                                      () => { TrackEditor.Track.MarkDirty(); TrackEditor.Instance.SetTrackEditingMode(); },
                                                      () => { RevertChanges(); TrackEditor.Instance.SetTrackEditingMode(); });
                TrackEditor.ShowPrompt();
            }
            else
            {
                TrackEditor.Instance.SetTrackEditingMode();
            }
        }

        // shared
        base.UpdateInput();
    }

    public override void OnEnterMode()
    {
        base.OnEnterMode();

        // setup initial cursor pos
        cursorPosition = EditingMode.CursorPosition;

        // setup camera
        var grid = TrackEditor.Instance.Grid;
        var gridCenter = grid.transform.position + new Vector3(grid.SquareSize * grid.NumHorizontalSquares * 0.5f,
                                                               0f,
                                                               grid.SquareSize * grid.NumVerticalSquares *  0.5f);

        var camera = TrackEditor.Instance.Camera;
        camera.orthographic = true;

        camera.transform.position = gridCenter + (Vector3.up * 100f);
        camera.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);
        camera.orthographicSize = grid.SquareSize * grid.NumVerticalSquares; // about half screen size

        // calculate current numPickups, store original pickup state
        var track = TrackEditor.Track;
        numPickups = track.Cells.Count(x =>x .HasPickup);
        originalPickupState = track.Cells.Select(x => x.HasPickup).ToArray();
        modified = false;
    }
}
