using OpenTK.Mathematics;
using ImGuiNET;
using System.Text.Json;

namespace NitricEngine2D
{
    public static class Helper
    {
        public static JsonDocumentOptions DefaultJSONOptions = new JsonDocumentOptions { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip };

        public static Vector2 ImguiDragFloat2(string label, float sensitivity, Vector2 v)
        {
            System.Numerics.Vector2 sv = new System.Numerics.Vector2(v.X, v.Y);
            ImGui.DragFloat2(label, ref sv, sensitivity);
            return new Vector2(sv.X, sv.Y);
        }

        public static Vector3 ImguiDragFloat3(string label, float sensitivity, Vector3 v)
        {
            System.Numerics.Vector3 sv = new System.Numerics.Vector3(v.X, v.Y, v.Z);
            ImGui.DragFloat3(label, ref sv, sensitivity);
            return new Vector3(sv.X, sv.Y, sv.Z);
        }

        public static Color4 ImguiColourEdit4(string label, Color4 v)
        {
            System.Numerics.Vector4 sv = new System.Numerics.Vector4(v.R, v.G, v.B, v.A);
            ImGui.ColorEdit4(label, ref sv);
            return new Color4(sv.X, sv.Y, sv.Z, sv.W);
        }

        public static Vector2 JSONGetPropertyVec2(JsonElement element, string name, Vector2 defaultValue)
        {
            JsonElement property;
            if (element.TryGetProperty(name, out property))
            {
                return new Vector2(property[0].GetSingle(), property[1].GetSingle());
            }

            return defaultValue;
        }

        public static int JSONGetPropertyInt(JsonElement element, string name, int defaultValue)
        {
            JsonElement property;
            if (element.TryGetProperty(name, out property))
            {
                return property.GetInt32();
            }

            return defaultValue;
        }

        public static float JSONGetPropertyFloat(JsonElement element, string name, float defaultValue)
        {
            JsonElement property;
            if (element.TryGetProperty(name, out property))
            {
                return property.GetSingle();
            }

            return defaultValue;
        }

        public static string JSONGetPropertyString(JsonElement element, string name, string defaultValue)
        {
            JsonElement property;
            if (element.TryGetProperty(name, out property))
            {
                return property.GetString();
            }

            return defaultValue;
        }

        public static bool JSONGetPropertyBool(JsonElement element, string name, bool defaultValue)
        {
            JsonElement property;
            if (element.TryGetProperty(name, out property))
            {
                return property.GetBoolean();
            }

            return defaultValue;
        }

        public static Color4 JSONGetPropertyColour(JsonElement element, string name, Color4 defaultValue)
        {
            JsonElement property;
            if (element.TryGetProperty(name, out property))
            {
                return new Color4(property[0].GetSingle(), property[1].GetSingle(), property[2].GetSingle(), property[3].GetSingle());
            }

            return defaultValue;
        }

        public static float wrapFloat(float value, float minValue, float maxValue)
        {
            if(minValue > maxValue)
            {
                return wrapFloat(value, maxValue, minValue);
            }

            return (value >= 0 ? minValue : maxValue) + (value % (maxValue - minValue));
        }

        public static Matrix4 createTransformation(Vector2 position, Vector2 scale, float rotation_degrees, int z_index)
        {
            Matrix4 result;
            result = Matrix4.CreateScale(new Vector3(scale.X, scale.Y, 1f));
            result *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation_degrees));
            result *= Matrix4.CreateTranslation(new Vector3(position.X, position.Y, -1 - z_index));
            return result;
        }

        public static Color4 LerpColor(Color4 a, Color4 b, float t)
        {
            Vector4 va = (Vector4)a;
            Vector4 vb = (Vector4)b;
            return (Color4)Vector4.Lerp(va, vb, t);
        }

        public static uint Color4ToImguiColour(Color4 col)
        {
            return ImGui.ColorConvertFloat4ToU32(new System.Numerics.Vector4(col.R, col.G, col.B, col.A));
        }

        public static System.Numerics.Vector2 FromOpentkVec2(Vector2 value) 
        {
            return new System.Numerics.Vector2(value.X, value.Y);
        }

        public static Vector2 FromNumericsVec2(System.Numerics.Vector2 value)
        {
            return new Vector2(value.X, value.Y);
        }
    }
}
