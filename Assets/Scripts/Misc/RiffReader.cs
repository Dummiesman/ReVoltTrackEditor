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
using System.Collections.Generic;
using System.IO;

namespace RIFF
{
    public class Chunk
    {
        public bool IsRIFF => FourCC.Equals("RIFF", StringComparison.OrdinalIgnoreCase);
        public bool IsList => FourCC.Equals("LIST", StringComparison.OrdinalIgnoreCase);
        public bool HasFormType => (IsList || IsRIFF);

        /// <summary>
        /// Returns form when it exists, else returns fourcc
        /// </summary>
        public string FourCCOrForm => (HasFormType) ? FormType : FourCC;

        public string FourCC { get; private set; }
        public string FormType { get; private set; }
        public int Length { get; private set; }

        private BinaryReader reader;
        private bool hasBeenRead = false;

        public IEnumerable<Chunk> GetSubchunks()
        {
            if (!HasFormType)
            {
                yield break;
            }

            if (hasBeenRead)
                throw new Exception("Data already read.");
            hasBeenRead = true;

            int readAmount = 0;
            while (readAmount < Length)
            {
                // yield chunk at current position
                var chunk = new Chunk(this.reader);
                yield return chunk;

                // increment read a mount
                readAmount += chunk.Length + (chunk.HasFormType ? 12 : 8) + (chunk.Length % 2);

                // if the user didnt deal with the data, discard it
                if (!chunk.hasBeenRead)
                    chunk.Skip();
            }
        }

        public byte[] GetData()
        {
            if (hasBeenRead)
                throw new Exception("Data already read.");
            hasBeenRead = true;

            byte[] returnBytes = reader.ReadBytes(Length);

            // read padding byte
            // in some cases, the end of the file is not 2 byte padded, so we check for this
            if ((Length % 2) != 0 && !(reader.BaseStream.Position == reader.BaseStream.Length))
                reader.ReadByte(); // align
            return returnBytes;
        }

        public void Skip()
        {
            if (hasBeenRead)
                throw new Exception("Data already read.");
            hasBeenRead = true;

            if (reader.BaseStream.CanSeek)
            {
                reader.BaseStream.Seek(Length + (Length % 2), SeekOrigin.Current);
            }
            else
            {
                GetData();
            }
        }

        // ctors
        public Chunk()
        {

        }

        public Chunk(BinaryReader reader)
        {
            this.reader = reader;

            // read fourcc
            string fourcc = new string(reader.ReadChars(4));
            if (fourcc.IndexOf((char)0x00) >= 0)
                fourcc = fourcc.Substring(0, fourcc.IndexOf((char)0x00));
            this.FourCC = fourcc;

            // read length
            Length = reader.ReadInt32();
            if (HasFormType)
            {
                Length -= 4; //remove formType from length

                string formType = new string(reader.ReadChars(4));
                if (formType.IndexOf((char)0x00) >= 0)
                    formType = fourcc.Substring(0, formType.IndexOf((char)0x00));
                this.FormType = formType;
            }
        }
    }

    public class RiffReader : IDisposable
    {
        private BinaryReader reader;
        public Stream Stream { get; private set; }
        public bool EOF => reader.BaseStream.Position >= reader.BaseStream.Length;
        public long Position => reader.BaseStream.Position;

        public Chunk NextChunk()
        {
            if (EOF)
                return null;
            return new Chunk(reader);
        }

        public RiffReader(string file) : this(File.OpenRead(file))
        {

        }

        public RiffReader(Stream stream)
        {
            this.Stream = stream;
            this.reader = new BinaryReader(stream);
        }

        public void Dispose()
        {
            reader.Dispose();
        }
    }
}
