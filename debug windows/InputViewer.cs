using ImGuiNET;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace NitricEngine2D.debug_windows
{
    /// <summary>
    /// A debug window that gives information on the current pressed inputs
    /// </summary>
    public class InputViewer : DebugWindow
    {
        public override void Update(float deltaTime)
        {
            if (ImGui.Begin("Input"))
            {
                //adds all pressed keys into one string
                string keystring = "";
                foreach (Keys k in Enum.GetValues<Keys>())
                {
                    if (k == Keys.Unknown) continue;
                    if (Input.IsKeyDown(k))
                    {
                        keystring += Enum.GetName<Keys>(k) + " ";
                    }
                }

                ImGui.Text("Pressed keys: " + keystring);

                ImGui.Text("Mouse Position: " + Input.GetMousePosition().ToString());

                ImGui.Text("Mouse Delta: " + Input.GetMouseDelta().ToString());

                string mousestring = "";

                //adds all pressed mouse buttons into one string
                foreach (MouseButton b in Enum.GetValues<MouseButton>())
                {
                    if (Input.IsMouseButtonDown(b))
                    {
                        mousestring += Enum.GetName<MouseButton>(b) + " ";
                    }
                }

                ImGui.Text("Pressed Mouse buttons: " + mousestring);

                if (Input.IsJoystickConnected())
                {
                    ImGui.Text("Joystick connected.");

                    string buttonList = "";

                    string[] buttonNames = Enum.GetNames(typeof(Input.JOY_BUTTONS));

                    //adds all pressed joystick buttons into one string
                    for (int i = 0; i < buttonNames.Length; i++)
                    {
                        if (Input.IsJoyButtonDown((Input.JOY_BUTTONS)i))
                        {
                            buttonList += buttonNames[i] + " ";
                        }
                    }

                    ImGui.Text("Pressed Joystick buttons: " + buttonList);

                    ImGui.Text("Joystick axis values: ");

                    string[] axisNames = Enum.GetNames(typeof(Input.JOY_AXES));

                    //outputs the value of all registered joystick axes
                    for (int i = 0; i < axisNames.Length; i++)
                    {
                        ImGui.Text(axisNames[i] + ":  " + Input.GetJoyAxis((Input.JOY_AXES)i).ToString());
                    }
                }
            }
        }
    }
}
