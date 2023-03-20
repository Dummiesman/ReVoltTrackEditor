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

using System.IO;
using UnityEngine;

public static class BinaryExtensions 
{
    public static void WriteMatrix3x3(this BinaryWriter writer, Matrix4x4 matrix)
    {
        writer.WriteVector3(matrix.GetColumn(0));
        writer.WriteVector3(matrix.GetColumn(1));
        writer.WriteVector3(matrix.GetColumn(2));
    }

    public static void WriteVector3(this BinaryWriter writer, Vector3 vec)
    {
        writer.Write(vec.x);
        writer.Write(vec.y);
        writer.Write(vec.z);
    }

    public static void WriteVector2(this BinaryWriter writer, Vector2 vec)
    {
        writer.Write(vec.x);
        writer.Write(vec.y);
    }

    public static Matrix4x4 ReadMatrix3x3(this BinaryReader reader)
    {
        var mtxBase = Matrix4x4.identity;
        mtxBase.SetColumn(0, reader.ReadVector3());
        mtxBase.SetColumn(1, reader.ReadVector3());
        mtxBase.SetColumn(2, reader.ReadVector3());
        return mtxBase;
    }

    public static Vector3 ReadVector3(this BinaryReader reader)
    {
        float x = reader.ReadSingle();
        float y = reader.ReadSingle();
        float z = reader.ReadSingle();
        return new Vector3(x, y, z);
    }

    public static Vector2 ReadVector2(this BinaryReader reader)
    {
        float x = reader.ReadSingle();
        float y = reader.ReadSingle();
        return new Vector3(x, y);
    }

    /// <summary>
    /// Read a 128 bit color in RGBA order
    /// </summary>
    public static Color ReadColor32F(this BinaryReader reader)
    {
        float r = reader.ReadSingle();
        float g = reader.ReadSingle();
        float b = reader.ReadSingle();
        float a = reader.ReadSingle();
        return new Color(r, g, b, a);
    }

    /// <summary>
    /// Read a 96 bit color in RGB order
    /// </summary>
    public static Color ReadColor24F(this BinaryReader reader)
    {
        float r = reader.ReadSingle();
        float g = reader.ReadSingle();
        float b = reader.ReadSingle();
        return new Color(r, g, b, 1f);
    }

    /// <summary>
    /// Read a 96 bit color in RGB order (converts to 0-255 range)
    /// </summary>
    public static Color ReadColor24FB(this BinaryReader reader)
    {
        float r = reader.ReadSingle() / 255f;
        float g = reader.ReadSingle() / 255f;
        float b = reader.ReadSingle() / 255f;
        return new Color(r, g, b, 1f);
    }

    /// <summary>
    /// Read a 48 bit color in RGB order (converts to 0-255 range)
    /// </summary>
    public static Color ReadColor24HB(this BinaryReader reader)
    {
        float r = reader.ReadUInt16() / 255f;
        float g = reader.ReadUInt16() / 255f;
        float b = reader.ReadUInt16() / 255f;
        return new Color(r, g, b, 1f);
    }

    /// <summary>
    /// Read a 32 bit color in RGBA order
    /// </summary>
    public static Color32 ReadColor32B(this BinaryReader reader)
    {
        byte r = reader.ReadByte();
        byte g = reader.ReadByte();
        byte b = reader.ReadByte();
        byte a = reader.ReadByte();
        return new Color32(r, g, b, a);
    }

    /// <summary>
    /// Read a 24 bit color in RGB order
    /// </summary>
    public static Color32 ReadColor24B(this BinaryReader reader)
    {
        byte r = reader.ReadByte();
        byte g = reader.ReadByte();
        byte b = reader.ReadByte();
        return new Color32(r, g, b, 255);
    }

    /// <summary>
    /// Read a 32 bit color in BGRA order
    /// </summary>
    public static Color32 ReadColorBGRA32B(this BinaryReader reader)
    {
        byte b = reader.ReadByte();
        byte g = reader.ReadByte();
        byte r = reader.ReadByte();
        byte a = reader.ReadByte();
        return new Color32(r, g, b, a);
    }

    /// <summary>
    /// Write a 32 bit color in BGRA order
    /// </summary>
    public static void WriteColorBGRA32B(this BinaryWriter writer, Color32 color)
    {
        writer.Write(color.b);
        writer.Write(color.g);
        writer.Write(color.r);
        writer.Write(color.a);
    }

    /// <summary>
    /// Write a 96 bit color in RGB order
    /// </summary>
    public static void WriteColor24F(this BinaryWriter writer, Color color)
    {
        writer.Write(color.r);
        writer.Write(color.g);
        writer.Write(color.b);
    }

    /// <summary>
    /// Write a 96 bit color in RGB order (converts to 0-255 range)
    /// </summary>
    public static void WriteColor24FB(this BinaryWriter writer, Color color)
    {
        writer.Write(color.r * 255f);
        writer.Write(color.g * 255f);
        writer.Write(color.b * 255f);
    }

    public static string ReadPaddedString(this BinaryReader reader)
    {
        byte len = reader.ReadByte();
        var builder = new System.Text.StringBuilder(len);

        for (int i = 0; i < len; i++)
            builder.Append(reader.ReadChar());

        reader.ReadByte(); // pad
        if ((len % 2) > 0)
            reader.ReadByte(); // pad

        return builder.ToString();
    }

    public static void WritePaddedString(this BinaryWriter writer, string str)
    {
        int len = (str == null) ? 0 : Mathf.Min(byte.MaxValue, str.Length);

        writer.Write((byte)len);
        if (str != null)
            for (int i = 0; i < len; i++)
                writer.Write(str[i]);

        writer.Write('\x00'); // pad
        if((len % 2) > 0)
            writer.Write('\x00'); // pad
    }
}
