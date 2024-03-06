using NitricEngine2D.loaders;
using NitricEngine2D.nodes;

namespace NitricEngine2D
{
    public static class GameManager
    {
        public static Node? currentRoot { get; private set; } = null;

        public static int windowWidth, windowHeight;

        public static Window? windowRef = null;

        private static string lastScenePath;

        public static bool EDIT_MODE = false;

        public static List<string> NODE_NAMESPACES = new List<string>();

        public static void SetEditMode(bool b = true)
        {
            EDIT_MODE = b;
        }

        public static void SetScene(string scenePath)
        {
            lastScenePath = scenePath;
            if(currentRoot != null)
            {
                currentRoot?.End();
            }
            
            if(windowRef == null)
            {
                windowRef = new Window(800,450, "Window");
                windowRef.Run();
            }
            else
            {
                EndSceneLoad();
            }

            
        }

        public static void EndSceneLoad()
        {
            windowRef.NewScene(lastScenePath);
            windowRef.CenterWindow();


            currentRoot = SceneLoader.LoadScene(lastScenePath);
            if(EDIT_MODE) DebugWindowManager.LoadFromSceneFile(lastScenePath);

            currentRoot?.Begin();
        }

        public static void Update(float deltaTime)
        {
            if(Input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.LeftAlt) && Input.IsKeyPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.R) && EDIT_MODE)
            {
                HotReload();
            }
            currentRoot?.Update(deltaTime);
        }

        public static void HotReload()
        {
            SetScene(lastScenePath);
        }

        public static void Render(float deltaTime)
        {
            currentRoot?.Render(deltaTime);
        }

        public static void Quit()
        {
            currentRoot?.End();
        }
    }
}
