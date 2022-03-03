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
using System.Text.RegularExpressions;

// !!!!!!!
// Changing any of the names or IDs in this enumeration
// may break localized UI elements, edit with caution
// !!!!!!!

public enum LocString
{
    //Mode Names
    MODE_PLACEMODULES = 8,
    MODE_MODULES = 0,
    MODE_PICKUPS = 1,
    MODE_ADJUST = 2,
    MODE_EXPORT = 6,
    MODE_MENU = 10,

    //Prompt
    PROMPT_WARNING = 11,
    PROMPT_QUESTION,
    PROMPT_INFO,

    ANSWER_YES,
    ANSWER_NO,

    ANSWER_HALF = 26,
    ANSWER_REGULAR,
    ANSWER_DOUBLE,

    //Prompts
    PROMPT_EXPORT_CHOOSE_SCALE = 25,

    PROMPT_TRACK_NOT_SAVED = 29,
    PROMPT_TRACK_SAVE_FAILED,
    PROMPT_OVERWRITE_TRACK,
    PROMPT_REALLY_DELETE,

    PROMPT_KEEP_CHANGES,

    PROMPT_TRACK_SAVED,
    PROMPT_ERROR_SAVING_TRACK,

    PROMPT_NO_LEVELS_DIR,

    PROMPT_NO_LOOP_FORMED_PIPE_C,
    PROMPT_NO_LOOP_FORMED_STEP_C,
    PROMPT_NO_LOOP_FORMED_PIPE,
    PROMPT_NO_LOOP_FORMED_STEP,

    PROMPT_TRACK_TOO_COMPLEX,

    PROMPT_MISSING_START_GRID,
    PROMPT_MULTIPLE_START_GRIDS,

    //Pickup Placement Mode
    PICKUPS_PLACED = 20,

    //Adjust Mode
    ADJUST_SUBMODE_POSITION = 21,
    ADJUST_SUBMODE_SIZE,
    ADJUST_WIDTH,
    ADJUST_HEIGHT,

    //Generic Menu
    MENU_SAVE = 3,
    MENU_NEW = 4,
    MENU_LOAD = 5,
    MENU_QUIT = 7,

    //Exporting
    EXPORT_STAGE_NUM = 16,
    EXPORT_STAGE_FINISHED,

    //Save Menu
    SAVE_SAVE = 18,
    SAVE_NAME_TEMPLATE,

    //Load Menu
    LOAD_NO_TRACKS_EXIST = 44,
    
    //Modules
    MODULE_STARTGRID = 45,
    MODULE_STRAIGHTS,
    MODULE_DIPS,
    MODULE_HUMPS,
    MODULE_SQUARE_BEND,
    MODULE_ROUND_BEND,
    MODULE_DIAGONAL,
    MODULE_BANK,
    MODULE_RUMBLE,
    MODULE_NARROW,
    MODULE_PIPE,
    MODULE_BRIDGE,
    MODULE_CROSSROAD,
    MODULE_JUMP,
    MODULE_CHICANE
}

public class Localization
{
    private const int EXPECTED_STRING_COUNT = 60;
    private readonly static Dictionary<LocString, string> loaded = new Dictionary<LocString, string>();

    private static string ProcessLine(string line)
    {
        // replace textual escape sequences
        string processed = line.Replace("\\n", "\n").Replace("\\r", "\r").Replace("\\\"", "\"");

        // discard numeric escape sequences, seems to have special meaning to Re-Volt but not to us
        if (line.StartsWith("\\"))
        {
            int escapeLen = 1;
            for(int i=1; i < line.Length; i++)
            {
                if (!char.IsNumber(line[i]))
                {
                    escapeLen = i;
                    break;
                }
            }
            processed = processed.Remove(0, escapeLen);
        }

        // replace %d %s etc with C# style formatting groups
        int g = -1;
        processed = Regex.Replace(processed, "%.", m => ("{" + ++g + "}"));

        return processed;
    }

    public static string Lookup(LocString id)
    {
        if(!loaded.TryGetValue(id, out string str))
        {
            str = id.ToString();
        }
        return str;
    }

    public static string[] LookupRange(LocString idStart, LocString idEnd)
    {
        int start = (int)idStart;
        int end = (int)idEnd;
        int count = (end - start) + 1;
        
        string[] returnValues = new string[count];
        for (int i = 0; i < count; i++)
            returnValues[i] = Lookup((LocString)(i + start));
        
        return returnValues;
    }

    public static bool Init(string languageFilePath)
    {
        loaded.Clear();

        // file exists?
        if (!System.IO.File.Exists(languageFilePath))
            return false;

        // load lines
        List<string> lines = new List<string>(System.IO.File.ReadAllLines(languageFilePath));
        lines.RemoveAll(x => x.StartsWith(@"\V") && !x.StartsWith(@"\VP")); // remove strings from other game versions

        for (int i = 0; i < lines.Count; i++)
        {
            loaded[(LocString)i] = ProcessLine(lines[i]);
        }

        // validate
        return loaded.Count == EXPECTED_STRING_COUNT;
    }
}
