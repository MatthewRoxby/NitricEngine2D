using ImGuiNET;
using NitricEngine2D.loaders;
using NitricEngine2D.particle_effects;
using OpenTK.Mathematics;
using System.Diagnostics;
using System.Text.Json;

namespace NitricEngine2D.nodes
{
    public class Particle
    {
        //MUST BE KEPT AS LEAN AS POSSIBLE
        public float x, y;
        public float v_x, v_y;
        public float s_x, s_y;
        public float rot;
        public float time;
        public Color4 c;
    }

    public class ParticleEmitter : VisibleNode2D
    {
        public List<Particle> aliveParticles = new List<Particle>();
        public Queue<Particle> deadParticles = new Queue<Particle>();

        public int numParticles { get; private set; }

        public float lifetime { get; private set; }

        public float emissionRate { get; private set; } //particles emitted per second

        public Texture texture;

        private List<ParticleEffect> effects = new List<ParticleEffect>();

        private float emissionCooldown = 0f;

        public bool emitting = false, autostart = false;

        public override void Update(float deltaTime)
        {
            if(emitting) emissionCooldown += deltaTime;

            while (emissionCooldown > (1f / emissionRate) && deadParticles.Count > 0 && emitting)
            {
                emissionCooldown -= (1f / emissionRate);

                //spawn particle
                Particle p = deadParticles.Dequeue();
                p.time = 0;
                p.v_x = 0f;
                p.v_y = 0f;
                p.x = 0;
                p.y = 0;
                p.rot = 0;
                p.s_x = 1;
                p.s_y = 1;
                p.c = Color4.White;
                foreach (ParticleEffect effect in effects)
                {
                    effect.ParticleSpawn(this, p);
                }

                //Debug.WriteLine("spawning particle...");
                aliveParticles.Add(p);

            }

            List<Particle> particlesToKill = new List<Particle>();

            foreach(Particle p in aliveParticles)
            {
                p.time += deltaTime;
                if (p.time > lifetime)
                {
                    particlesToKill.Add(p);
                    continue;
                }

                foreach (ParticleEffect effect in effects)
                {
                    effect.Update(this, p, deltaTime);
                }

                p.x += p.v_x * deltaTime;
                p.y += p.v_y * deltaTime;
            }

            foreach(Particle p in particlesToKill)
            {
                aliveParticles.Remove(p);
                deadParticles.Enqueue(p);
                
            }


            base.Update(deltaTime);
        }

        public override void ExposeToInspector()
        {
            base.ExposeToInspector();

            ImGui.Checkbox("emitting", ref emitting);

            ImGui.Text($"number of alive particles: {aliveParticles.Count} \n number of dead particles: {deadParticles.Count}");

            if(ImGui.TreeNode("Particle Effects:##" + this.id))
            {
                foreach (ParticleEffect e in effects)
                {
                    e.ExposeToInspector();
                }

                ImGui.TreePop();
            }
            
        }

        public Particle[] GetParticles()
        {
            return aliveParticles.ToArray();
        }

        public ParticleEmitter(JsonElement data) : base(data)
        {
            string dataPath = Helper.JSONGetPropertyString(data, "dataPath", null);

            if (dataPath == null) return;
            
            JsonElement root = JsonDocument.Parse(File.OpenRead(dataPath), Helper.DefaultJSONOptions).RootElement;

            this.numParticles = Helper.JSONGetPropertyInt(root, "numParticles", 0);
            this.lifetime = Helper.JSONGetPropertyFloat(root, "lifetime", 1f);
            this.texture = TextureLoader.LoadTexture(Helper.JSONGetPropertyString(root, "texturePath", null));
            this.emissionRate = Helper.JSONGetPropertyFloat(root, "emissionRate", 1f);
            this.autostart = Helper.JSONGetPropertyBool(root, "autostart", false);

            JsonElement effects;
            if(root.TryGetProperty("effects", out effects))
            {
                foreach(JsonElement effect in effects.EnumerateArray())
                {
                    string type = Helper.JSONGetPropertyString(effect, "type", null);

                    if (type == null) continue;

                    Type t = Type.GetType("NitricEngine2D.particle_effects." + type);

                    if(t != null)
                    {
                        this.effects.Add((ParticleEffect)Activator.CreateInstance(t, new object[] { effect }));
                    }
                    else
                    {
                        Debug.WriteLine($"could not find particle effect of type: {type}");
                    }
                }
            }
        }

        public override void Begin()
        {
            emitting = autostart;
            deadParticles.EnsureCapacity(this.numParticles);
            for(int i = 0; i < this.numParticles; i++)
            {
                deadParticles.Enqueue(new Particle());
            }
            base.Begin();

        }

        public override void Render(float deltaTime)
        {
            if (!visible) return;

            Renderer.RenderParticles(this);

            base.Render(deltaTime);
        }
    }
}
