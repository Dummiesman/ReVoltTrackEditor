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

using ReVolt.TrackUnit;
using System.IO;
using System.Linq;
using UnityEngine;

public static class FileHelper 
{
    const string saveFilePrefix = "!";

    static string rootPath = null;
    static string gamePath = null;

    static string soundsPath => Path.Combine(rootPath, "sounds");
    static string tdfPath => Path.Combine(rootPath, "TDF");
    static string stringsPath => Path.Combine(rootPath, "strings");
    static string profilesPath => (gamePath != null) ? Path.Combine(gamePath, "profiles") : null;
    static string levelsPath => (gamePath != null) ? Path.Combine(gamePath, "levels") : null;


    public static TrackUnitFile LoadTrackUnit()
    {
        string fullPath = Path.Combine(rootPath, "trackunit.rtu");
        if (File.Exists(fullPath))
        {
            using (var stream = File.OpenRead(fullPath))
            {
                return new TrackUnitFile(stream);
            }
        }
        else
        {
            return null;
        }
    }

    public static AudioClip LoadSound(string sound)
    {
        string soundPath = Path.Combine(soundsPath, $"{sound}.wav");
        if (File.Exists(soundPath))
        {
            return Dummiesman.Wave.WAVLoader.Load(soundPath);
        }
        else
        {
            return null;
        }
    }

    public static Texture2D LoadTPage(int num)
    {
        var image = LoadImage($"tpage_{num:D2}");
        image.wrapMode = TextureWrapMode.Clamp;
        return image;
    }

    public static Texture2D LoadImage(string image)
    {
        string fullPathLower = Path.Combine(rootPath, $"{image}.bmp");
        string fullPathUpper = Path.Combine(rootPath, $"{image}.BMP");
        if (File.Exists(fullPathLower))
            return new B83.Image.BMP.BMPLoader().LoadBMP(fullPathLower).ToTexture2D();
        else if (File.Exists(fullPathUpper))
            return new B83.Image.BMP.BMPLoader().LoadBMP(fullPathUpper).ToTexture2D();
        else
            return Texture2D.whiteTexture;
    }

    public static string GetTPagePath(int num)
    {
        return Path.Combine(rootPath, $"tpage_{num:D2}.bmp");
    }

    private static string[] GetAvailableLanguages()
    {
        if (!Directory.Exists(stringsPath))
            return new string[] { };

        string[] filePaths = Directory.GetFiles(stringsPath, "*.txt", SearchOption.TopDirectoryOnly);
        string[] languageNames = new string[filePaths.Length];

        for(int i=0; i < filePaths.Length; i++)
        {
            languageNames[i] = Path.GetFileNameWithoutExtension(filePaths[i]);
        }

        return languageNames;
    }

    private static string GetUserLanguage()
    {
        string language = "english";

        if(profilesPath != null)
        {
            string rvglSettingsPath = Path.Combine(profilesPath, "rvgl.ini");
            if (File.Exists(rvglSettingsPath))
            {
                var rvglSettingsFile = new IniFile(File.ReadAllText(rvglSettingsPath));
                string defaultProfileName = rvglSettingsFile.GetString("Misc", "DefaultProfile", null);

                if (!string.IsNullOrEmpty(defaultProfileName))
                {
                    defaultProfileName = defaultProfileName.Substring(1, defaultProfileName.Length - 2); //remove surrounding quotes
                    string userProfileDirPath = Path.Combine(profilesPath, defaultProfileName);
                    string userProfileSettingsPath = Path.Combine(userProfileDirPath, "profile.ini");

                    if (File.Exists(userProfileSettingsPath))
                    {
                        var userSettingsFile = new IniFile(File.ReadAllText(userProfileSettingsPath));
                        string userLanguage = userSettingsFile.GetString("Game", "Language", null);
                        if (!string.IsNullOrEmpty(userLanguage))
                        {
                            userLanguage = userLanguage.Substring(1, userLanguage.Length - 2); //remove surrounding quotes
                            language = userLanguage;
                            Debug.Log($"GetUserLanguage: Got {userLanguage} from profile {defaultProfileName}");
                        }
                    }
                }
            }
        }

        return language;
    }

    public static void LoadStrings()
    {
        var availableLanguages = GetAvailableLanguages();
        string languageName = GetUserLanguage();

        if(availableLanguages.Length == 0)
        {
            Debug.LogWarning($"LoadStrings: language '{languageName}' not available, no language installed to fall back to.");
            return;
        }
        else if (!availableLanguages.Contains(languageName))
        {
            Debug.LogWarning($"LoadStrings: language '{languageName}' not available, falling back to 'english'");
            languageName = "english";
        }

        string languagePath = Path.Combine(stringsPath, $"{languageName}.txt");
        if(!Localization.Init(languagePath))
        {
            Debug.LogError($"LoadStrings: failed to load strings file file {languagePath}.");
        }
    }

    public static string[] GetSavedTrackFileList()
    {
        if (!Directory.Exists(tdfPath))
            return new string[0];
        return Directory.GetFiles(tdfPath, "*.TDF", SearchOption.TopDirectoryOnly);
    }

    public static string GetTrackSavePath(string trackName)
    {
        string sanitized = TrackNameToSaveName(trackName);
        return Path.Combine(tdfPath, sanitized + ".TDF");
    }

    public static string GetFirstAvailableDefaultTrackFilename()
    {
        return GetFirstAvailableDefaultTrackName() + ".TDF";
    }

    public static string GetFirstAvailableDefaultTrackName()
    {
        for (int i=0; i < 1000; i++)
        {
            string saveName =  string.Format(Localization.Lookup(LocString.SAVE_NAME_TEMPLATE), i);
            string saveFileName = TrackNameToSaveName(saveName) + ".TDF";
            string saveFullPath = Path.Combine(tdfPath, saveFileName);
            if (!File.Exists(saveFullPath))
            {
                return saveName;
            }
        }
        return "YouAreWinner"; // wow, you have 1000 tracks saved?! (or a missing language file)
    }

    public static string SanitizeFilename(string filename)
    {
        return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
    }

    public static string TrackNameToSaveName(string trackName)
    {
        return saveFilePrefix + SanitizeFilename(trackName);
    }

    public static string TrackNameToExportDirectory(string trackName)
    {
        return $"USER_{trackName.GetHashCode():x}";
    }

    public static void EnsureSaveDirectoryExists()
    {
        Directory.CreateDirectory(tdfPath);
    }

    public static string GetCompiledLevelsPath()
    {
        return levelsPath;
    }


    public static void Initialize()
    {
        if ((Application.platform == RuntimePlatform.WindowsPlayer 
           || Application.platform == RuntimePlatform.LinuxPlayer
           || Application.platform == RuntimePlatform.OSXPlayer) || ApplicationHelper.isEditor)
        {
            // our files exist alongside the executable on other platforms
#if UNITY_EDITOR
            if (!File.Exists("editor_path.txt"))
            {
                Debug.LogError($"Please create editor_path.txt in the project root.");
                UnityEditor.EditorApplication.isPlaying = false;
                return;
            }
            rootPath = File.ReadAllText("editor_path.txt");
#else
            rootPath = System.Environment.CurrentDirectory;
#endif

            // look for Re-Volt in the parent folder
            var rootDirInfo = new DirectoryInfo(rootPath);
            if (rootDirInfo.Parent != null)
            {
                if (Directory.Exists(Path.Combine(rootDirInfo.Parent.FullName, "levels")) &&
                    Directory.Exists(Path.Combine(rootDirInfo.Parent.FullName, "editor")))
                {
                    gamePath = rootDirInfo.Parent.FullName;
                }
            }
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            // our files exist in the Android data folder
            rootPath = Application.persistentDataPath;

            // look for an RVGL installation
            string[] rvglAndroidPaths = new[]{"/sdcard/RVGL", "/storage/sdcard0/RVGL",
                                              "/storage/sdcard1/RVGL", "/storage/emulated/legacy/RVGL",
                                              "/storage/emulated/0/RVGL", "/storage/emulated/1/RVGL",
                                              "/storage/extSdCard/RVGL"};
            for (int i = 0; i < rvglAndroidPaths.Length; i++)
            {
                if (Directory.Exists(rvglAndroidPaths[i]))
                {
                    gamePath = rvglAndroidPaths[i];
                    break;
                }
            }
        }
        else
        {
            Debug.LogError($"Unsupported platform {Application.platform}?");
        }

        // output paths to log
        Debug.Log($"FileHelper.Initialize");
        Debug.Log($"GamePath: {gamePath}");
        Debug.Log($"RootPath: {rootPath}");

        // create track save dir
        Directory.CreateDirectory(tdfPath);
    }
}
