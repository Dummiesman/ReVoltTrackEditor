using ReVolt.TrackUnit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackExporter
{
    // Forwarded from compiler. These use the forward exporter since it's the primary focus.
    public Vector2Int LastAICell => trackCompilerForward.LastAICell;
    public IReadOnlyList<TrackZone> Zones => trackCompilerForward.Zones;
    public IReadOnlyList<ZoneSequenceEntry> ZoneSequence => trackCompilerForward.ZoneSequence;

    // Validity checks. These again use the forward exporter since it's the primary focus.
    public bool TrackFormsLoop => trackCompilerForward.TrackFormsLoop;
    public bool AIIsValid => trackCompilerForward.AIIsValid;
    public bool LastZoneIsPipe => trackCompilerForward.LastZoneIsPipe;

    protected TrackCompiler trackCompilerForward;
    protected TrackCompiler trackCompilerReverse;
    protected bool isReversePossible { get; private set; }

    protected readonly string trackExportName;

    public int GetStartGridCount()
    {
        return trackCompilerForward.GetStartGridCount();
    }

    public virtual void Initialize()
    {
        trackCompilerForward.Initialize();
        trackCompilerReverse.Initialize();
        isReversePossible = trackCompilerReverse.TrackFormsLoop;
    }

    public void CompileAI()
    {
        trackCompilerForward.CompileAI();
        trackCompilerReverse.CompileAI();
        if (!trackCompilerReverse.AIIsValid)
            isReversePossible = false;
    }

    public void CompileCollision()
    {
        trackCompilerForward.CompileCollision();
    }

    public void CompileWorld()
    {
        trackCompilerForward.CompileWorld();
    }

    public void CompileSmallFiles()
    {
        trackCompilerForward.CompileLights();
        trackCompilerForward.CompileObjects();
    }

    public virtual void WriteFiles()
    {

    }

    public TrackExporter(EditorTrack track, TrackUnitFile unitFile, float scale = 1f)
    {
        this.trackExportName = FileHelper.TrackNameToExportName(track.Name);
        this.trackCompilerForward = new TrackCompiler(track, unitFile, false, scale);
        this.trackCompilerReverse = new TrackCompiler(track, unitFile, true, scale);
    }
}
