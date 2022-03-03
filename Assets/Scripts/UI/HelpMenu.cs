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

using System.Text;
using TMPro;
using UnityEngine;

public class HelpMenu : MonoBehaviour
{
    public TMP_Text Text;
    public RectTransform BackgroundRowsLayout;
    public RectTransform Content;

    const int MAX_LINES = 11;

    public void LoadHelpText(string resourcePath)
    {
        string[] textLines = Resources.Load<TextAsset>(resourcePath).text.Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        int numLines = textLines.Length - 1;

        var newText = new StringBuilder();

        // output header
        newText.AppendLine("<align=center>Controls Reference</align>");
        newText.AppendLine();
        newText.AppendLine("<line-height=140%><u>ACTION\t\t\tKEYBOARD\t\t\t\tGAMEPAD              <color=#0000>`</color></u>");

        // output lines
        for (int i=0; i < numLines && i < MAX_LINES; i++)
        {
            string[] splits = textLines[i+1].Split(',');
            newText.AppendLine($"{splits[0]}<pos=34.5%>{splits[1]}<pos=68.75%>{splits[2]}" + ((i == 0) ? "</line-height>" : string.Empty));
        }
        
        // setup background and content rect
        for(int i=0; i < MAX_LINES; i++)
        {
            BackgroundRowsLayout.GetChild(i).gameObject.SetActive(i < numLines);
        }
        Content.sizeDelta = new Vector2(Content.sizeDelta.x, 185f + 22f * numLines);
        
        //output footer
        newText.AppendLine();
        newText.AppendLine("<align=center>Press Any Button To Close</align>");

        Text.text = newText.ToString();
    }
}
