using NitricEngine2D.nodes;
using OpenTK.Mathematics;
using System.Text.Json;
using ImGuiNET;

namespace NitricEngine2D.particle_effects
{
    public abstract class ParticleEffect
    {
        public ParticleEffect(JsonElement data)
        {

        }

        public virtual void ParticleSpawn(ParticleEmitter emitter, Particle p)
        {

        }

        public virtual void Update(ParticleEmitter emitter, Particle p, float deltaTime)
        {

        }

        public virtual void ExposeToInspector()
        {

        }
    }

    public class AccelerationEffect : ParticleEffect
    {
        private Vector2 acc;

        public AccelerationEffect(JsonElement data) : base(data) {
            this.acc = Helper.JSONGetPropertyVec2(data, "acceleration", Vector2.Zero);
        }

        public override void Update(ParticleEmitter emitter, Particle p, float deltaTime)
        {
            p.v_y += acc.Y * deltaTime;
            p.v_x += acc.X * deltaTime;

            base.Update(emitter, p, deltaTime);
        }

        public override void ExposeToInspector()
        {
            if(ImGui.TreeNode("Acceleration effect"))
            {
                acc = Helper.ImguiDragFloat2("acceleration", 0.1f, acc);
                ImGui.TreePop();
            }
        }
    }

    public class InitialVelocityEffect : ParticleEffect
    {
        private float startVelocity;
        private float spread;
        Random rand = new Random();

        public InitialVelocityEffect(JsonElement data) : base(data)
        {
            this.startVelocity = Helper.JSONGetPropertyFloat(data, "startVelocity", 0f);
            this.spread = Helper.JSONGetPropertyFloat(data, "spread", 0f);
        }

        public override void ParticleSpawn(ParticleEmitter emitter, Particle p)
        {
            float a = MathHelper.DegreesToRadians((int)emitter.rotation_degrees + rand.Next((int)(-spread / 2), (int)(spread / 2)));
            p.v_x = startVelocity * MathF.Cos(a);
            p.v_y = startVelocity * MathF.Sin(a);
            base.ParticleSpawn(emitter, p);
        }

        public override void ExposeToInspector()
        {
            if (ImGui.TreeNode("Initial velocity effect"))
            {
                ImGui.DragFloat("initial velocity", ref startVelocity);
                ImGui.DragFloat("spread", ref spread, 1f, 0f, 360f);
                ImGui.TreePop();
            }
        }
    }

    public class AnimatedScaleEffect : ParticleEffect
    {
        BezierCurve curve;


        public AnimatedScaleEffect(JsonElement data) : base(data)
        {
            JsonElement c;
            if(data.TryGetProperty("curve", out c))
            {
                this.curve = new BezierCurve(c);
            }
            else
            {
                this.curve = BezierCurve.GetDefault();
            }
        }

        public override void Update(ParticleEmitter emitter, Particle p, float deltaTime)
        {
            p.s_x = curve.SampleAt(p.time / emitter.lifetime);
            p.s_y = curve.SampleAt(p.time / emitter.lifetime);
            base.Update(emitter, p, deltaTime);
        }

        public override void ExposeToInspector()
        {
            if (ImGui.TreeNode("Animated scale effect"))
            {
                curve.ExposeToInspector();
                ImGui.TreePop();
            }

            
        }
    }

    public class AnimatedVelocityEffect : ParticleEffect
    {
        private BezierCurve curve;
        private float spread;
        Random rand = new Random();

        public AnimatedVelocityEffect(JsonElement data) : base(data)
        {
            this.spread = Helper.JSONGetPropertyFloat(data, "spread", 0f);
            JsonElement c;
            if(data.TryGetProperty("curve", out c))
            {
                this.curve = new BezierCurve(c);
            }
            else
            {
                this.curve = BezierCurve.GetDefault();
            }
        }

        public override void ParticleSpawn(ParticleEmitter emitter, Particle p)
        {
            float a = MathHelper.DegreesToRadians((int)emitter.rotation_degrees + rand.Next((int)(-spread / 2), (int)(spread / 2)));
            p.v_x = 0.01f * MathF.Cos(a);
            p.v_y = 0.01f * MathF.Sin(a);
            base.ParticleSpawn(emitter, p);
        }

        public override void Update(ParticleEmitter emitter, Particle p, float deltaTime)
        {
            Vector2 v = new Vector2(p.v_x, p.v_y).Normalized() * curve.SampleAt(p.time / emitter.lifetime);
            p.v_x = v.X;
            p.v_y = v.Y;

            base.Update(emitter , p, deltaTime);
        }

        public override void ExposeToInspector()
        {
            if (ImGui.TreeNode("Animated velocity effect"))
            {
                ImGui.DragFloat("spread", ref spread, 1f, 0f, 360f);
                curve.ExposeToInspector();

                ImGui.TreePop();
            }
        }
    }

    public class AnimatedColourEffect : ParticleEffect
    {
        Gradient colourGradient;

        public AnimatedColourEffect(JsonElement data) : base(data)
        {
            JsonElement grad;
            if(data.TryGetProperty("gradient", out grad))
            {
                colourGradient = new Gradient(grad);
            }
            else
            {
                colourGradient = Gradient.GetDefault();
            }
        }


        public override void Update(ParticleEmitter emitter, Particle p, float deltaTime)
        {
            p.c = colourGradient.SampleAt(p.time / emitter.lifetime);
        }

        public override void ExposeToInspector()
        {
            if (ImGui.TreeNode("Animated colour effect"))
            {
                colourGradient.ExposeToInspector();
                ImGui.TreePop();
            }
        }
    }

    public class RingPlacementEffect : ParticleEffect
    {
        float min_radius, max_radius;
        Random rand = new Random();

        public RingPlacementEffect(JsonElement data) : base(data)
        {
            this.min_radius = Helper.JSONGetPropertyFloat(data, "minRadius", 0f);
            this.max_radius = Helper.JSONGetPropertyFloat(data, "maxRadius", 0f);
        }

        public override void ParticleSpawn(ParticleEmitter emitter, Particle p)
        {
            float a = MathHelper.DegreesToRadians(rand.Next(0, 360));
            float r = (float)(rand.NextDouble() * (max_radius - min_radius) + min_radius);
            p.x = emitter.position.X + MathF.Cos(a) * r;
            p.y = emitter.position.Y + MathF.Sin(a) * r;

            base.ParticleSpawn(emitter, p);
        }

        public override void ExposeToInspector()
        {
            if (ImGui.TreeNode("Ring placement effect"))
            {
                ImGui.DragFloat("min radius", ref min_radius);
                ImGui.DragFloat("max radius", ref max_radius);
                ImGui.TreePop();
            }
        }
    }
}
