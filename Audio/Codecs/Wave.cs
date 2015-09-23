using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Media;

namespace Claw.Audio.Codecs
{
    public class Wave
    {
        //By Paul Ishak
        //WAVE PCM soundfile format
        //The Canonical WAVE file format
        //As Described Here: https://ccrma.stanford.edu/courses/422/projects/WaveFormat/ 
        //The File's Header
        public Header FileHeader;
        //Wave File's Format Sub Chunk
        public FormatSubChunk FileFormatSubChunk;
        //Data Subchunk
        public DataSubChunk FileDataSubChunk;

        private SoundPlayer simpleSound;

        //This structure is an optional parameter for creating a new wave file
        public struct WaveFileOptions
        {
            public WavSampleRate SampleRate;
            public Format AudioFormat;
            public BitsPerSample BitsPerSample;
            public NumberOfChannels NumberOfChannels;
            public FormatSize FormatSize;
            public UInt32 NumberOfSamples;
            public byte[] Data;
        }

        //These are the various structures in the wave file and their description
        //                                               DATATYPE          OFFSET        Endian           Description
        public struct Header
        {
            public byte[] ChunkID { get; set; }
            //          Dword              0             Big            Contains the letters "RIFF" in ASCII form(0x52494646 big-endian form).
            public uint ChunkSize { get; set; }
            //        Dword              4             Little         36 + SubChunk2Size, or more precisely: 4 + (8 + SubChunk1Size) + (8 + SubChunk2Size)
            public byte[] Format { get; set; }
            //           Dword              8             Big            Contains the letters "WAVE" in ASCII form (0x57415645 big-endian form).
        }

        public struct FormatSubChunk
        {
            public byte[] Subchunk1ID { get; set; }
            //      Dword              12            Big            Contains the letters "fmt "(0x666d7420 big-endian form).
            public uint Subchunk1Size { get; set; }
            //    Dword              16            little         16 for PCM.  This is the size of the rest of the Subchunk which follows this number.
            public ushort AudioFormat { get; set; }
            //     Word               20            little         PCM = 1 (i.e. Linear quantization)Values other than 1 indicate some form of compression.
            public ushort NumChannels { get; set; }
            //      Word               22            little         Mono = 1, Stereo = 2, etc.
            public uint SampleRate { get; set; }
            //       Dword              24            little         8000, 44100, etc.
            public uint ByteRate { get; set; }
            //         Dword              28            little         == SampleRate * NumChannels * BitsPerSample/8
            public ushort BlockAlign { get; set; }
            //       Word               32            little         == NumChannels * BitsPerSample/8
            public ushort BitsPerSample { get; set; }
            //    Word               34            little         8 bits = 8, 16 bits = 16, etc.
        }

        public struct DataSubChunk
        {
            public byte[] Subchunk2ID { get; set; }
            //      Dword              36            Big            Contains the letters "data"(0x64617461 big-endian form).
            public uint Subchunk2Size { get; set; }
            //    Dword              40            little         == NumSamples * NumChannels * BitsPerSample/8     This is the number of bytes in the data.
            public byte[] Data { get; set; }
            //             VariableLength     44            little         The actual sound data.
        }

        public void Open(string FileName)
        {
            if (!File.Exists(FileName))
                return;
            Open(File.ReadAllBytes(FileName));
        }

        public void Open(byte[] FileBytes)
        {
            try
            {
                this.FileHeader.ChunkID = GetDataFromByteArray(FileBytes, 0, 0, 4);
                this.FileHeader.ChunkSize = BitConverter.ToUInt32(FileBytes, 4);
                this.FileHeader.Format = GetDataFromByteArray(FileBytes, 0, 8, 4);
                this.FileFormatSubChunk.Subchunk1ID = GetDataFromByteArray(FileBytes, 0, 12, 4);
                this.FileFormatSubChunk.Subchunk1Size = BitConverter.ToUInt32(FileBytes, 16);
                this.FileFormatSubChunk.AudioFormat = BitConverter.ToUInt16(FileBytes, 20);
                this.FileFormatSubChunk.NumChannels = BitConverter.ToUInt16(FileBytes, 22);
                this.FileFormatSubChunk.SampleRate = BitConverter.ToUInt32(FileBytes, 24);
                this.FileFormatSubChunk.ByteRate = BitConverter.ToUInt32(FileBytes, 28);
                this.FileFormatSubChunk.BlockAlign = BitConverter.ToUInt16(FileBytes, 32);
                this.FileFormatSubChunk.BitsPerSample = BitConverter.ToUInt16(FileBytes, 34);
                this.FileDataSubChunk.Subchunk2ID = GetDataFromByteArray(FileBytes, 0, 36, 4);
                this.FileDataSubChunk.Subchunk2Size = BitConverter.ToUInt32(FileBytes, 40);
                this.FileDataSubChunk.Data = GetDataFromByteArray(FileBytes, 0, 44, this.FileDataSubChunk.Subchunk2Size);
            }
            catch
            {
                throw new Exception("File Is Invalid or corrupt!");
            }
        }

        public static Wave FromFile(string FileName)
        {
            Wave Result = new Wave();
            if (!File.Exists(FileName))
                return Result;
            byte[] FileBytes = File.ReadAllBytes(FileName);
            try
            {
                Result.FileHeader.ChunkID = GetDataFromByteArray(FileBytes, 0, 0, 4);
                Result.FileHeader.ChunkSize = BitConverter.ToUInt32(FileBytes, 4);
                Result.FileHeader.Format = GetDataFromByteArray(FileBytes, 0, 8, 4);
                Result.FileFormatSubChunk.Subchunk1ID = GetDataFromByteArray(FileBytes, 0, 12, 4);
                Result.FileFormatSubChunk.Subchunk1Size = BitConverter.ToUInt32(FileBytes, 16);
                Result.FileFormatSubChunk.AudioFormat = BitConverter.ToUInt16(FileBytes, 20);
                Result.FileFormatSubChunk.NumChannels = BitConverter.ToUInt16(FileBytes, 22);
                Result.FileFormatSubChunk.SampleRate = BitConverter.ToUInt32(FileBytes, 24);
                Result.FileFormatSubChunk.ByteRate = BitConverter.ToUInt32(FileBytes, 28);
                Result.FileFormatSubChunk.BlockAlign = BitConverter.ToUInt16(FileBytes, 32);
                Result.FileFormatSubChunk.BitsPerSample = BitConverter.ToUInt16(FileBytes, 34);
                Result.FileDataSubChunk.Subchunk2ID = GetDataFromByteArray(FileBytes, 0, 36, 4);
                Result.FileDataSubChunk.Subchunk2Size = BitConverter.ToUInt32(FileBytes, 40);
                Result.FileDataSubChunk.Data = GetDataFromByteArray(FileBytes, 0, 44, Result.FileDataSubChunk.Subchunk2Size);
                return Result;
            }
            catch
            {
                return null;
            }
        }

        public static Wave FromArray(byte[] Bytes)
        {
            Wave Result = new Wave();
            byte[] FileBytes = Bytes;
            if (Bytes.Length == 0)
                return Result;
            try
            {
                Result.FileHeader.ChunkID = GetDataFromByteArray(FileBytes, 0, 0, 4);
                Result.FileHeader.ChunkSize = BitConverter.ToUInt32(FileBytes, 4);
                Result.FileHeader.Format = GetDataFromByteArray(FileBytes, 0, 8, 4);
                Result.FileFormatSubChunk.Subchunk1ID = GetDataFromByteArray(FileBytes, 0, 12, 4);
                Result.FileFormatSubChunk.Subchunk1Size = BitConverter.ToUInt32(FileBytes, 16);
                Result.FileFormatSubChunk.AudioFormat = BitConverter.ToUInt16(FileBytes, 20);
                Result.FileFormatSubChunk.NumChannels = BitConverter.ToUInt16(FileBytes, 22);
                Result.FileFormatSubChunk.SampleRate = BitConverter.ToUInt32(FileBytes, 24);
                Result.FileFormatSubChunk.ByteRate = BitConverter.ToUInt32(FileBytes, 28);
                Result.FileFormatSubChunk.BlockAlign = BitConverter.ToUInt16(FileBytes, 32);
                Result.FileFormatSubChunk.BitsPerSample = BitConverter.ToUInt16(FileBytes, 34);
                Result.FileDataSubChunk.Subchunk2ID = GetDataFromByteArray(FileBytes, 0, 36, 4);
                Result.FileDataSubChunk.Subchunk2Size = BitConverter.ToUInt32(FileBytes, 40);
                Result.FileDataSubChunk.Data = GetDataFromByteArray(FileBytes, 0, 44, Result.FileDataSubChunk.Subchunk2Size);
                return Result;
            }
            catch
            {
                Result = new Wave();
                return Result;
            }
        }

        public byte[] ToArray()
        {
            byte[] Results = null;
            Results = CombineArrays(FileHeader.ChunkID, BitConverter.GetBytes(FileHeader.ChunkSize));
            Results = CombineArrays(Results, FileHeader.Format);
            Results = CombineArrays(Results, FileFormatSubChunk.Subchunk1ID);
            Results = CombineArrays(Results, BitConverter.GetBytes(FileFormatSubChunk.Subchunk1Size));
            Results = CombineArrays(Results, BitConverter.GetBytes(FileFormatSubChunk.AudioFormat));
            Results = CombineArrays(Results, BitConverter.GetBytes(FileFormatSubChunk.NumChannels));
            Results = CombineArrays(Results, BitConverter.GetBytes(FileFormatSubChunk.SampleRate));
            Results = CombineArrays(Results, BitConverter.GetBytes(FileFormatSubChunk.ByteRate));
            Results = CombineArrays(Results, BitConverter.GetBytes(FileFormatSubChunk.BlockAlign));
            Results = CombineArrays(Results, BitConverter.GetBytes(FileFormatSubChunk.BitsPerSample));
            Results = CombineArrays(Results, FileDataSubChunk.Subchunk2ID);
            Results = CombineArrays(Results, BitConverter.GetBytes(FileDataSubChunk.Subchunk2Size));
            Results = CombineArrays(Results, FileDataSubChunk.Data);
            return Results;
        }

        public Exception Play()
        {
            try
            {
                using (MemoryStream memstr = new MemoryStream(this.ToArray()))
                {
                    memstr.Position = 0;
                    simpleSound = new SoundPlayer(memstr);

                    simpleSound.Play();
                }

                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        public void Stop()
        {
            if (simpleSound != null)
                simpleSound.Stop();
        }

        public MemoryStream ToStream()
        {
            MemoryStream stream = new MemoryStream(this.ToArray());
            stream.Position = 0;

            return stream;
        }

        public void Save(string FileName)
        {
            File.WriteAllBytes(FileName, this.ToArray());
        }

        public byte[] CombineArrays(byte[] Array1, byte[] Array2)
        {
            byte[] AllResults = new byte[Array1.Length + Array2.Length];
            Array1.CopyTo(AllResults, 0);
            Array2.CopyTo(AllResults, Array1.Length);
            return AllResults;
        }

        private static byte[] GetDataFromByteArray(byte[] ByteArray, long BlockOffset, long RangeStartOffset, long DataLength)
        {
            List<byte> AnswerL = new List<byte>();
            byte[] Answer = new byte[Convert.ToInt32((DataLength - 1)) + 1];
            long CurrentOffset = 0;
            for (uint I = 0; I <= ByteArray.GetUpperBound(0); I++)
            {
                CurrentOffset = BlockOffset + I;
                if (CurrentOffset >= RangeStartOffset)
                {
                    if (CurrentOffset <= RangeStartOffset + DataLength)
                    {
                        AnswerL.Add(ByteArray[I]);
                    }
                }
            }
            int count = 0;
            foreach (byte bt in AnswerL)
            {
                if (count < Answer.Length)
                    Answer[count++] = bt;
            }
            return Answer;
        }

        public Wave(WaveFileOptions Options)
        {
            try
            {
                FileHeader.ChunkID = Encoding.ASCII.GetBytes("RIFF");
                FileFormatSubChunk.Subchunk1Size = (uint)Options.FormatSize;
                FileFormatSubChunk.NumChannels = (ushort)Options.NumberOfChannels;
                FileFormatSubChunk.BitsPerSample = (ushort)Options.BitsPerSample;
                FileDataSubChunk.Subchunk2Size = Convert.ToUInt32(Options.NumberOfSamples * ((uint)Options.NumberOfChannels) * ((uint)Options.BitsPerSample) / 8);
                FileHeader.ChunkSize = Convert.ToUInt32(4 + (8 + FileFormatSubChunk.Subchunk1Size) + (8 + FileDataSubChunk.Subchunk2Size));
                FileHeader.Format = Encoding.ASCII.GetBytes("WAVE");
                FileFormatSubChunk.Subchunk1ID = Encoding.ASCII.GetBytes("fmt ");
                FileFormatSubChunk.AudioFormat = (ushort)Options.AudioFormat;
                FileFormatSubChunk.SampleRate = (uint)Options.SampleRate;
                FileFormatSubChunk.ByteRate = Convert.ToUInt32((uint)Options.SampleRate * (ushort)Options.NumberOfChannels * (ushort)Options.BitsPerSample / 8);
                FileFormatSubChunk.BlockAlign = Convert.ToUInt16((ushort)Options.NumberOfChannels * (ushort)Options.BitsPerSample / 8);
                FileDataSubChunk.Subchunk2ID = Encoding.ASCII.GetBytes("data");
                FileDataSubChunk.Data = Options.Data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Wave()
        {
        }

        public enum WavSampleRate : uint
        {
            hz8000 = 8000,
            hz11025 = 11025,
            hz16000 = 16000,
            hz22050 = 22050,
            hz32000 = 32000,
            hz44100 = 44100,
            hz48000 = 48000,
            hz96000 = 96000,
            hz192000 = 192000
        }

        public enum Format : ushort
        {
            Standard = 1
        }

        public enum BitsPerSample : ushort
        {
            bps_8 = 8,
            bps_16 = 16,
            bps_32 = 32,
            bps_64 = 64,
            bps_128 = 128,
            bps_256 = 256
        }

        public enum NumberOfChannels : ushort
        {
            Mono = 1,
            Stereo = 2
        }

        public enum FormatSize : uint
        {
            PCM = 16
        }
    }
}