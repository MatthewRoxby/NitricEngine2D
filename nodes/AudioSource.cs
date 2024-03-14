using ImGuiNET;
using NitricEngine2D.loaders;
using OpenTK.Audio.OpenAL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NitricEngine2D.nodes
{
    /// <summary>
    /// A node that creates a 2D or global audio source
    /// </summary>
    public class AudioSource : Node2D
    {
        private int sourceID;
        private int audioID;
        private bool looping;
        private bool autostart;
        public bool globalSound;
        private float volume = 1f;
        private float pitch = 1f;
        private float range = 1f;
        private bool playing;

        /// <summary>
        /// loads an AudioSource node from JSON data
        /// </summary>
        /// <param name="data">the JSON element</param>
        public AudioSource(JsonElement data) : base(data)
        {
            string audioPath = Helper.JSONGetPropertyString(data, "audio", null);
            if(audioPath != null) audioID = AudioLoader.LoadWav(audioPath);

            this.looping = Helper.JSONGetPropertyBool(data, "looping", false);
            this.autostart = Helper.JSONGetPropertyBool(data, "autostart", false);
            this.globalSound = Helper.JSONGetPropertyBool(data, "global", false);
            this.range = Helper.JSONGetPropertyFloat(data, "range", 1f);
            this.volume = Helper.JSONGetPropertyFloat(data, "volume", 1f);
            this.pitch = Helper.JSONGetPropertyFloat(data, "pitch", 1f);
            this.sourceID = AL.GenSource();
            AL.Source(sourceID, ALSourcei.Buffer, this.audioID);
            AL.Source(sourceID, ALSourceb.Looping, this.looping);

            
        }

        public override void Update(float deltaTime)
        {
            if (globalSound)
            {
                AL.Source(sourceID, ALSourcef.ReferenceDistance, 0f);
                AL.Source(sourceID, ALSourceb.SourceRelative, true);
                AL.Source(sourceID, ALSource3f.Position, 0f, 0f, 0f);
            }
            else
            {
                AL.Source(sourceID, ALSourcef.ReferenceDistance, range);
                AL.Source(sourceID, ALSource3f.Position, global_position.X, global_position.Y, 0f);
                AL.Source(sourceID, ALSourceb.SourceRelative, false);
            }

            

            if (AL.GetSource(sourceID, ALGetSourcei.SourceState) == (int)ALSourceState.Stopped)
            {
                AL.SourceStop(sourceID);
            }

            base.Update(deltaTime);
        }

        public override void Begin()
        {
            if (this.autostart)
            {
                Play();
            }
            base.Begin();
        }

        public void SetSound(int soundID)
        {
            this.audioID = soundID;
            AL.Source(sourceID, ALSourcei.Buffer, soundID);
            if (autostart)
            {
                AL.SourcePlay(sourceID);
            }
        }

        public void SetLooping(bool b)
        {
            looping = b;
            AL.Source(sourceID, ALSourceb.Looping, b);
        }

        public void SetAutostart(bool b)
        {
            autostart = b;
        }

        public void SetVolume(float v)
        {
            volume = v;
            AL.Source(sourceID, ALSourcef.Gain, volume);
        }

        public void SetPitch(float p)
        {
            pitch = p;
            AL.Source(sourceID, ALSourcef.Pitch, pitch);
        }

        public void SetRange(float r)
        {
            range = r;
            AL.Source(sourceID, ALSourcef.ReferenceDistance, range);
        }

        public void Play()
        {
            if (!playing)
            {
                playing = true;
                AL.SourcePlay(sourceID);
            }
        }

        public void Pause()
        {
            if (playing)
            {
                playing = false;
                AL.SourcePause(sourceID);
            }
        }

        public void Stop()
        {
            playing = false;
            AL.SourceStop(sourceID);
            
        }

        

        public void Restart()
        {
            playing = true;
            AL.SourcePlay(sourceID);
        }


        public override void ExposeToInspector()
        {
            base.ExposeToInspector();
            ImGui.Checkbox("global sound", ref globalSound);

            bool l = looping;
            ImGui.Checkbox("looping", ref l);
            SetLooping(l);

            bool p = playing;
            ImGui.Checkbox("playing", ref p);

            if (p) Play();
            else Pause();

            float v = volume;
            ImGui.DragFloat("volume", ref v, 0.1f, 0f, 10f);
            SetVolume(v);

            float pit = pitch;
            ImGui.DragFloat("pitch", ref pit, 0.1f, -2f, 2f);
            SetPitch(pit);
            

            if (!globalSound)
            {
                float r = range;
                ImGui.DragFloat("range", ref r, 0.5f, 0.1f, 500f);
                SetRange(r);
            }
        }
    }
}
