using ImGuiNET;
using NitricEngine2D.shaders;
using OpenTK.Mathematics;
using System.Diagnostics;
using System.Text.Json;

namespace NitricEngine2D.nodes
{
    public class VisibleNode2D : Node2D
    {
        public Box2 AABB { get; protected set; }

        public bool visible = true;

        public bool computedVisible { get; private set; }

        public Shader? shader = null;

        public VisibleNode2D(string name = "VisibleNode2D"): base(name) {
            this.AABB = new Box2(-Vector2.One, Vector2.One);
        }

        public VisibleNode2D(JsonElement data) : base(data)
        {
            this.visible = Helper.JSONGetPropertyBool(data, "visible", true);
            JsonElement shaderData;
            if(data.TryGetProperty("shader", out shaderData))
            {
                this.shader = new Shader(shaderData);
            }
        }

        public override void Update(float deltaTime)
        {
            //AABB = new Box2() { Center = global_position, HalfSize = global_scale };
            if (visible)
            {
                if(Renderer.renderCamera == null)
                {
                    Debug.WriteLine($"no camera, so {name} is visible!");
                    computedVisible = true;
                }
                else
                {
                    computedVisible = AABB.Contains(Renderer.renderCamera.bounds);
                }
                
            }
            else
            {
                computedVisible = false;
            }

            base.Update(deltaTime);
        }

        public override void Render(float deltaTime)
        {
            if (!computedVisible) return;
            base.Render(deltaTime);
        }

        public override void ExposeToInspector()
        {
            base.ExposeToInspector();
            ImGui.Checkbox("visible", ref visible);
            ImGui.Text("computed visibility: " + (computedVisible ? "visible" : "invisible"));
            ImGui.Text("bounds: " + AABB.ToString());
        }
    }
}
