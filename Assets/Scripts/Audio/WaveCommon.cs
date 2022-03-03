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

namespace Dummiesman.Wave
{
    public class Common
    {
        public static float[] ByteToFloatTable = new[] { -1f, -0.9921569f, -0.9843137f, -0.9764706f, -0.9686275f, -0.9607843f, -0.9529412f, -0.945098f, -0.9372549f, -0.9294118f, -0.9215686f, -0.9137255f, -0.9058824f, -0.8980392f, -0.8901961f, -0.8823529f, -0.8745098f, -0.8666667f, -0.8588235f, -0.8509804f, -0.8431373f, -0.8352941f, -0.827451f, -0.8196079f, -0.8117647f, -0.8039216f, -0.7960784f, -0.7882353f, -0.7803922f, -0.772549f, -0.7647059f, -0.7568628f, -0.7490196f, -0.7411765f, -0.7333333f, -0.7254902f, -0.7176471f, -0.7098039f, -0.7019608f, -0.6941177f, -0.6862745f, -0.6784314f, -0.6705883f, -0.6627451f, -0.654902f, -0.6470588f, -0.6392157f, -0.6313726f, -0.6235294f, -0.6156863f, -0.6078432f, -0.6f, -0.5921569f, -0.5843138f, -0.5764706f, -0.5686275f, -0.5607843f, -0.5529412f, -0.5450981f, -0.5372549f, -0.5294118f, -0.5215687f, -0.5137255f, -0.5058824f, -0.4980392f, -0.490196f, -0.4823529f, -0.4745098f, -0.4666666f, -0.4588235f, -0.4509804f, -0.4431372f, -0.4352941f, -0.427451f, -0.4196078f, -0.4117647f, -0.4039215f, -0.3960784f, -0.3882353f, -0.3803921f, -0.372549f, -0.3647059f, -0.3568627f, -0.3490196f, -0.3411765f, -0.3333333f, -0.3254902f, -0.317647f, -0.3098039f, -0.3019608f, -0.2941176f, -0.2862745f, -0.2784314f, -0.2705882f, -0.2627451f, -0.2549019f, -0.2470588f, -0.2392157f, -0.2313725f, -0.2235294f, -0.2156863f, -0.2078431f, -0.2f, -0.1921569f, -0.1843137f, -0.1764706f, -0.1686274f, -0.1607843f, -0.1529412f, -0.145098f, -0.1372549f, -0.1294118f, -0.1215686f, -0.1137255f, -0.1058823f, -0.09803921f, -0.09019607f, -0.08235294f, -0.0745098f, -0.06666666f, -0.05882353f, -0.05098039f, -0.04313725f, -0.03529412f, -0.02745098f, -0.01960784f, -0.01176471f, -0.003921568f, 0.003921628f, 0.01176476f, 0.0196079f, 0.02745104f, 0.03529418f, 0.04313731f, 0.05098045f, 0.05882359f, 0.06666672f, 0.07450986f, 0.082353f, 0.09019613f, 0.09803927f, 0.1058824f, 0.1137255f, 0.1215687f, 0.1294118f, 0.137255f, 0.1450981f, 0.1529412f, 0.1607844f, 0.1686275f, 0.1764706f, 0.1843138f, 0.1921569f, 0.2f, 0.2078432f, 0.2156863f, 0.2235295f, 0.2313726f, 0.2392157f, 0.2470589f, 0.254902f, 0.2627451f, 0.2705883f, 0.2784314f, 0.2862746f, 0.2941177f, 0.3019608f, 0.309804f, 0.3176471f, 0.3254902f, 0.3333334f, 0.3411765f, 0.3490196f, 0.3568628f, 0.3647059f, 0.3725491f, 0.3803922f, 0.3882353f, 0.3960785f, 0.4039216f, 0.4117647f, 0.4196079f, 0.427451f, 0.4352942f, 0.4431373f, 0.4509804f, 0.4588236f, 0.4666667f, 0.4745098f, 0.482353f, 0.4901961f, 0.4980392f, 0.5058824f, 0.5137255f, 0.5215687f, 0.5294118f, 0.5372549f, 0.5450981f, 0.5529412f, 0.5607843f, 0.5686275f, 0.5764706f, 0.5843138f, 0.5921569f, 0.6f, 0.6078432f, 0.6156863f, 0.6235294f, 0.6313726f, 0.6392157f, 0.6470588f, 0.654902f, 0.6627451f, 0.6705883f, 0.6784314f, 0.6862745f, 0.6941177f, 0.7019608f, 0.7098039f, 0.7176471f, 0.7254902f, 0.7333333f, 0.7411765f, 0.7490196f, 0.7568628f, 0.7647059f, 0.772549f, 0.7803922f, 0.7882353f, 0.7960784f, 0.8039216f, 0.8117647f, 0.8196079f, 0.827451f, 0.8352941f, 0.8431373f, 0.8509804f, 0.8588235f, 0.8666667f, 0.8745098f, 0.8823529f, 0.8901961f, 0.8980392f, 0.9058824f, 0.9137255f, 0.9215686f, 0.9294118f, 0.9372549f, 0.945098f, 0.9529412f, 0.9607843f, 0.9686275f, 0.9764706f, 0.9843137f, 0.9921569f, 1f };
    }

    public enum WaveFormatTag : ushort
    {
        UNKNOWN = 0x0000, /* Unknown Format */
        PCM = 0x0001, /* PCM */
        ADPCM = 0x0002, /* Microsoft ADPCM Format */
        IEEE_FLOAT = 0x0003, /* IEEE Float */
        VSELP = 0x0004, /* Compaq Computer's VSELP */
        IBM_CSVD = 0x0005, /* IBM CVSD */
        ALAW = 0x0006, /* ALAW */
        MULAW = 0x0007, /* MULAW */
        OKI_ADPCM = 0x0010, /* OKI ADPCM */
        DVI_ADPCM = 0x0011, /* Intel's DVI ADPCM aka IMA */
        MEDIASPACE_ADPCM = 0x0012, /* Videologic's MediaSpace ADPCM*/
        SIERRA_ADPCM = 0x0013, /* Sierra ADPCM */
        G723_ADPCM = 0x0014, /* G.723 ADPCM */
        DIGISTD = 0x0015, /* DSP Solution's DIGISTD */
        DIGIFIX = 0x0016, /* DSP Solution's DIGIFIX */
        DIALOGIC_OKI_ADPCM = 0x0017, /* Dialogic OKI ADPCM */
        MEDIAVISION_ADPCM = 0x0018, /* MediaVision ADPCM */
        CU_CODEC = 0x0019, /* HP CU */
        YAMAHA_ADPCM = 0x0020, /* Yamaha ADPCM */
        SONARC = 0x0021, /* Speech Compression's Sonarc */
        TRUESPEECH = 0x0022, /* DSP Group's True Speech */
        ECHOSC1 = 0x0023, /* Echo Speech's EchoSC1 */
        AUDIOFILE_AF36 = 0x0024, /* Audiofile AF36 */
        APTX = 0x0025, /* APTX */
        AUDIOFILE_AF10 = 0x0026, /* AudioFile AF10 */
        PROSODY_1612 = 0x0027, /* Prosody 1612 */
        LRC = 0x0028, /* LRC */
        AC2 = 0x0030, /* Dolby AC2 */
        GSM610 = 0x0031, /* GSM610 */
        MSNAUDIO = 0x0032, /* MSNAudio */
        ANTEX_ADPCME = 0x0033, /* Antex ADPCME */
        CONTROL_RES_VQLPC = 0x0034, /* Control Res VQLPC */
        DIGIREAL = 0x0035, /* Digireal */
        DIGIADPCM = 0x0036, /* DigiADPCM */
        CONTROL_RES_CR10 = 0x0037, /* Control Res CR10 */
        VBXADPCM = 0x0038, /* NMS VBXADPCM */
        ROLAND_RDAC = 0x0039, /* Roland RDAC */
        ECHOSC3 = 0x003A, /* EchoSC3 */
        ROCKWELL_ADPCM = 0x003B, /* Rockwell ADPCM */
        ROCKWELL_DIGITALK = 0x003C, /* Rockwell Digit LK */
        XEBEC = 0x003D, /* Xebec */
        G721_ADPCM = 0x0040, /* Antex Electronics G.721 */
        G728_CELP = 0x0041, /* G.728 CELP */
        MSG723 = 0x0042, /* MSG723 */
        MPEG = 0x0050, /* MPEG Layer 1,2 */
        RT24 = 0x0051, /* RT24 */
        PAC = 0x0051, /* PAC */
        MPEGLAYER3 = 0x0055, /* MPEG Layer 3 */
        CIRRUS = 0x0059, /* Cirrus */
        ESPCM = 0x0061, /* ESPCM */
        VOXWARE = 0x0062, /* Voxware (obsolete) */
        CANOPUS_ATRAC = 0x0063, /* Canopus Atrac */
        G726_ADPCM = 0x0064, /* G.726 ADPCM */
        G722_ADPCM = 0x0065, /* G.722 ADPCM */
        DSAT = 0x0066, /* DSAT */
        DSAT_DISPLAY = 0x0067, /* DSAT Display */
        VOXWARE_BYTE_ALIGNED = 0x0069, /* Voxware Byte Aligned (obsolete) */
        VOXWARE_AC8 = 0x0070, /* Voxware AC8 (obsolete) */
        VOXWARE_AC10 = 0x0071, /* Voxware AC10 (obsolete) */
        VOXWARE_AC16 = 0x0072, /* Voxware AC16 (obsolete) */
        VOXWARE_AC20 = 0x0073, /* Voxware AC20 (obsolete) */
        VOXWARE_RT24 = 0x0074, /* Voxware MetaVoice (obsolete) */
        VOXWARE_RT29 = 0x0075, /* Voxware MetaSound (obsolete) */
        VOXWARE_RT29HW = 0x0076, /* Voxware RT29HW (obsolete) */
        VOXWARE_VR12 = 0x0077, /* Voxware VR12 (obsolete) */
        VOXWARE_VR18 = 0x0078, /* Voxware VR18 (obsolete) */
        VOXWARE_TQ40 = 0x0079, /* Voxware TQ40 (obsolete) */
        SOFTSOUND = 0x0080, /* Softsound */
        VOXWARE_TQ60 = 0x0081, /* Voxware TQ60 (obsolete) */
        MSRT24 = 0x0082, /* MSRT24 */
        G729A = 0x0083, /* G.729A */
        MVI_MV12 = 0x0084, /* MVI MV12 */
        DF_G726 = 0x0085, /* DF G.726 */
        DF_GSM610 = 0x0086, /* DF GSM610 */
        ISIAUDIO = 0x0088, /* ISIAudio */
        ONLIVE = 0x0089, /* Onlive */
        SBC24 = 0x0091, /* SBC24 */
        DOLBY_AC3_SPDIF = 0x0092, /* Dolby AC3 SPDIF */
        ZYXEL_ADPCM = 0x0097, /* ZyXEL ADPCM */
        PHILIPS_LPCBB = 0x0098, /* Philips LPCBB */
        PACKED = 0x0099, /* Packed */
        RHETOREX_ADPCM = 0x0100, /* Rhetorex ADPCM */
        IRAT = 0x0101, /* BeCubed Software's IRAT */
        VIVO_G723 = 0x0111, /* Vivo G.723 */
        VIVO_SIREN = 0x0112, /* Vivo Siren */
        DIGITAL_G723 = 0x0123, /* Digital G.723 */
        CREATIVE_ADPCM = 0x0200, /* Creative ADPCM */
        CREATIVE_FASTSPEECH8 = 0x0202, /* Creative FastSpeech8 */
        CREATIVE_FASTSPEECH10 = 0x0203, /* Creative FastSpeech10 */
        QUARTERDECK = 0x0220, /* Quarterdeck */
        FM_TOWNS_SND = 0x0300, /* FM Towns Snd */
        BTV_DIGITAL = 0x0400, /* BTV Digital */
        VME_VMPCM = 0x0680, /* VME VMPCM */
        OLIGSM = 0x1000, /* OLIGSM */
        OLIADPCM = 0x1001, /* OLIADPCM */
        OLICELP = 0x1002, /* OLICELP */
        OLISBC = 0x1003, /* OLISBC */
        OLIOPR = 0x1004, /* OLIOPR */
        LH_CODEC = 0x1100, /* LH Codec */
        NORRIS = 0x1400, /* Norris */
        SOUNDSPACE_MUSICOMPRESS = 0x1500, /* Soundspace Music Compression */
        DVM = 0x2000, /* DVM */
        EXTENSIBLE = 0xFFFE, /* SubFormat */
        DEVELOPMENT = 0xFFFF /* Development */
    }

    public struct WaveFormatEx
    {
        /// A number indicating the WAVE format category of the file.
        /// The content of the <format-specific-fields> portion of the fmt chunk,
        ///  and the interpretation of the waveform data, depend on this value.
        /// DLS Level 1 only supports WAVE_FORMAT_PCM(0x0001) Microsoft Pulse Code Modulation(PCM) format
        public WaveFormatTag wFormatTag;

        /// The number of channels represented in the waveform data,
        ///  such as 1 for mono or 2 for stereo.DLS Level 1 supports only mono data(value = "1").
        public ushort wChannels;

        /// The sampling rate (in samples per second) at which each channel should be played.
        public uint dwSamplesPerSec;

        /// The average number of bytes per second at which the waveform data should transferred.
        /// Playback software can estimate the buffer size using this value.
        public uint dwAvgBytesPerSec;

        /// The block alignment (in bytes) of the waveform data.
        /// Playback software needs to process a multiple of
        ///  wBlockAlign bytes of data at a time, so the value of
        ///  wBlockAlign can be used for buffer alignment.
        public ushort wBlockAlign;

        /// Specifies the number of bits of data used to represent each sample of each channel.
        /// If there are multiple channels, the sample size is the same for each channel.
        /// DLS level 1 supports only 8 or 16 bit samples.
        public ushort wBitsPerSample;

        /// Size, in bytes, of extra format information appended to the end of the WAVEFORMATEX structure.
        /// This information can be used by non - PCM formats to store extra attributes for the wFormatTag.
        /// If no extra information is required by the wFormatTag, this member must be set to zero.
        /// For WAVE_FORMAT_PCM formats only, this member is ignored.
        public ushort cbSize;

        public static int SizeOf => 18;

        public WaveFormatEx(BinaryReader reader)
        {
            wFormatTag = (WaveFormatTag)reader.ReadUInt16();
            wChannels = reader.ReadUInt16();
            dwSamplesPerSec = reader.ReadUInt32();
            dwAvgBytesPerSec = reader.ReadUInt32();
            wBlockAlign = reader.ReadUInt16();
            wBitsPerSample = reader.ReadUInt16();
            cbSize = (reader.BaseStream.Length > 16) ? reader.ReadUInt16() : default;
        }
    }

}