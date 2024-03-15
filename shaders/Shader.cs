using OpenTK.Graphics.OpenGL4;
using NitricEngine2D.loaders;
using OpenTK.Mathematics;
using System.Text.Json;
using System.Diagnostics;

namespace NitricEngine2D.shaders
{
    public class Shader
    {
        protected int program;
        private Dictionary<string, int> locationCache = new Dictionary<string, int>();

        public Shader(string vertexPath, string fragmentPath)
        {
            this.program = ShaderLoader.LoadShader(vertexPath, fragmentPath);
        }

        public Shader(JsonElement data)
        {
            string v = Helper.JSONGetPropertyString(data, "vertexPath", null);
            string f = Helper.JSONGetPropertyString(data, "fragmentPath", null);

            if(v != null && f != null)
            {
                this.program = ShaderLoader.LoadShader(v, f);
            }
            else
            {
                Debug.WriteLine("shader loading from JSON failed");
                this.program = -1;
            }

            JsonElement uniformData;
            if(data.TryGetProperty("uniforms", out uniformData))
            {
                Use();
                foreach(JsonElement u in uniformData.EnumerateArray())
                {
                    SetUniformFromJSON(u);
                }
            }
        }

        private void SetUniformFromJSON(JsonElement data)
        {
            string uniName = Helper.JSONGetPropertyString(data, "name", null);
            string uniType = Helper.JSONGetPropertyString(data, "type", null);
            if(uniName != null && uniType != null)
            {
                //Debug.WriteLine($"loading uniform with name {uniName} and type {uniType}");
                switch (uniType)
                {
                    case "vec2":
                        SetUniform2(uniName, Helper.JSONGetPropertyVec2(data, "value", Vector2.Zero));
                        break;
                    case "float":
                        SetUniform1(uniName, Helper.JSONGetPropertyFloat(data, "value", 0f));
                        break;
                    case "int":
                        SetUniform1(uniName, Helper.JSONGetPropertyInt(data, "value", 0));
                        break;
                    case "bool":
                        SetUniform1(uniName, Helper.JSONGetPropertyBool(data, "value", false));
                        break;
                    case "vec4":
                        SetUniform4(uniName, Helper.JSONGetPropertyColour(data, "value", Color4.White));
                        break;
                }
            }
            else
            {
                Debug.WriteLine($"could not load uniform with name {uniName} and type {uniType}");
            }
        }

        public int GetUniformLocation(string name)
        {
            if (locationCache.ContainsKey(name))
            {
                return locationCache[name];
            }
            else
            {
                int l = GL.GetUniformLocation(program, name);
                locationCache[name] = l;
                return l;
            }
        }

        public void Use()
        {
            GL.UseProgram(program);
        }

        public void SetUniformMatrix(string name, bool transposed, Matrix4 value)
        {
            GL.UniformMatrix4(GetUniformLocation(name), transposed, ref value);
        }

        public void SetUniform4(string name, Color4 value)
        {
            GL.Uniform4(GetUniformLocation(name), value);
        }

        public void SetUniform1(string name, float value)
        {
            GL.Uniform1(GetUniformLocation(name), value);
        }

        public void SetUniform1(string name, int value)
        {
            GL.Uniform1(GetUniformLocation(name), value);
        }

        public void SetUniform1(string name, bool value)
        {
            GL.Uniform1(GetUniformLocation(name), value? 1:0);
        }

        public void SetUniform2(string name, Vector2 value)
        {
            GL.Uniform2(GetUniformLocation(name), value);
        }

        public void SetUniformTexture2D(string name, int textureBank, Texture value)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + textureBank);

            GL.BindTexture(TextureTarget.Texture2D, value.id);

            GL.Uniform1(GetUniformLocation(name), textureBank);
        }
    }
}