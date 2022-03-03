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

using UnityEngine;
using System.IO;
using System;

namespace Dummiesman.Wave
{
    public class WAVLoader
    {
        private static string LastFileName = null;

        public static AudioClip Load(Stream stream)
        {
            var wavLoader = new RIFF.RiffReader(stream);
            var rootChunk = wavLoader.NextChunk();
            if (!rootChunk.IsRIFF || !rootChunk.FormType.Equals("wave", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception($"Not a wave file!");
            }

            // read in wav data
            WaveFormatEx format = default;
            float[] samples = null;
            foreach (var subchunk in rootChunk.GetSubchunks())
            {
                if (subchunk.FourCCOrForm.Equals("fmt ", StringComparison.OrdinalIgnoreCase))
                {
                    format = new WaveFormatEx(subchunk.GetData().CreateReader());
                    if(format.wFormatTag != WaveFormatTag.PCM && format.wFormatTag != WaveFormatTag.IEEE_FLOAT)
                    {
                        throw new Exception($"Unsupported wave wFormatTag {format.wFormatTag}.");
                    }
                }
                else if (subchunk.FourCCOrForm.Equals("data", StringComparison.OrdinalIgnoreCase))
                {
                    var rawSampleData = subchunk.GetData();
                    var sampleDataReader = rawSampleData.CreateReader();
                    int sampleCount = rawSampleData.Length / format.wChannels / (format.wBitsPerSample / 8);

                    int sampleCountWChannels = sampleCount * format.wChannels;
                    samples = new float[sampleCountWChannels];

                    // read in samples
                    switch (format.wBitsPerSample)
                    {
                        case 8:
                            {
                                byte[] byteSamples = sampleDataReader.ReadBytes(sampleCountWChannels);
                                for (int i = 0; i < byteSamples.Length; i++)
                                {
                                    float sample = Wave.Common.ByteToFloatTable[byteSamples[i]];
                                    samples[i] = sample;
                                }
                            }
                            break;
                        case 16:
                            {
                                byte[] sampleData = sampleDataReader.ReadBytes((sampleCountWChannels) * 2);
                                for (int i = 0; i < sampleCountWChannels; i++)
                                {
                                    int sampleIndex = i * 2;
                                    short sample = (short)(sampleData[sampleIndex] | (sampleData[sampleIndex + 1] << 8));
                                    samples[i] = sample / 32767f;
                                }
                            }
                            break;
                        case 24:
                            {
                                byte[] sampleData = sampleDataReader.ReadBytes((sampleCountWChannels) * 3);
                                for (int i = 0; i < sampleCountWChannels; i++)
                                {
                                    int sampleIndex = i * 3;
                                    float sample = (sampleData[sampleIndex] << 8 | sampleData[sampleIndex + 1] << 16 | sampleData[sampleIndex + 2] << 24) / 2147483648f;
                                    samples[i] = sample;
                                }
                            }
                            break;
                        case 32:
                            {
                                if (format.wFormatTag == WaveFormatTag.IEEE_FLOAT)
                                {
                                    for (int i = 0; i < sampleCountWChannels; i++)
                                        samples[i] = sampleDataReader.ReadSingle();
                                }
                                else
                                {
                                    byte[] sampleData = sampleDataReader.ReadBytes((sampleCountWChannels) * 4);
                                    for (int i = 0; i < sampleCountWChannels; i++)
                                    {
                                        int sampleIndex = i * 4;
                                        float sample = (sampleData[sampleIndex] | sampleData[sampleIndex + 1] << 8 | sampleData[sampleIndex + 2] << 16 | sampleData[sampleIndex + 3]  << 24) / 2147483648f;
                                        samples[i] = sample;
                                    }
                                }
                            }
                            break;
                        default:
                            Debug.LogError($"Unsupported bits per sample {format.wBitsPerSample}.");
                            return null;
                    }
                }
            }

            // return read wave data as audioclip
            if (samples == null)
                throw new Exception("File had no data chunk!");

            AudioClip clip = AudioClip.Create(Path.GetFileNameWithoutExtension(LastFileName), samples.Length / format.wChannels, format.wChannels, (int)format.dwSamplesPerSec, false);
            clip.SetData(samples, 0);
            return clip;
        }

        public static AudioClip Load(string path)
        {
            LastFileName = path;
            var returnVal = Load(File.OpenRead(path));
            LastFileName = null;
            return returnVal;
        }
    }
}

