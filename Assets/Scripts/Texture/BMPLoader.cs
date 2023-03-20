/*
    Re-Volt Track Editor - Unity Edition
    A version of the track editor re-built from the ground up in Unity
    Copyright (C) 2023 Dummiesman

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
using System.IO;
using UnityEngine;

/// <summary>
/// BMP File Loader
/// Capabilities: Loads BITMAPCOREHEADER, BITMAPINFOHEADER, BITMAPV2HEADER, BITMAPV4HEADER, BITMAPV5HEADER typed bitmaps
/// - Supports uncompressed, 1/2/4/8/16/24/32 bit images
/// - Supports RLE compressed 4/8 bit images
/// Notes: There is a bunch of nearly duplicate code to facilitate fast loading
/// </summary>
public class BMPLoader
{
    const ushort MAGIC = 0x4D42; // "BM"

    const int BITMAPCOREHEADER_SIZE = 12;
    const int BITMAPINFOHEADER_SIZE = 40;
    const int BITMAPV2HEADER_SIZE = 52;
    const int BITMAPV3HEADER_SIZE = 56;
    const int BITMAPV4HEADER_SIZE = 108;
    const int BITMAPV5HEADER_SIZE = 124;

    public enum BMPCompressionMode : int
    {
        BI_RGB = 0x00,
        BI_RLE8 = 0x01,
        BI_RLE4 = 0x02,
        BI_BITFIELDS = 0x03,
        BI_JPEG = 0x04,
        BI_PNG = 0x05,
        BI_ALPHABITFIELDS = 0x06,

        BI_CMYK = 0x0B,
        BI_CMYKRLE8 = 0x0C,
        BI_CMYKRLE4 = 0x0D
    }

    class BitmapInfoHeader
    {
        public int Width;
        public int Height;
        public int BitsPerPixel;
        public uint ColorMaskR;
        public uint ColorMaskG;
        public uint ColorMaskB;
        public uint ColorMaskA;
        public bool VerticalFlip = false;
        public int PaletteSize = 0;
        public uint ImageDataSize = 0;
        public BMPCompressionMode CompressionType = BMPCompressionMode.BI_RGB;
        public Color32[] Palette = new Color32[256];

        public void DefaultColorMask()
        {
            ColorMaskR = (BitsPerPixel > 16) ? (uint)0x00FF0000 : (uint)0x00007C00;
            ColorMaskG = (BitsPerPixel > 16) ? (uint)0x0000FF00 : (uint)0x000003E0;
            ColorMaskB = (BitsPerPixel > 16) ? (uint)0x000000FF : (uint)0x0000001F;
            ColorMaskA = 0x00000000;
        }
    }

    private static int DefaultPaletteSize(int bpp)
    {
        switch(bpp)
        {
            case 8:
                return 256;
            case 4:
                return 16;
            case 1:
                return 2;
            
        }
        return 0;
    }

    private static int PaddedScanLineSize(int scanLineSize)
    {
        if((scanLineSize % 4) == 0)
            return scanLineSize;
        else
            return scanLineSize + (4 - (scanLineSize % 4));
    }

    private static int GetShiftValue(uint mask)
    {
        for (int i = 0; i < 32; i++)
        {
            if ((mask & (1 << i)) != 0)
                return i;
        }
        return 0;
    }

    private static float GetShiftMultiplier(uint mask)
    {
        int numSetBits = 0;
        for (int i = 0; i < 32; i++)
        {
            if ((mask & (1 << i)) != 0)
                numSetBits++;
        }
        return (numSetBits > 0) ? 255f / ((1 << numSetBits) - 1) : 0f;
    }

    private static Color32[] ReadColorsMasked(BinaryReader reader, BitmapInfoHeader header)
    {
        Color32[] colors = new Color32[header.Width * header.Height];
        int pixelByteSize = (header.BitsPerPixel >> 3);
        int scanLineSize = PaddedScanLineSize(header.Width * pixelByteSize);

        int scanLinePaddedSize = scanLineSize + 2; // allocate enough bytes to allow treating each pixel as 32 bit
        byte[] scanLine = new byte[scanLinePaddedSize];

        uint dataMask = 0xFFFFFFFF >> (32 - header.BitsPerPixel);
        int[] shiftAmounts = new[] { GetShiftValue(header.ColorMaskR), GetShiftValue(header.ColorMaskG),
                                     GetShiftValue(header.ColorMaskB), GetShiftValue(header.ColorMaskA) };
        float[] shiftMultipliers = new[] { GetShiftMultiplier(header.ColorMaskR), GetShiftMultiplier(header.ColorMaskG), 
                                           GetShiftMultiplier(header.ColorMaskB), GetShiftMultiplier(header.ColorMaskA) };

        if (header.ColorMaskA == 0x00000000)
        {
            for (int y = 0; y < header.Height; y++)
            {
                reader.BaseStream.Read(scanLine, 0, scanLineSize);
                for (int x = 0; x < header.Width; x++)
                {
                    uint pixelData = BitConverter.ToUInt32(scanLine, x * pixelByteSize) & dataMask;
                    byte cR = (byte)(((pixelData & header.ColorMaskR) >> shiftAmounts[0]) * shiftMultipliers[0]);
                    byte cG = (byte)(((pixelData & header.ColorMaskG) >> shiftAmounts[1]) * shiftMultipliers[1]);
                    byte cB = (byte)(((pixelData & header.ColorMaskB) >> shiftAmounts[2]) * shiftMultipliers[2]);
                    colors[(y * header.Width) + x] = new Color32(cR, cG, cB, 255);
                }
            }
        }
        else
        {
            for (int y = 0; y < header.Height; y++)
            {
                reader.BaseStream.Read(scanLine, 0, scanLineSize);
                for (int x = 0; x < header.Width; x++)
                {
                    uint pixelData = BitConverter.ToUInt32(scanLine, x * pixelByteSize) & dataMask;
                    byte cR = (byte)(((pixelData & header.ColorMaskR) >> shiftAmounts[0]) * shiftMultipliers[0]);
                    byte cG = (byte)(((pixelData & header.ColorMaskG) >> shiftAmounts[1]) * shiftMultipliers[1]);
                    byte cB = (byte)(((pixelData & header.ColorMaskB) >> shiftAmounts[2]) * shiftMultipliers[2]);
                    byte cA = (byte)(((pixelData & header.ColorMaskA) >> shiftAmounts[3]) * shiftMultipliers[3]);
                    colors[(y * header.Width) + x] = new Color32(cR, cG, cB, cA);
                }
            }
        }
        return colors;
    }

    private static Color32[] ReadColors8BPP(BinaryReader reader, BitmapInfoHeader header)
    {
        Color32[] colors = new Color32[header.Width * header.Height];
        int scanLineSize = PaddedScanLineSize(header.Width);
        byte[] scanLine = new byte[scanLineSize];

        for (int y = 0; y < header.Height; y++)
        {
            reader.BaseStream.Read(scanLine, 0, scanLineSize);
            for (int x = 0; x < header.Width; x++)
            {
                byte palIndex = scanLine[x];
                colors[(y * header.Width) + x] = header.Palette[palIndex];
            }
        }
        return colors;
    }

    private static Color32[] ReadColors8BPP_RLE(BinaryReader reader, BitmapInfoHeader header)
    {
        Color32[] colors = new Color32[header.Width * header.Height];

        int lineNum = 0;
        int y = 0;
        int x = 0;

        void nextLine()
        {
            x = 0;
            lineNum++; y = lineNum;
        }

        void setPixelsRLE(int count, byte palIndex)
        {
            for (int j = 0; j < count && x < header.Width && y < header.Height; j++)
            {
                colors[x++ + (y * header.Width)] = header.Palette[palIndex];
            }
        }

        void setPixelsAbsolute(int count)
        {
            for (int j = 0; j < count && x < header.Width && y < header.Height; j++)
            {
                byte palIndex = reader.ReadByte();
                colors[x++ + (y * header.Width)] = header.Palette[palIndex];
            }
            if ((count % 2) != 0)
                reader.ReadByte();
        }

        long targetPos = reader.BaseStream.Position + header.ImageDataSize;
        while (reader.BaseStream.Position != targetPos)
        {
            byte b = reader.ReadByte();
            if (reader.BaseStream.Position == targetPos)
                break;

            if (b == 0)
            {
                byte escapeCode = reader.ReadByte();
                if (escapeCode == 0)
                {
                    nextLine();
                }
                else if (escapeCode == 1)
                {
                    break;
                }
                else if (escapeCode == 2)
                {
                    x += reader.ReadByte();
                    y += reader.ReadByte();
                }
                else
                {
                    setPixelsAbsolute(escapeCode);
                }
            }
            else
            {
                byte palIndex = reader.ReadByte();
                setPixelsRLE(b, palIndex);
            }
        }
        return colors;
    }

    private static Color32[] ReadColors4BPP(BinaryReader reader, BitmapInfoHeader header)
    {
        Color32[] colors = new Color32[header.Width * header.Height];
        int scanLineSize = PaddedScanLineSize((header.Width + (header.Width % 2)) / 2);
        byte[] scanLine = new byte[scanLineSize];       

        for (int y = 0; y < header.Height; y++)
        {
            reader.BaseStream.Read(scanLine, 0, scanLineSize);
            int i, x = 0;
            for (i = 0; i < (header.Width / 2); i++)
            {
                colors[x++ + (y * header.Width)] = header.Palette[(scanLine[i] & 0xF0) >> 4];
                colors[x++ + (y * header.Width)] = header.Palette[scanLine[i] & 0x0F]; 
            }
            if ((header.Width % 2) != 0)
            {
                colors[x++ + (y * header.Width)] = header.Palette[(scanLine[i] & 0xF0) >> 4];
            }
        }

        return colors;
    }

    private static Color32[] ReadColors4BPP_RLE(BinaryReader reader, BitmapInfoHeader header)
    {
        Color32[] colors = new Color32[header.Width * header.Height];

        int lineNum = 0;
        int y = 0;
        int x = 0;

        void nextLine()
        {
            x = 0;
            lineNum++; y = lineNum;
        }

        void setPixelsRLE(int count, byte palIndices)
        {
            int[] nibbles = new int[2] { (palIndices & 0xF0) >> 4, palIndices & 0x0F };
            for (int j = 0; j < count && x < header.Width && y < header.Height; j++)
            {
                colors[x++ + (y * header.Width)] = header.Palette[nibbles[j%2]];
            }
        }

        void setPixelsAbsolute(int count)
        {
            int byteCount = (count + (count % 2)) / 2;
            for(int j=0; j < (count / 2); j++)
            {
                byte lastByte = reader.ReadByte();
                colors[x++ + (y * header.Width)] = header.Palette[(lastByte & 0xF0) >> 4];
                colors[x++ + (y * header.Width)] = header.Palette[lastByte & 0x0F];
            }
            if((count % 2) != 0)
            {
                byte lastByte = reader.ReadByte();
                colors[x++ + (y * header.Width)] = header.Palette[(lastByte & 0xF0) >> 4];
            }
            if ((byteCount % 2) != 0)
                reader.ReadByte();
        }

        long targetPos = reader.BaseStream.Position + header.ImageDataSize;
        while (reader.BaseStream.Position != targetPos)
        {
            byte b = reader.ReadByte();
            if (reader.BaseStream.Position == targetPos)
                break;

            if (b == 0)
            {
                byte escapeCode = reader.ReadByte();
                if (escapeCode == 0)
                {
                    nextLine();
                }
                else if (escapeCode == 1)
                {
                    break;
                }
                else if (escapeCode == 2)
                {
                    x += reader.ReadByte();
                    y += reader.ReadByte();
                }
                else
                {
                    setPixelsAbsolute(escapeCode);
                }
            }
            else
            {
                byte palIndices = reader.ReadByte();
                setPixelsRLE(b, palIndices);
            }
        }
        return colors;
    }

    private static Color32[] ReadColors1BPP(BinaryReader reader, BitmapInfoHeader header)
    {
        Color32[] colors = new Color32[header.Width * header.Height];
        int scanLineSize = PaddedScanLineSize((header.Width + (header.Width % 8)) / 8);
        byte[] scanLine = new byte[scanLineSize];

        for (int y = 0; y < header.Height; y++)
        {
            reader.BaseStream.Read(scanLine, 0, scanLineSize);
            for (int x = 0; x < header.Width; x++)
            {
                int scanLineIndex = (x / 8);
                int bitIndex = (x % 8);

                byte b = scanLine[scanLineIndex];
                int palIndex = (b & (128 >> bitIndex)) != 0 ? 1 : 0;
                colors[(y * header.Width) + x] = header.Palette[palIndex];
            }
        }

        return colors;
    }

    private static void LoadHeaderCore(BinaryReader reader, BitmapInfoHeader header)
    {
        header.Width = reader.ReadUInt16();
        header.Height = reader.ReadUInt16();
        reader.BaseStream.Seek(2, SeekOrigin.Current);
        header.BitsPerPixel = reader.ReadUInt16();
        header.PaletteSize = DefaultPaletteSize(header.BitsPerPixel);
        header.DefaultColorMask();
    }

    private static void LoadHeader(BinaryReader reader, BitmapInfoHeader header, int headerSize)
    {
        long infoHeaderOrigin = reader.BaseStream.Position;

        header.Width = reader.ReadInt32();
        header.Height = reader.ReadInt32();
        header.VerticalFlip = (header.Height < 0);
        header.Height = Mathf.Abs(header.Height);
        reader.BaseStream.Seek(2, SeekOrigin.Current);
        header.BitsPerPixel = reader.ReadUInt16();
        header.CompressionType = (BMPCompressionMode)reader.ReadInt32();
        header.ImageDataSize = reader.ReadUInt32();
        reader.BaseStream.Seek(8, SeekOrigin.Current); // skip past image data size, pixelspermeter,

        int paletteSize = reader.ReadInt32();
        header.PaletteSize = (paletteSize == 0) ? DefaultPaletteSize(header.BitsPerPixel) 
                                                : Math.Min(header.Palette.Length, paletteSize);

        reader.BaseStream.Seek(4, SeekOrigin.Current); // skip past important color count

        if (headerSize > BITMAPINFOHEADER_SIZE)
        {
            header.ColorMaskR = reader.ReadUInt32();
            header.ColorMaskG = reader.ReadUInt32();
            header.ColorMaskB = reader.ReadUInt32();
            if (headerSize > BITMAPV2HEADER_SIZE)
            {
                header.ColorMaskA = reader.ReadUInt32();
            }
        }
        else
        {
            header.DefaultColorMask();
        }
        reader.BaseStream.Seek(headerSize + infoHeaderOrigin - 4, SeekOrigin.Begin); // skip to end of info header
    }

    private static Texture2D Load(BinaryReader reader)
    {
        long seekOrigin = reader.BaseStream.Position;

        // read header
        ushort magic = reader.ReadUInt16();
        if (magic != MAGIC)
        {
            Debug.LogError("Not a bitmap file.");
            return null;
        }

        reader.BaseStream.Seek(8, SeekOrigin.Current); // skip file size, reserved
        uint pixelDataOffset = reader.ReadUInt32();

        // read info header
        int infoHeaderSize = reader.ReadInt32();
        if (infoHeaderSize != BITMAPCOREHEADER_SIZE && infoHeaderSize != BITMAPINFOHEADER_SIZE
          && infoHeaderSize != BITMAPV2HEADER_SIZE && infoHeaderSize != BITMAPV3HEADER_SIZE
          && infoHeaderSize != BITMAPV4HEADER_SIZE && infoHeaderSize != BITMAPV5HEADER_SIZE) 
        {
            Debug.LogError($"Unknown bitmap header type {infoHeaderSize}");
            return null;
        }

        BitmapInfoHeader header = new BitmapInfoHeader();
        if(infoHeaderSize == BITMAPCOREHEADER_SIZE)
        {
            LoadHeaderCore(reader, header);
        }
        else
        {
            LoadHeader(reader, header, infoHeaderSize);
        }

        // sanity checks
        var compressionType = header.CompressionType;
        if (compressionType != BMPCompressionMode.BI_RGB
            && compressionType != BMPCompressionMode.BI_BITFIELDS
            && compressionType != BMPCompressionMode.BI_ALPHABITFIELDS
            && compressionType != BMPCompressionMode.BI_RLE8
            && compressionType != BMPCompressionMode.BI_RLE4)
        {
            Debug.LogError($"Unsupported compression type ({compressionType})");
            return null;
        }
        if(header.BitsPerPixel > 32 || header.BitsPerPixel == 2)
        {
            Debug.LogError($"Unsupported bits per pixel ({header.BitsPerPixel})");
            return null;
        }
        if(header.Width < 0 || header.Width >= 1000000 || header.Height >= 1000000)
        {
            Debug.LogError($"Malformed image");
            return null;
        }

        // read the color mask if applicable
        if (infoHeaderSize == BITMAPINFOHEADER_SIZE)
        {
            if (compressionType == BMPCompressionMode.BI_BITFIELDS || compressionType == BMPCompressionMode.BI_ALPHABITFIELDS)
            {
                header.ColorMaskR = reader.ReadUInt32();
                header.ColorMaskG = reader.ReadUInt32();
                header.ColorMaskB = reader.ReadUInt32();
            }
            if (compressionType == BMPCompressionMode.BI_ALPHABITFIELDS)
            {
                header.ColorMaskA = reader.ReadUInt32();
            }
        }

        // read the palette
        for (int i=0; i < header.PaletteSize; i++)
        {
            byte cB = reader.ReadByte();
            byte cG = reader.ReadByte();
            byte cR = reader.ReadByte();
            if(infoHeaderSize != BITMAPCOREHEADER_SIZE) reader.ReadByte();
            header.Palette[i] = new Color32(cR, cG, cB, 255);
        }

        // start reading image data
        bool hasAlphaChannel = (header.ColorMaskA != 0x00000000);
        Texture2D texture = new Texture2D(header.Width, header.Height,  hasAlphaChannel ? TextureFormat.RGBA32 : TextureFormat.RGB24, true);
        reader.BaseStream.Seek(seekOrigin + pixelDataOffset, SeekOrigin.Begin);

        if (header.BitsPerPixel == 16 || header.BitsPerPixel == 24 || header.BitsPerPixel == 32)
        {
            texture.SetPixels32(ReadColorsMasked(reader, header));
        }
        else if (header.BitsPerPixel == 8)
        {
            if (compressionType == BMPCompressionMode.BI_RLE8)
                texture.SetPixels32(ReadColors8BPP_RLE(reader, header));
            else
                texture.SetPixels32(ReadColors8BPP(reader, header));
        }
        else if (header.BitsPerPixel == 4)
        {
            if (compressionType == BMPCompressionMode.BI_RLE4)
                texture.SetPixels32(ReadColors4BPP_RLE(reader, header));
            else
                texture.SetPixels32(ReadColors4BPP(reader, header));
        }
        else if (header.BitsPerPixel == 1)
        {
            texture.SetPixels32(ReadColors1BPP(reader, header));
        }

        texture.Apply(true);
        return texture;
    }

    public static Texture2D Load(Stream stream)
    {
        using(var reader = new BinaryReader(stream))
        {
            return Load(reader);
        }
    }

    public static Texture2D Load(byte[] data)
    {
        using (var stream = new MemoryStream(data))
        {
            return Load(stream);
        }
    }

    public static Texture2D Load(string path)
    {
        using (var stream = File.OpenRead(path))
        {
            return Load(stream);
        }
    }
}
