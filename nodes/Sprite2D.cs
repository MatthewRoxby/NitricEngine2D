using ImGuiNET;
using OpenTK.Mathematics;
using NitricEngine2D.loaders;
using System.Text.Json;

namespace NitricEngine2D.nodes
{
    public class Sprite2D : VisibleNode2D
    {
        public Texture? texture { get; private set; } = null;

        public int h_frames = 1;

        public int v_frames = 1;

        public int frame = 0;

        public Color4 modulate = Color4.White;

        public float SPRITE_SCALE { get; private set; } = 0.5f;

        public Sprite2D(JsonElement data) : base(data)
        {
            this.texture = TextureLoader.LoadTexture(Helper.JSONGetPropertyString(data, "texturePath", "null"), false);
            this.h_frames = Helper.JSONGetPropertyInt(data, "h_frames", 1);
            this.v_frames = Helper.JSONGetPropertyInt(data, "v_frames", 1);
            this.frame = Helper.JSONGetPropertyInt(data, "frame", 0);
            this.modulate = Helper.JSONGetPropertyColour(data, "modulate", Color4.White);
        }

        public override void Render(float deltaTime)
        {
            if (!visible) return;

            //Debug.WriteLine(position);
            Renderer.RenderSprite(this);

            base.Render(deltaTime);
        }

        public override void ExposeToInspector()
        {
            base.ExposeToInspector();

            ImGui.DragInt("h_frames", ref h_frames, 1, 1, 100);
            ImGui.DragInt("v_frames", ref v_frames, 1, 1, 100);
            ImGui.DragInt("frame", ref frame, 1, 0, h_frames * v_frames);

            modulate = Helper.ImguiColourEdit4("modulate", modulate);
        }
    }
}
