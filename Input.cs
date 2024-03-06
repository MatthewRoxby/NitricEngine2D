using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;

namespace NitricEngine2D
{
    public static class Input
    {
        static KeyboardState keyboard;
        static MouseState mouse;
        static JoystickState? joystick = null;

        public enum JOY_BUTTONS
        {
            XBOX_A = 0,
            XBOX_B = 1,
            XBOX_X = 2,
            XBOX_Y = 3,
            XBOX_LB = 4,
            XBOX_RB = 5,
            XBOX_SHARE = 6,
            XBOX_SELECT = 7,
            XBOX_LS = 8,
            XBOX_RS = 9,
            XBOX_DPAD_UP = 10,
            XBOX_DPAD_RIGHT = 11,
            XBOX_DPAD_DOWN = 12,
            XBOX_DPAD_LEFT = 13,
        }

        public enum JOY_AXES
        {
            XBOX_LS_X = 0,
            XBOX_LS_Y = 1,
            XBOX_RS_X = 2,
            XBOX_RS_Y = 3,
            XBOX_LT = 4,
            XBOX_RT = 5
        }

        public static void Update(GameWindow window)
        {
            keyboard = window.KeyboardState;
            mouse = window.MouseState;
            if(window.JoystickStates.Count > 0)
            {
                joystick = window.JoystickStates[0];
            }
        }

        public static bool IsJoystickConnected()
        {
            return joystick != null;
        }

        public static bool IsKeyPressed(Keys key)
        {
            return keyboard.IsKeyPressed(key);
        }

        public static bool IsKeyDown(Keys key)
        {
            return keyboard.IsKeyDown(key);
            
        }

        public static bool IsKeyJustReleased(Keys key)
        {
            return keyboard.IsKeyReleased(key);
        }

        public static bool IsMouseButtonPressed(MouseButton button)
        {
            return mouse.IsButtonPressed(button);
        }

        public static bool IsMouseButtonDown(MouseButton button)
        {
            return mouse.IsButtonDown(button);
        }

        public static bool IsMouseButtonJustReleased(MouseButton button)
        {
            return mouse.IsButtonReleased(button);
        }

        public static Vector2 GetMousePosition()
        {
            return mouse.Position;
        }

        public static Vector2 GetLastMousePosition()
        {
            return mouse.PreviousPosition;
        }

        public static Vector2 GetMouseDelta()
        {
            return mouse.Delta;
        }

        public static Vector2 GetMouseScrollDelta()
        {
            return mouse.ScrollDelta;
        }

        public static bool IsJoyButtonPressed(JOY_BUTTONS button)
        {
            if (joystick!= null)
            {
                return joystick.IsButtonPressed((int)button);
            }
            else
            {
                return false;
            }
        }

        public static bool IsJoyButtonDown(JOY_BUTTONS button)
        {
            if (joystick != null)
            {
                return joystick.IsButtonDown((int)button);
            }
            else
            {
                return false;
            }
        }

        public static bool IsJoyButtonJustReleased(JOY_BUTTONS button)
        {
            if (joystick != null)
            {
                return joystick.IsButtonReleased((int)button);
            }
            else
            {
                return false;
            }
        }

        public static float GetJoyAxis(JOY_AXES axis)
        {
            if (joystick!= null)
            {
                return joystick.GetAxis((int)axis);
            }
            else
            {
                return 0f;
            }
        }

        
    }
}
