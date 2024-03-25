using NitricEngine2D.debug_windows;
using System.Text.Json;

namespace NitricEngine2D
{
    public static class DebugWindowManager
    {
        private static List<DebugWindow> windows = new List<DebugWindow>();

        private static bool visible = true;

        public static void AddWindow(DebugWindow window)
        {
            windows.Add(window);
        }

        public static void DeleteWindow(DebugWindow window)
        {
            windows.Remove(window);
            window.Dispose();
        }

        public static void Update(float deltaTime)
        {
            if (Input.IsKeyPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.GraveAccent)) visible = !visible;


            if (!visible) return;
            foreach(DebugWindow window in windows)
            {
                window.Update(deltaTime);
            }
        }

        public static void ClearAllWindows()
        {
            foreach(DebugWindow window in windows)
            {
                window.Dispose();
            }

            windows.Clear();
        }

        public static T? GetWindow<T>() where T : DebugWindow
        {
            foreach(DebugWindow window in windows)
            {
                if(window is T)
                {
                    return (T)window;
                }
            }

            return null;
        }

        public static void LoadFromSceneFile(string scenePath)
        {
            ClearAllWindows();
            JsonDocument doc = JsonDocument.Parse(File.OpenRead(scenePath), Helper.DefaultJSONOptions);

            JsonElement win;
            if(doc.RootElement.TryGetProperty("debugWindows", out win))
            {
                foreach(JsonElement w in win.EnumerateArray())
                {
                    string type = w.GetString();
                    if (type == null) continue;

                    Type t = Type.GetType("NitricEngine2D.debug_windows." + type);

                    if (t == null) continue;

                    AddWindow((DebugWindow)Activator.CreateInstance(t));
                }
            }
        }
    }
}
