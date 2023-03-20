using ReVolt.TrackUnit;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DirectoryTrackExporter : TrackExporter
{
    private readonly string exportPath;
    private readonly string exportPathReversed;

    public void CreateTrackFolder()
    {
        Directory.CreateDirectory(exportPath);
    }

    public void EmptyTrackFolderContents()
    {
        var di = new DirectoryInfo(exportPath);
        if (di.Exists)
        {
            foreach (FileInfo file in di.EnumerateFiles())
                file.Delete();
            foreach (DirectoryInfo dir in di.EnumerateDirectories())
                dir.Delete(true);
        }
    }

    public void CopyBitmaps()
    {
        for (int i = 1; i <= RVConstants.TPAGE_COUNT; i++)
        {
            using (var inStream = FileHelper.GetTPageStream(i))
            {
                if (inStream != null)
                {
                    string targetFileName = Path.Combine(exportPath, $"{trackExportName}{(char)('a' + (i - 1))}.bmp");
                    var outStream = File.OpenWrite(targetFileName);
                    inStream.CopyTo(outStream);
                }
            }
        }
    }

    public override void WriteFiles()
    {
        void saveBothDirections<T>(string extension, T data, T reverseData) where T : ISaveLoad
        {
            string filePath = Path.Combine(exportPath, $"{trackExportName}.{extension}");
            data.Save(filePath);

            if (isReversePossible)
            {
                string filePathRev = Path.Combine(exportPathReversed, $"{trackExportName}.{extension}");
                reverseData.Save(filePathRev);
            }
        }

        // info
        string infoFilePath = Path.Combine(exportPath, $"{trackExportName}.inf");
        File.WriteAllText(infoFilePath, trackCompilerForward.CreateInfoFile());

        // pos nodes
        saveBothDirections("pan", trackCompilerForward.CompiledPOSNodes, trackCompilerReverse.CompiledPOSNodes);

        // ai
        saveBothDirections("fan", trackCompilerForward.CompiledAINodes, trackCompilerReverse.CompiledAINodes);

        // zones
        saveBothDirections("taz", trackCompilerForward.CompiledZones, trackCompilerReverse.CompiledZones);

        // objects
        string objectsFilePath = Path.Combine(exportPath, $"{trackExportName}.fob");
        trackCompilerForward.CompiledObjects.Save(objectsFilePath);

        // lights
        string lightFilePath = Path.Combine(exportPath, $"{trackExportName}.lit");
        trackCompilerForward.CompiledLights.Save(lightFilePath);

        // collision
        string collisionFilePath = Path.Combine(exportPath, $"{trackExportName}.ncp");
        trackCompilerForward.CompiledCollision.Save(collisionFilePath);

        // world
        string worldFilePath = Path.Combine(exportPath, $"{trackExportName}.w");
        trackCompilerForward.CompiledWorld.Save(worldFilePath);
    }
    
    public DirectoryTrackExporter(EditorTrack track, TrackUnitFile unitFile, float scale = 1) : base(track, unitFile, scale)
    {
        this.exportPath = Path.Combine(FileHelper.GetCompiledLevelsPath(), trackExportName);
        this.exportPathReversed = Path.Combine(FileHelper.GetCompiledLevelsPath(), trackExportName, "reversed");
    }
}
