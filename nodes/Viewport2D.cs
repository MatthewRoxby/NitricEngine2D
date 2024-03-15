using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Diagnostics;
using System.Text.Json;

namespace NitricEngine2D.nodes
{
    public class Viewport2D : VisibleNode2D
    {
        public int width { get; private set; }
        public int height { get; private set; }

        public Color4 clearColour;

        public bool disableColourClear = false;

        public Camera2D? renderCamera = null;

        public int frameBuffer { get; private set; }

        public bool stretchToWindow, keepAspect;

        public int colourTexture {  get; private set; }

        public Viewport2D(int width, int height, Color4 clearColour, bool stretchToWindow = true, bool keepAspect = true, string name = "Viewport2D") : base(name)
        {
            this.width = width;
            this.height = height;
            this.clearColour = clearColour;
            this.stretchToWindow = stretchToWindow; 
            this.keepAspect = keepAspect;
        }

        public Viewport2D(JsonElement data) : base(data)
        {
            this.width = Helper.JSONGetPropertyInt(data, "width", GameManager.windowWidth);
            this.height = Helper.JSONGetPropertyInt(data, "height", GameManager.windowHeight);

            this.clearColour = Helper.JSONGetPropertyColour(data, "clearColour", Color4.CornflowerBlue);

            this.stretchToWindow = Helper.JSONGetPropertyBool(data, "stretch", true);
            this.keepAspect = Helper.JSONGetPropertyBool(data, "keepAspect", true);
        }

        public override void Begin()
        {
            frameBuffer = GL.GenFramebuffer();

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);

            colourTexture = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, colourTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, 0);
            GL.TextureParameter(colourTexture, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TextureParameter(colourTexture, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, colourTexture, 0);
            
            base.Begin();
        }

        public override void Update(float deltaTime)
        {
            AABB = new Box2(-1000000, -1000000, 1000000, 100000);
            if(renderCamera != null)
            {
                renderCamera.width = width;
                renderCamera.height = height;
            }
            base.Update(deltaTime);
        }

        public override void Render(float deltaTime)
        {
            if (!visible) return;

            Renderer.SetViewport(this);
            //Debug.WriteLine("before: " + (Renderer.renderViewport == null));
            GL.ClearColor(clearColour);
            
            if(!disableColourClear)GL.Clear(ClearBufferMask.ColorBufferBit);

            
            base.Render(deltaTime);

            Renderer.SetViewport(null);
            //Debug.WriteLine("after: " + (Renderer.renderViewport == null));

            Renderer.RenderViewport(this);
        }

        public override void End()
        {
            GL.DeleteTexture(colourTexture);
            GL.DeleteFramebuffer(frameBuffer);
            base.End();
        }

        public override void ExposeToInspector()
        {
            base.ExposeToInspector();
            ImGui.Text($"size: ({width},{height})");
            ImGui.Checkbox("stretch", ref stretchToWindow);
            ImGui.Checkbox("keepAspect", ref keepAspect);

            ImGui.Checkbox("disable colour clear", ref disableColourClear);

            clearColour = Helper.ImguiColourEdit4("clear colour", clearColour);

        }
    }
}
