using System;
using System.Collections.Generic;
using System.IO;

public static class SavedFilesList
{
    private static readonly List<SavedFile> savedFiles = new List<SavedFile>();
    public static IReadOnlyList<SavedFile> SavedFiles => savedFiles;

    public struct SavedFile
    {
        public string Name;
        public string Path;
        public DateTime ModifiedDate;

        public SavedFile(string path)
        {
            var fi = new FileInfo(path);
            ModifiedDate = fi.LastWriteTimeUtc;
            Path = path;

            using(var br = new BinaryReader(File.OpenRead(path)))
            {
                var track = new EditorTrack();
                track.ReadBinaryHeader(br);
                Name = track.Name;
            }
        }
    }

    public static void Update()
    {
        HashSet<string> savedFilePaths = new HashSet<string>(FileHelper.GetSavedTrackFileList());
        bool sortList = false;

        // first, update and remove saved files
        for(int i=savedFiles.Count - 1; i >= 0; i--)
        {
            var savedFile = savedFiles[i];
            if (!savedFilePaths.Contains(savedFile.Path))
            {
                // saved file no longer exists
                savedFiles.RemoveAt(i);
            }
            else
            {
                // saved file still exists, update it if it's been modified
                var fileInfo = new FileInfo(savedFile.Path);
                if(fileInfo.LastWriteTimeUtc >= savedFile.ModifiedDate)
                {
                    savedFiles[i] = new SavedFile(savedFile.Path);
                    sortList |= savedFiles[i].Name != savedFile.Name;
                }
            }
            savedFilePaths.Remove(savedFile.Path);
        }

        // now add new ones
        sortList |= savedFilePaths.Count > 0;
        foreach (string newPath in savedFilePaths)
        {
            savedFiles.Add(new SavedFile(newPath));
        }

        // finally, sort if requested
        if (sortList)
        {
            savedFiles.Sort((s1, s2) => s1.Name.CompareTo(s2.Name));
        }
    }
}
