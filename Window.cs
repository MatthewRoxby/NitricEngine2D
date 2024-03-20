using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common.Input;
using NitricEngine2D.loaders;
using System.Text.Json;
using StbImageSharp;
using System.Runtime.CompilerServices;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace NitricEngine2D
{

    public class Window : GameWindow
    {
        ImGuiController controller;

        Color4 clearColour;

        public Window(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = new Vector2i(width, height), Title = title})
        {
            
            Unload += Window_Unload;
            RenderFrame += Window_RenderFrame;
            UpdateFrame += Window_UpdateFrame;
            Load += Window_Load;
            Resize += Window_Resize;
            TextInput += Window_TextInput;
            MouseWheel += Window_MouseWheel;
            controller = new ImGuiController(ClientSize.X, ClientSize.Y);
            CenterWindow();
            LoadIcon("NElogo.png");
            GameManager.windowWidth = width;
            GameManager.windowHeight = height;
        }

        public void LoadIcon(string path)
        {
            
            ImageResult icon = ImageResult.FromStream(File.OpenRead(path));
            OpenTK.Windowing.Common.Input.Image i = new OpenTK.Windowing.Common.Input.Image(icon.Width, icon.Height, icon.Data);
            this.Icon = new OpenTK.Windowing.Common.Input.WindowIcon(i);
        }

        public void NewScene(string scenePath)
        {
            JsonDocument doc = JsonDocument.Parse(File.OpenRead(scenePath), Helper.DefaultJSONOptions);

            JsonElement windowData;

            if(doc.RootElement.TryGetProperty("window", out windowData))
            {

                Title = Helper.JSONGetPropertyString(windowData, "title", "Unnamed Scene");
                if (GameManager.EDIT_MODE)
                {
                    Title += "   [EDIT MODE]";
                }

                Size = (Vector2i)Helper.JSONGetPropertyVec2(windowData, "size", new Vector2(800, 450));

                clearColour = Helper.JSONGetPropertyColourNoAlpha(windowData, "clearColour", Color4.Black);

                LoadIcon(Helper.JSONGetPropertyString(windowData, "iconPath", "NElogo.png"));
            }
        }

        private void Window_MouseWheel(OpenTK.Windowing.Common.MouseWheelEventArgs obj)
        {
            controller.MouseScroll(obj.Offset);
        }

        private void Window_TextInput(OpenTK.Windowing.Common.TextInputEventArgs obj)
        {
            controller.PressChar((char)obj.Unicode);
        }

        private void Window_Resize(OpenTK.Windowing.Common.ResizeEventArgs obj)
        {
            GL.Viewport(0,0,obj.Width ,obj.Height);
            controller.WindowResized(ClientSize.X, ClientSize.Y);
            GameManager.windowWidth = obj.Width;
            GameManager.windowHeight = obj.Height;
        }

        private void Window_Load()
        {
            
            GL.ClearColor(Color4.LightGreen);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.CullFace);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            

            GameManager.EndSceneLoad();
        }

        private void Window_UpdateFrame(OpenTK.Windowing.Common.FrameEventArgs obj)
        {
            float deltaTime = (float)obj.Time;

            Input.Update(this);

            if (Input.IsKeyPressed(Keys.Tab) && GameManager.EDIT_MODE)
            {
                Renderer.SetWireframe(!Renderer.wireframe);
            }
            

            controller.Update(this, deltaTime);

            GameManager.Update(deltaTime);
        }

        private void Window_RenderFrame(OpenTK.Windowing.Common.FrameEventArgs obj)
        {
            float deltaTime = (float)obj.Time;
            GL.ClearColor(clearColour);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GameManager.Render(deltaTime);

            DebugWindowManager.Update(deltaTime);

            controller.Render();

            ImGuiController.CheckGLError("End of frame");

            SwapBuffers();
        }

        private void Window_Unload()
        {
            GameManager.Quit();

            MeshLoader.CleanUp();
            ShaderLoader.CleanUp();
            TextureLoader.CleanUp();
        }
    }
}
