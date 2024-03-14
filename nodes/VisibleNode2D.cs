using NitricEngine2D.shaders;
using System.Text.Json;

namespace NitricEngine2D.nodes
{
    public class VisibleNode2D : Node2D
    {
        public bool visible = true;

        public Shader? shader = null;

        public VisibleNode2D(string name = "VisibleNode2D"): base(name) { }

        public VisibleNode2D(JsonElement data) : base(data)
        {
            this.visible = Helper.JSONGetPropertyBool(data, "visible", true);
            JsonElement shaderData;
            if(data.TryGetProperty("shader", out shaderData))
            {
                this.shader = new Shader(shaderData);
            }
        }

        public override void Render(float deltaTime)
        {
            if (!visible) return;

            base.Render(deltaTime);
        }
    }
}
