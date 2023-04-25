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

using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExportMode : EditorMode
{
    // Virtual properties
    public override string ModeName => Localization.Lookup(LocString.MODE_EXPORT);

    // Scene and asset references
    public Image ClockImage;
    public Image[] ClockProgressImages;

    public TMP_Text ExportStageText;

    public Sprite ProgressCircleFilled;
    public Sprite ProgressCircle;
    public Sprite CompletedClockSprite;
    public Sprite ClockSprite;

    public Sprite JumpyClockLeft;
    public Sprite JumpyClockRight;

    public TrackEditingMode EditingMode;

    // 
    private AudioSource clockLoopSource;
    private bool askedTrackScale = false;
    private float exportScale = 1f;

    // Other
    private void AskTrackScale()
    {
        TrackEditor.Prompt.InitQuestionPrompt(LocString.PROMPT_EXPORT_CHOOSE_SCALE, 1,
            () =>
            {
                TrackEditor.SetEditMode<TrackEditingMode>();
            },
            new PromptMode.PromptItem(LocString.ANSWER_HALF, () =>
            {
                askedTrackScale = true;
                exportScale = 0.5f;
                TrackEditor.SetEditMode<ExportMode>();
            }),
            new PromptMode.PromptItem(LocString.ANSWER_REGULAR, () =>
            {
                askedTrackScale = true;
                exportScale = 1f;
                TrackEditor.SetEditMode<ExportMode>();
            }),
            new PromptMode.PromptItem(LocString.ANSWER_DOUBLE, () =>
            {
                askedTrackScale = true;
                exportScale = 2f;
                TrackEditor.SetEditMode<ExportMode>();
            }));
        TrackEditor.ShowPrompt();
    }

    private void ShowProgressImages()
    {
        foreach (var image in ClockProgressImages)
            image.enabled = true;
    }

    private void HideProgressImages()
    {
        foreach (var image in ClockProgressImages)
            image.enabled = false;
    }

    private void SetExportProgress(float progress)
    {
        int maxImage = Mathf.RoundToInt(ClockProgressImages.Length * Mathf.Clamp01(progress));
        for(int i=0; i < ClockProgressImages.Length; i++)
            ClockProgressImages[i].sprite = (i < maxImage) ? ProgressCircleFilled : ProgressCircle;

        int stageNumber = Mathf.RoundToInt(12 * Mathf.Clamp01(progress));
        ExportStageText.text = string.Format(Localization.Lookup(LocString.EXPORT_STAGE_NUM), stageNumber);
    }

    void ContinuityError(TrackExporter exporter)
    {
        // set cursor pos
        if (!exporter.TrackFormsLoop)
        {
            //set cursor pos to last zone
            EditingMode.SetActiveModule((int)Modules.ID.TWM_START);
            EditingMode.CursorPosition = exporter.Zones[exporter.ZoneSequence.Last().ZoneID].CellCoordinate;
        }
        else if (!exporter.AIIsValid)
        {
            //set cursor pos to last pos
            EditingMode.SetActiveModule((int)Modules.ID.TWM_START);
            EditingMode.CursorPosition = exporter.LastAICell;
        }

        // throw an error
        LocString errorMesage;
        if (exporter.ZoneSequence.Count > 0)
        {
            errorMesage = (exporter.LastZoneIsPipe) ? LocString.PROMPT_NO_LOOP_FORMED_PIPE_C
                                                    : LocString.PROMPT_NO_LOOP_FORMED_STEP_C;
        }
        else
        {
            errorMesage = (exporter.LastZoneIsPipe) ? LocString.PROMPT_NO_LOOP_FORMED_PIPE
                                                    : LocString.PROMPT_NO_LOOP_FORMED_STEP;
        }

        TrackEditor.Prompt.InitWarningPrompt(errorMesage, TrackEditor.Instance.SetTrackEditingMode);
        TrackEditor.ShowPrompt();
    }

    IEnumerator ExportRoutine()
    {
        var perfLogger = new PerfTimeLogger("Export:Total");
        ClockImage.sprite = ClockSprite;
        ShowProgressImages();
        SetExportProgress(0f);

        // If there's no compiled levels folder, exit right here and now
        if(FileHelper.GetCompiledLevelsPath() == null)
        {
            TrackEditor.Prompt.InitWarningPrompt(LocString.PROMPT_NO_LEVELS_DIR, TrackEditor.Instance.SetTrackEditingMode);
            TrackEditor.ShowPrompt();
            yield break;
        }

        // Otherwise, keep going
        var exporter = new TrackExporter(TrackEditor.Track, TrackEditor.TrackUnit, exportScale);
        exporter.Initialize();
        yield return null;

        int startGridCount = exporter.GetStartGridCount();
        if(startGridCount != 1)
        {
            LocString errorMesage = (startGridCount == 0) ? LocString.PROMPT_MISSING_START_GRID
                                                          : LocString.PROMPT_MULTIPLE_START_GRIDS;
            TrackEditor.Prompt.InitWarningPrompt(errorMesage, TrackEditor.Instance.SetTrackEditingMode);
            TrackEditor.ShowPrompt();
            yield break;
        }

        if (!exporter.TrackFormsLoop)
        {
            ContinuityError(exporter);
            yield break;
        }
        
        SetExportProgress(0.1f);
        yield return null;

        // Create AI, check validity
        exporter.CreateAINodes();
        if (!exporter.AIIsValid)
        {
            ContinuityError(exporter);
            yield break;
        }

        // Track is valid
        // Create reversed exporter
        var reverseExporter = new TrackExporter(TrackEditor.Track, TrackEditor.TrackUnit, true, exportScale);
        reverseExporter.Initialize();
        reverseExporter.CreateAINodes();

        if (!reverseExporter.TrackFormsLoop || !reverseExporter.AIIsValid)
            reverseExporter = null; // can't export reverse, set to null

        // Create and empty destination folder
        exporter.CreateTrackFolder();
        exporter.EmptyTrackFolderContents();
        reverseExporter?.CreateTrackFolder();

        // Write the AI nodes
        exporter.WriteAINodes();
        reverseExporter?.WriteAINodes();

        SetExportProgress(0.2f);
        yield return null;

        // Create world and collision file
        var worldTask = Task.Run(() =>
        {
            exporter.CreateWorld();
            exporter.WriteWorldFile();
        });
        var collisionTask = Task.Run(() =>
        {
            exporter.CreateCollision();
            exporter.WriteCollision();
        });

        while(!worldTask.IsCompleted || !collisionTask.IsCompleted)
        {
            //0.2-0.5
            float progress = 0.2f + (worldTask.IsCompleted ? 0.15f : 0f) + (collisionTask.IsCompleted ? 0.15f : 0f);
            SetExportProgress(progress);
            yield return null;
        }

        // Save zones, pos nodes, etc
        exporter.WritePosNodesFile();
        exporter.WriteZonesFile();

        reverseExporter?.WritePosNodesFile();
        reverseExporter?.WriteZonesFile();

        exporter.WriteLightsFile();
        exporter.WriteObjectsFile();

        SetExportProgress(0.7f);
        yield return null;

        // Finally, copy textures and write info file
        exporter.CopyBitmaps();
        exporter.WriteInfoFile();

        perfLogger.Log("Total");
        SetExportProgress(1f);
        yield return null;

        // We're finished, ring the clock!
        ExportStageText.text = Localization.Lookup(LocString.EXPORT_STAGE_FINISHED);
        HideProgressImages();
        clockLoopSource.Stop();

        TrackEditor.PlaySound(TrackEditor.SndExportComplete);
        double ringTimeEnd = Time.timeSinceLevelLoadAsDouble + (TrackEditor.SndExportComplete.length * 0.9f);
        
        while(Time.timeSinceLevelLoadAsDouble < ringTimeEnd)
        {
            bool clockFrame = ((Time.timeSinceLevelLoad * 12f) % 1f) > 0.5f;
            ClockImage.sprite = (clockFrame) ? JumpyClockRight : JumpyClockLeft;
            yield return null;
        }
        
        //finally, wait a second before heading back to the editor mode
        ClockImage.sprite = CompletedClockSprite;
        yield return new WaitForSeconds(1f);

        TrackEditor.Instance.SetTrackEditingMode();
    }

    // Virtuals
    public override void Init()
    {
        base.Init();

        clockLoopSource = GetComponent<AudioSource>();
        clockLoopSource.clip = FileHelper.LoadSound("Ticktock")
                            ?? FileHelper.LoadSound("ticktock");
    }

    public override void OnEnterMode()
    {
        base.OnEnterMode();
        TrackEditor.Instance.Camera.enabled = false;

        if (TrackEditor.ExportScaleOverride.HasValue)
        {
            // user has sepecified a custom scale
            exportScale = TrackEditor.ExportScaleOverride.Value;
        }
        else if (!askedTrackScale)
        {
            // ask for export scale, then re-enter export mode
            AskTrackScale();
            return;
        }

        clockLoopSource.Play();
        askedTrackScale = false;
        StartCoroutine(ExportRoutine());
    }

    public override void OnExitMode()
    {
        base.OnExitMode();
        TrackEditor.Instance.Camera.enabled = true;

        clockLoopSource.Stop();
        StopAllCoroutines();
    }
}
