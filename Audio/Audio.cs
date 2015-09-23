using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Claw.Audio.Formats;

namespace Claw.Audio
{
    public class Audio
    {
        const byte CODEC_VERSION = 5;

        /// <summary>
        /// Title of the sound file
        /// </summary>
        public string Title;

        /// <summary>
        /// Author of the sound file
        /// </summary>
        public string Author;

        /// <summary>
        /// Comment to the sound file
        /// </summary>
        public string Comment;

        /// <summary>
        /// Samples per second
        /// </summary>
        public ushort Speed;

        /// <summary>
        /// Samples data
        /// </summary>
        public byte[] Samples;

        /// <summary>
        /// Total samples
        /// </summary>
        public uint Length
        {
            get { return (uint)Samples.Length / 2; }
        }

        /// <summary>
        /// Total duration in seconds
        /// </summary>
        public float Duration
        {
            get { return (float)Length / Speed; }
        }

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="Title">Title of the sound file</param>
        /// <param name="Author">Author of the sound file</param>
        /// <param name="Speed">Samples per Second</param>
        /// <param name="Samples">Samples data</param>
        public Audio(string Title, string Author, ushort Speed, byte[] Samples)
        {
            this.Title = Title.Replace("\r\n", "\n");
            this.Author = Author.Replace("\r\n", "\n");
            this.Comment = "";
            this.Speed = Speed;
            this.Samples = Samples;
        }

        /// <summary>
        /// Minimal constructor
        /// </summary>
        /// <param name="Speed">Samples per Second</param>
        /// <param name="Samples">Samples data</param>
        public Audio(ushort Speed, byte[] Samples)
        {
            this.Title = "";
            this.Author = "";
            this.Comment = "";
            this.Speed = Speed;
            this.Samples = Samples;
        }

        /// <summary>
        /// Full constructor
        /// </summary>
        /// <param name="Title">Title of the sound file</param>
        /// <param name="Author">Author of the sound file</param>
        /// <param name="Comment">Comment to the sound file</param>
        /// <param name="Speed">Samples per Second</param>
        /// <param name="Samples">Samples data</param>
        public Audio(string Title, string Author, string Comment, ushort Speed, byte[] Samples)
        {
            this.Title = Title.Replace("\r\n", "\n");
            this.Author = Author.Replace("\r\n", "\n");
            this.Comment = Comment.Replace("\r\n", "\n");
            this.Speed = Speed;
            this.Samples = Samples;
        }

        /// <summary>
        /// Load audio from input stream
        /// </summary>
        /// <param name="InputStream">Input stream</param>
        public static Audio FromStream(Stream InputStream)
        {
            // Create a binary reader
            var input = new BinaryReader(InputStream);

            // Check magic numbers
            char[] magic = new char[4];
            input.Read(magic, 0, 4);
            if(! (magic[0] == 0x5 && magic[1] == 'C' && magic[2] == 'A' && magic[3] == 'U'))
                return null; // invalid maginc number or not start of data

            // Get version of the file format
            byte version = input.ReadByte();
            if (version != CODEC_VERSION)
                return null; // invalid codec version

            // Get length of title
            byte titleLength = input.ReadByte();
            // Create title string
            string title = "";
            // Check if length is > 0
            if (titleLength > 0)
            {
                // Allocate space for title
                char[] titleBytes = new char[titleLength];
                // Read the title
                input.Read(titleBytes, 0, titleLength);
                // Convert to string object
                title = new string(titleBytes);
            }

            // Get length of author
            byte authorLength = input.ReadByte();
            // Create author string
            string author = "";
            // Check if length is > 0
            if (authorLength > 0)
            {
                // Allocate space for author
                char[] authorBytes = new char[authorLength];
                // Read the author
                input.Read(authorBytes, 0, authorLength);
                // Convert to string object
                author = new string(authorBytes);
            }

            // Get length of comment
            byte commentLength = input.ReadByte();
            // Create comment string
            string comment = "";
            // Check if length is > 0
            if (commentLength > 0)
            {
                // Allocate space for comment
                char[] commentBytes = new char[commentLength];
                // Read the comment
                input.Read(commentBytes, 0, commentLength);
                // Convert to string object
                comment = new string(commentBytes);
            }

            // Get speed
            ushort speed = input.ReadUInt16();

            // Get number of samples
            uint length = input.ReadUInt32();

            // Allocate space for the samples
            byte[] samples = new byte[length * 2];
            // Read the samples
            for (uint i = 0; i < samples.Length; i++)
                samples[i] = input.ReadByte();

            // Create new audio object
            var result = new Audio(title, author, comment, speed, samples);

            // Return the result
            return result;
        }

        /// <summary>
        /// Compile the audio raw data and save it to a stream
        /// </summary>
        /// <param name="OutputStream">Output stream</param>
        public uint ToStream(Stream OutputStream)
        {
            // Save initial output stream position
            long streamPosition = OutputStream.Position;

            // Create a binary writer
            var output = new BinaryWriter(OutputStream);

            // Write magic numbers
            output.Write((byte)0x5);
            output.Write('C');
            output.Write('A');
            output.Write('U');

            // Write audio version
            output.Write((byte)CODEC_VERSION);

            // Write the title and truncate string to max 255 characters
            output.Write((byte)(Title.Length & 0xFF));
            output.Write(Title.Substring(0, Title.Length & 0xFF).ToCharArray());

            // Write the author and truncate string to max 255 characters
            output.Write((byte)(Author.Length & 0xFF));
            output.Write(Author.Substring(0, Author.Length & 0xFF).ToCharArray());

            // Write the comment and truncate string to max 255 characters
            output.Write((byte)(Comment.Length & 0xFF));
            output.Write(Comment.Substring(0, Comment.Length & 0xFF).ToCharArray());

            // Write number of samples per second
            output.Write((ushort)Speed);

            // Write total number of samples
            output.Write((uint)Length);

            // Write the samples
            foreach (byte sample in Samples)
                output.Write((byte)sample);

            // Return the final size
            return (uint)(OutputStream.Position - streamPosition);
        }

        /// <summary>
        /// Load audio from a wave object
        /// </summary>
        /// <param name="WaveObject">Wave object</param>
        /// <returns></returns>
        public static Audio FromWave(Wave WaveObject)
        {
            return FromWave("", "", WaveObject);
        }

        /// <summary>
        /// Load audio from a wave object
        /// </summary>
        /// <param name="Title">Title of the audio</param>
        /// <param name="Author">Author of the audio</param>
        /// <param name="WaveObject">Wave object</param>
        /// <returns></returns>
        public static Audio FromWave(string Title, string Author, Wave WaveObject)
        {
            return FromWave(Title, Author, "", WaveObject);
        }

        /// <summary>
        /// Load audio from a wave object
        /// </summary>
        /// <param name="Title">Title of the audio</param>
        /// <param name="Author">Author of the audio</param>
        /// <param name="Comment">Comment to the audio</param>
        /// <param name="WaveObject">Wave object</param>
        /// <returns></returns>
        public static Audio FromWave(string Title, string Author, string Comment, Wave WaveObject)
        {
            // Get speed
            ushort speed = (ushort)WaveObject.FileFormatSubChunk.SampleRate;

            // Allocate space for the samples
            byte[] samples = new byte[WaveObject.FileDataSubChunk.Data.Length / WaveObject.FileFormatSubChunk.NumChannels];
            
            // Allocate counter variables
            uint sourceSample = 0;
            uint targetSample = 0;
            byte byteOfSample = 0;

            // Copy over samples (only channel 0 - ignoring other channels)
            while (targetSample < samples.Length) {
                samples[targetSample] = WaveObject.FileDataSubChunk.Data[sourceSample]; // copy wave sample over
                targetSample++; // advance target pointer

                if (byteOfSample == 0) {
                    sourceSample++; // advance source pointer
                    byteOfSample++; // advance byte of sample pointer
                } else {
                    sourceSample += (ushort)((WaveObject.FileFormatSubChunk.NumChannels - 1) * 2 + 1); // 1 channel (mono) is output, therefore skip all samples of the other channels
                    byteOfSample = 0; // reset byte of sample pointer and start with the next sample
                }
            }

            // Return new audio object
            return new Audio(Title, Author, Comment, speed, samples);
        }

        private uint CalculateCount(byte[] Samples)
        {
            return (uint)Samples.Length / 2;
        }

        /// <summary>
        /// Create a full copy of the audio in RAM
        /// </summary>
        public Audio Copy()
        {
            byte[] samples = new byte[this.Samples.Length];
            this.Samples.CopyTo(samples, 0);
            return new Audio(this.Title, this.Author, this.Comment, this.Speed, samples);
        }

        /// <summary>
        /// Create a wave object
        /// </summary>
        /// <returns>Wave file object</returns>
        public Wave ToWave()
        {
            // Create a new wave meta
            var waveStruct = new Wave.WaveFileOptions();

            // Set the constant properties
            waveStruct.AudioFormat = Wave.Format.Standard;
            waveStruct.FormatSize = Wave.FormatSize.PCM;
            waveStruct.NumberOfChannels = Wave.NumberOfChannels.Mono;

            // Set the dynamic properties
            waveStruct.SampleRate = (Wave.WavSampleRate)Speed;
            waveStruct.BitsPerSample = Wave.BitsPerSample.bps_16;
            waveStruct.NumberOfSamples = Length;

            // Copy data over to new array
            waveStruct.Data = new byte[Samples.Length];
            for (uint i = 0; i < Samples.Length; i++)
                waveStruct.Data[i] = Samples[i];

            // Return new wave object
            return new Wave(waveStruct);
        }
    }
}
