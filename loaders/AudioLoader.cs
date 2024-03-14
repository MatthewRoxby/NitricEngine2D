using NAudio.Wave;
using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitricEngine2D.loaders
{
    /// <summary>
    /// a static loader class for loading .WAV audio files
    /// </summary>
    public static class AudioLoader
    {
        //classes needed for OpenAL to run
        private static ALDevice? device = null;
        private static ALContext? context = null;

        //a cache for all loaded sounds by file path
        private static Dictionary<string, int> cachedSounds = new Dictionary<string, int>();

        /// <summary>
        /// Initialises OpenAL if not done so already. Must be unsafe because of weird pointer code
        /// </summary>
        private static unsafe void Init()
        {
            device = ALC.OpenDevice(null);
            context = ALC.CreateContext(device.Value, (int*)null);

            //Debug.WriteLine(context != null);
            ALC.MakeContextCurrent(context.Value);

            AL.DistanceModel(ALDistanceModel.ExponentDistanceClamped);
        }

        /// <summary>
        /// Returns an ALFormat from wave data. NOTE that stereo audio will not be attenuated
        /// </summary>
        /// <param name="channels">number of audio channels. 1 for Mono, 2 for Stereo</param>
        /// <param name="bits">number of bits per sample</param>
        /// <returns>An OpenTK.Audio.OpenAL.ALFormat for the given channels and bits</returns>
        /// <exception cref="NotSupportedException"></exception>
        private static ALFormat GetSoundFormat(int channels, int bits)
        {
            switch (channels)
            {
                case 1: return bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
                case 2: return bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
                default: throw new NotSupportedException("The specified sound format is not supported.");
            }
        }

        /// <summary>
        /// Loads a wav file as an OpenAL buffer
        /// </summary>
        /// <param name="filePath">the path to the audio file</param>
        /// <returns>the buffer ID as an integer</returns>
        /// <exception cref="Exception">will cause an exception if the file does not contain a riff or wave segment</exception>
        public static int LoadWav(string filePath)
        {
            //initialise OpenAL if not done so already
            if (device == null || context == null) Init();

            //return the cached sound if it has been loaded already
            if (cachedSounds.ContainsKey(filePath))
            {
                return cachedSounds[filePath];
            }
            //the following is NOT MY CODE, and is attributed to piegfx on github
            // A port of my rust wav loader https://github.com/piegfx/mixr/blob/master/mixr/src/stream.rs#L46

            const uint riff = 0x46464952;
            const uint wave = 0x45564157;
            const uint fmt = 0x20746D66;
            const uint data = 0x61746164;

            using BinaryReader reader = new BinaryReader(File.OpenRead(filePath));

            if (reader.ReadUInt32() != riff)
                throw new Exception("Given file is not a valid wave file! (Missing RIFF header).");

            reader.ReadUInt32(); // File size

            if (reader.ReadUInt32() != wave)
                throw new Exception("Given file is not a valid wave file! (Missing WAVE header).");

            byte[] pcmData = null;

            ALFormat format = ALFormat.Mono8;

            ushort channels;

            uint sampleRate = 0;

            while (true)
            {
                switch (reader.ReadUInt32())
                {
                    case fmt:
                        if (reader.ReadUInt32() != 16)
                            throw new Exception("Malformed fmt header.");

                        ushort formatType = reader.ReadUInt16();

                        channels = reader.ReadUInt16();

                        sampleRate = reader.ReadUInt32();

                        reader.ReadUInt32(); // bytes per second
                        reader.ReadUInt16(); // bytes per sample

                        ushort bitsPerSample = reader.ReadUInt16();

                        if (formatType != 1)
                        {
                            throw new Exception("format type is not integer!");
                        }

                        format = GetSoundFormat(channels, bitsPerSample);

                        break;

                    case data:
                        uint dataSize = reader.ReadUInt32();

                        pcmData = reader.ReadBytes((int)dataSize);

                        goto FINISH;
                }
            }

        FINISH:;

            Debug.WriteLine($"loading audio file of type {format}");
            int buffer = AL.GenBuffer();

            AL.BufferData(buffer, format, pcmData, (int)sampleRate);

            cachedSounds.Add(filePath, buffer);
            return buffer;
        }

        /// <summary>
        /// Deletes all cached buffers
        /// </summary>
        public static void CleanUp()
        {
            foreach(int i in cachedSounds.Values)
            {
                AL.DeleteBuffer(i);
            }
        }
    }
}
