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
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AtlasFont
{
    //
    private static AtlasFont _systemFont;
    public static AtlasFont SystemFont
    {
        get
        {
            if (_systemFont != null)
                return _systemFont;
            var fontTextAsset = Resources.Load<TextAsset>("BuiltinFonts/modeseven");
            using (var fontAssetReader = new BinaryReader(new MemoryStream(fontTextAsset.bytes)))
            {
                _systemFont = AtlasFont.FromBinary(fontAssetReader);
            }
            return _systemFont;
        }
    }

    //
    public Dictionary<char, Rect> CharacterMappingDict = new Dictionary<char, Rect>();
    public Texture2D AtlasImage;
    public int BaseFontSize = 20;

    public int CharacterCount => CharacterMappingDict.Count;

    private Material StampMaterial = null;
    
    private void CreateStampMaterial()
    {
        if (StampMaterial != null)
            return;
        StampMaterial = new Material(Shader.Find("Hidden/Stamper"));
        StampMaterial.mainTexture = AtlasImage;
    }

    private void SetStampColor(Color color)
    {
        StampMaterial.color = color;
    }

    private void SetStampSourceRect(Rect rect)
    {
        StampMaterial.SetVector("_StampRect", new Vector4(rect.x, rect.y, rect.width, rect.height));
    }

    private void SetStampDestRect(Rect rect)
    {
        StampMaterial.SetVector("_StampDest", new Vector4(rect.x, rect.y, rect.width, rect.height));
    }

    public Rect GetPixelRect(char forChar)
    {
        var uvRect = CharacterMappingDict[forChar];
        return new Rect(uvRect.x * AtlasImage.width, (1f - uvRect.y - uvRect.height) * AtlasImage.height, uvRect.width * AtlasImage.width, uvRect.height * AtlasImage.height);
    }

    public void DrawWorldspace(Vector3 position, string text, float fontSize, Color foregroundColor)
    {
        DrawWorldspace(null, text, position, fontSize, foregroundColor, default, false);
    }

    public void DrawWorldspace(Vector3 position, string text, float fontSize, Color foregroundColor, Color backgroundColor)
    {
        DrawWorldspace(null, text, position, fontSize, foregroundColor, backgroundColor, true);
    }

    private int GetLongestLineLength(string text)
    {
        int curLineLength = 0;
        int longestLineLength = 0;
        for(int i=0; i < text.Length; i++)
        {
            char chr = text[i];
            if (chr == '\n' || chr == '\r')
            {
                longestLineLength = Mathf.Max(curLineLength, longestLineLength);
                curLineLength = 0;
                continue;
            }
            curLineLength++;
        }
        longestLineLength = Mathf.Max(curLineLength, longestLineLength);
        return longestLineLength;
    }

    private void DrawWorldspace(object IGNOREARG, string text, Vector3 position, float fontSize, Color foregroundColor,  Color backgroundColor, bool hasBackground)
    {
        // get main camera, rotation, and directions
        Camera activeCamera = Camera.current;

        if (hasBackground && backgroundColor.a <= 0.01f)
            hasBackground = false;

        var rotation = Quaternion.LookRotation(activeCamera.transform.forward);
        var leftDir = -activeCamera.transform.right;
        var downDir = -activeCamera.transform.up;
        var backDir = -activeCamera.transform.forward;
        float negFontSizeHalf = fontSize / -2f;
        float fontSizeHalf = fontSize / 2f;

        // compute line length, center offset, and base position
        int longestLineLength = GetLongestLineLength(text);
        float centerOffsetMultiplier = (longestLineLength == 2 ? 0.5f : longestLineLength / 2);
        Vector3 basePosition = position + (leftDir * centerOffsetMultiplier * fontSize);

        // original state
        var originalDDColor = DebugDraw.Color;

        // start drawing
        DebugDraw.PushMatrix();

        // how many \n's we've hit
        int currentLine = 0;
        position = basePosition;

        if (hasBackground)
        {
            DebugDraw.Color = backgroundColor;
            DebugDraw.BeginOpaque(DrawType.QUADS);

            Vector3 lineStartPosition = position;
            int currentLineLength = 0;

            void drawLineBg()
            {
                if (currentLineLength <= 0)
                    return;
                Vector3 bgBottomLeft = rotation * new Vector3(negFontSizeHalf, negFontSizeHalf, 0f) + lineStartPosition;
                Vector3 bgBottomRight = rotation * new Vector3(fontSizeHalf, negFontSizeHalf, 0f) + position;
                Vector3 bgTopLeft = rotation * new Vector3(negFontSizeHalf, fontSizeHalf, 0f) + lineStartPosition;
                Vector3 bgTopRight = rotation * new Vector3(fontSizeHalf, fontSizeHalf, 0f) + position;

                DebugDraw.Vertex(bgTopLeft + -(backDir * 0.01f));
                DebugDraw.Vertex(bgTopRight + -(backDir * 0.01f));
                DebugDraw.Vertex(bgBottomRight + -(backDir * 0.01f));
                DebugDraw.Vertex(bgBottomLeft + -(backDir * 0.01f));
            }

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\r')
                {
                    // draw background for this line
                    position += -leftDir * fontSize * (currentLineLength - 1);
                    drawLineBg();

                    // move to next line
                    currentLine++;
                    position = basePosition + (downDir * currentLine * fontSize);
                    lineStartPosition = position;
                    currentLineLength = 0;
                    continue;
                }

                // increment position
                currentLineLength++;
            }

            // draw final line bg
            position += -leftDir * fontSize * (currentLineLength - 1);
            drawLineBg();

            DebugDraw.End();
        }

        currentLine = 0;
        position = basePosition;

        DebugDraw.Color = foregroundColor;
        DebugDraw.BeginTexturedTransparent(DrawType.QUADS, AtlasImage);
        
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == '\n')
            {
                currentLine++;
                position = basePosition + (downDir * currentLine * fontSize);
                continue;
            }

            Vector3 bgBottomLeft = rotation * new Vector3(negFontSizeHalf, negFontSizeHalf, 0f) + position;
            Vector3 bgBottomRight = rotation * new Vector3(fontSizeHalf, negFontSizeHalf, 0f) + position;
            Vector3 bgTopLeft = rotation * new Vector3(negFontSizeHalf, fontSizeHalf, 0f) + position;
            Vector3 bgTopRight = rotation * new Vector3(fontSizeHalf, fontSizeHalf, 0f) + position;

            if (CharacterMappingDict.TryGetValue(text[i], out Rect charMapping))
            {
                Vector2 uv0 = new Vector2(charMapping.x, 1f - charMapping.y);
                Vector2 uv1 = new Vector2(charMapping.x + charMapping.width, 1f - charMapping.y);
                Vector2 uv2 = new Vector2(charMapping.x + charMapping.width, 1f - charMapping.y - charMapping.height);
                Vector2 uv3 = new Vector2(charMapping.x, 1f - charMapping.y - charMapping.height);


                DebugDraw.Vertex(bgTopLeft);
                DebugDraw.TexCoord(uv0);
                DebugDraw.Vertex(bgTopRight);
                DebugDraw.TexCoord(uv1);
                DebugDraw.Vertex(bgBottomRight);
                DebugDraw.TexCoord(uv2);
                DebugDraw.Vertex(bgBottomLeft);
                DebugDraw.TexCoord(uv3);
            }

            // increment position
            position += -leftDir * fontSize;
        }

        DebugDraw.PopMatrix();
        DebugDraw.End();

        DebugDraw.Color = originalDDColor;
    }

    public void DrawOnTexture(string text, Texture2D drawOn, float fontSize)
    {
        DrawOnTexture(text, drawOn, fontSize, Color.white);
    }

    public void DrawOnTexture(string text, Texture2D drawOn, float fontSize, Color color)
    {
        int reverseY = drawOn.height - Mathf.CeilToInt(fontSize);
        DrawOnTexture(text, drawOn, fontSize, 0, reverseY, color);
    }

    public void DrawOnTexture(string text, Texture2D drawOn, float fontSize, int offsetX, int offsetY)
    {
        DrawOnTexture(text, drawOn, fontSize, offsetX, offsetY, Color.white);
    }

    public void DrawOnTexture(string text, Texture2D drawOn, float fontSize, int offsetX, int offsetY, Color color)
    {
        // stamping vars
        int ix = 0;
        int iy = 0;
        float sx = (fontSize / drawOn.width);
        float sy = (fontSize / drawOn.height);
        float ox = (float)offsetX / drawOn.width;
        float oy = (float)offsetY / drawOn.height;

        // create rt
        RenderTexture rt = RenderTexture.GetTemporary(drawOn.width, drawOn.height);
        RenderTexture.active = rt;

        // blit down original
        Graphics.Blit(drawOn, rt);

        // create stamp material if not exist
        CreateStampMaterial();
        SetStampColor(color);

        // start stamping
        for (int i=0; i < text.Length; i++)
        {
            char textChar = text[i];
            if(textChar == '\n')
            {
                ix = 0;
                iy -= Mathf.RoundToInt(fontSize); // subtract since Y is inverted in Unity
                continue;
            }

            if (CharacterMappingDict.ContainsKey(textChar))
            {
                var mapping = CharacterMappingDict[textChar];
                mapping.y = 1f - mapping.y - mapping.height; // invert Y since Y is inverted in Unity
                
                SetStampSourceRect(mapping);
                SetStampDestRect(new Rect(ox + ((float)ix / drawOn.width), oy + ((float)iy / drawOn.height), sx, sy));
                Graphics.Blit(AtlasImage, rt, StampMaterial);
                
            }
            ix += Mathf.FloorToInt(fontSize);
            if (ix + fontSize >= drawOn.width)
                break;

        }

        drawOn.ReadPixels(new Rect(0, 0, drawOn.width, drawOn.height), 0, 0);
        drawOn.Apply();
        RenderTexture.active = null;
    }

    public static AtlasFont FromBinary(BinaryReader reader)
    {
        bool validMagic = (reader.ReadChar() == 'F' && reader.ReadChar() == 'A' && reader.ReadChar() == 'T');
        if (!validMagic)
            throw new Exception("Not a FontATlas file.");
        char version = reader.ReadChar();
        if (version != '0')
            throw new Exception("Unsupported FontATlas file.");

        var returnAtlas = new AtlasFont();

        int charCount = reader.ReadUInt16();
        returnAtlas.BaseFontSize = reader.ReadUInt16();
        int atlasByteLen = reader.ReadInt32();
        for (int i = 0; i < charCount; i++)
        {
            ushort charCode = reader.ReadUInt16();
            char @char = (char)charCode;

            float mapX = reader.ReadSingle();
            float mapY = reader.ReadSingle();
            float mapW = reader.ReadSingle();
            float mapH = reader.ReadSingle();

            returnAtlas.CharacterMappingDict[@char] = new Rect(mapX, mapY, mapW, mapH);
            
        }

        // read atlas from stream
        byte[] pngBytes = reader.ReadBytes(atlasByteLen);
        var pngImage = new Texture2D(8, 8, TextureFormat.ARGB32, true);
        pngImage.LoadImage(pngBytes);

        returnAtlas.AtlasImage = pngImage;
        returnAtlas.AtlasImage.filterMode = FilterMode.Trilinear;

        return returnAtlas;
    }

}
