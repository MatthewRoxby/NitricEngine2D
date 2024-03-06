using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using NitricEngine2D.loaders;
using System.Text.Json;

namespace NitricEngine2D
{

    public class Window : GameWindow
    {
        ImGuiController controller;

        Color4 clearColour;

        public Window(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { Size = new Vector2i(width, height), Title = title})
        {
            Unload += Window_Unload;
            RenderFrame += Window_RenderFrame;
            UpdateFrame += Window_UpdateFrame;
            Load += Window_Load;
            Resize += Window_Resize;
            TextInput += Window_TextInput;
            MouseWheel += Window_MouseWheel;
            CenterWindow();
            GameManager.windowWidth = width;
            GameManager.windowHeight = height;
        }

        public void NewScene(string scenePath)
        {
            JsonDocument doc = JsonDocument.Parse(File.OpenRead(scenePath), Helper.DefaultJSONOptions);

            JsonElement windowData;

            if(doc.RootElement.TryGetProperty("window", out windowData))
            {
                JsonElement title, size, clearCol;
                if(windowData.TryGetProperty("title", out title))
                {
                    Title = title.GetString();
                    if (GameManager.EDIT_MODE) {
                        Title += "   [EDIT MODE]";
                    }
                }

                if(windowData.TryGetProperty("size", out size)){
                    Size = new Vector2i(size[0].GetInt32(), size[1].GetInt32());
                }

                if(windowData.TryGetProperty("clearColour", out clearCol))
                {
                    clearColour = new Color4(clearCol[0].GetSingle(), clearCol[1].GetSingle(), clearCol[2].GetSingle(), 1f);
                    
                }
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

            controller = new ImGuiController(ClientSize.X, ClientSize.Y);

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
