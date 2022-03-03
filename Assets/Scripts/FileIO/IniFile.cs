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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

public class IniFile : IEnumerable
{
    // public things
    public bool AllowInheritance { get; set; } = true;
    public bool RenameDuplicateSections { get; set; } = false;
    public bool StripValueComments { get; set; } = true;
    public bool TrimKeyValues { get; set; } = true;
    public bool AllowEmptyValues { get; set; } = true;
    public char[] CommentChars = { '#', ';' };
    public char SetterChar = '=';

    // private props
    private Dictionary<string, IniSection> _sections = new Dictionary<string, IniSection>();

    // accessor
    public IniSection this[string key]
    {
        get { return _sections.ContainsKey(key) ? _sections[key] : null; }
        set { _sections[key] = value; }
    }

    // data access
    public string GetStringMulti(string section, string fallback, params string[] keyAliases)
    {
        if(_sections.TryGetValue(section, out var iniSection))
        {
            return iniSection.GetStringMulti(fallback, keyAliases);
        }
        return fallback;
    }

    public string GetString(string section, string key, string fallback)
    {
        if (_sections.TryGetValue(section, out var iniSection))
        {
            return iniSection.GetString(key, fallback);
        }
        return fallback;
    }

    public bool GetBoolMulti(string section, bool fallback, params string[] keyAliases)
    {
        if (_sections.TryGetValue(section, out var iniSection))
        {
            return iniSection.GetBoolMulti(fallback, keyAliases);
        }
        return fallback;
    }

    public bool GetBool(string section, string key, bool fallback = false)
    {
        if (_sections.TryGetValue(section, out var iniSection))
        {
            return iniSection.GetBool(key, fallback);
        }
        return fallback;
    }

    public float GetFloatMulti(string section, float fallback, params string[] keyAliases)
    {
        if (_sections.TryGetValue(section, out var iniSection))
        {
            return iniSection.GetFloatMulti(fallback, keyAliases);
        }
        return fallback;
    }

    public float GetFloat(string section, string key, float fallback = 0f)
    {
        if (_sections.TryGetValue(section, out var iniSection))
        {
            return iniSection.GetFloat(key, fallback);
        }
        return fallback;
    }

    public int GetIntMulti(string section, int fallback, params string[] keyAliases)
    {
        if (_sections.TryGetValue(section, out var iniSection))
        {
            return iniSection.GetIntMulti(fallback, keyAliases);
        }
        return fallback;
    }

    public int GetInt(string section, string key, int fallback = 0)
    {
        if (_sections.TryGetValue(section, out var iniSection))
        {
            return iniSection.GetInt(key, fallback);
        }
        return fallback;
    }

    // parsing
    public void Parse(string input)
    {
        // scope vars
        string currentSection = "Root";
        this["Root"] = new IniSection();

        // parser
        var lines = input.Split('\r', '\n');
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();

            // should we skip this line?
            if (line.Length == 0 || CommentChars.Contains(line[0]))
                continue;

            // new section
            if (line[0] == '[')
            {
                // get section name, and inherited section if allowed
                currentSection = line.Substring(1, line.Length - 2);
                string inheritSection = null;
                if (currentSection.Contains(":") && AllowInheritance)
                {
                    string[] secSplits = currentSection.Split(':');
                    currentSection = secSplits[0].Trim();
                    inheritSection = secSplits[1].Trim();
                }

                // rename duplicates
                if (RenameDuplicateSections)
                {
                    int appendNum = 0;
                    string nameBeforeAppend = currentSection;
                    while (_sections.ContainsKey(currentSection))
                    {
                        currentSection = nameBeforeAppend + appendNum;
                        appendNum++;
                    }
                }

                // add to sections dict
                var newSection = new IniSection();

                // check inheritance
                if (AllowInheritance && inheritSection != null)
                {
                    if (_sections.ContainsKey(inheritSection))
                    {
                        _sections[inheritSection].CopyTo(newSection);
                    }
                }

                _sections[currentSection] = newSection;
                continue;
            }

            // check for kvp
            if (!line.Contains(SetterChar))
                continue;

            // split kv
            string[] splits = line.Split(new[] { SetterChar }, AllowEmptyValues ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
            if (splits.Length <= 1)
                continue;

            // split key and value
            string key = splits[0];
            string value = splits[1];

            // check if we need to strip a comment
            if (StripValueComments)
            {
                foreach (var commentChar in CommentChars)
                {
                    if (value.IndexOf(commentChar) >= 0)
                    {
                        value = value.Substring(0, commentChar);
                    }
                }
            }

            // trim if needed
            if (TrimKeyValues)
            {
                key = key.Trim();
                value = value.Trim();
            }

            // finally, add to dictionary
            _sections[currentSection][key] = value;
        }
    }

    // etc
    public override string ToString()
    {
        var emit = new StringBuilder();

        // first emit root stuff
        foreach (KeyValuePair<string, IniSection> f_kvp in _sections)
        {
            emit.AppendLine($"[{f_kvp.Key}]");
            foreach (KeyValuePair<string, string> s_kvp in f_kvp.Value)
            {
                emit.AppendLine($"{s_kvp.Key}{SetterChar}{s_kvp.Value}");
            }
            emit.AppendLine();
        }

        return emit.ToString();
    }

    public IEnumerator GetEnumerator()
    {
        return _sections.GetEnumerator();
    }

    //ctors
    public IniFile() { }

    public IniFile(string content)
    {
        Parse(content);
    }
}

public class IniSection : IEnumerable
{
    // private properties
    private Dictionary<string, string> _values = new Dictionary<string, string>();

    // utility functions
    public void CopyTo(IniSection other)
    {
        foreach (var kvp in _values)
        {
            other[kvp.Key] = kvp.Value;
        }
    }

    // accessor
    public string this[string key]
    {
        get { return _values.ContainsKey(key) ? _values[key] : null; }
        set { _values[key] = value; }
    }

    public bool ContainsKey(string key)
    {
        return _values.ContainsKey(key);
    }

    public string GetStringMulti(string fallback, params string[] keyAliases)
    {
        foreach (var alias in keyAliases)
        {
            string result = GetString(alias, null);
            if (result != null)
                return result;
        }
        return fallback;
    }

    public string GetString(string key, string fallback)
    {
        if (_values.TryGetValue(key, out var retval))
        {
            return retval;
        }
        else
        {
            return fallback;
        }
    }

    public bool GetBoolMulti(bool fallback, params string[] keyAliases)
    {
        foreach (var alias in keyAliases)
        {
            string result = GetString(alias, null);
            if (result != null)
                return bool.Parse(result);
        }
        return fallback;
    }

    public bool GetBool(string key, bool fallback = false)
    {
        bool retval = fallback;
        if(_values.TryGetValue(key, out var tryParse))
        {
            retval = bool.Parse(tryParse);
        }
        return retval;
    }

    public float GetFloatMulti(float fallback, params string[] keyAliases)
    {
        foreach (var alias in keyAliases)
        {
            string result = GetString(alias, null);
            if (result != null)
                return float.Parse(result, CultureInfo.InvariantCulture);
        }
        return fallback;
    }

    public float GetFloat(string key, float fallback = 0f)
    {
        float retval = fallback;
        if (_values.TryGetValue(key, out var tryParse))
        {
            retval = float.Parse(tryParse, CultureInfo.InvariantCulture);
        }
        return retval;
    }

    public int GetIntMulti(int fallback, params string[] keyAliases)
    {
        foreach (var alias in keyAliases)
        {
            string result = GetString(alias, null);
            if (result != null)
                return int.Parse(result, CultureInfo.InvariantCulture);
        }
        return fallback;
    }

    public int GetInt(string key, int fallback = 0)
    {
        int retval = fallback;
        if (_values.TryGetValue(key, out var tryParse))
        {
            retval = int.Parse(tryParse, CultureInfo.InvariantCulture);
        }
        return retval;
    }

    public Dictionary<string, string> AsDictionary()
    {
        return new Dictionary<string, string>(_values);
    }

    public IEnumerator GetEnumerator()
    {
        return _values.GetEnumerator();
    }
}

