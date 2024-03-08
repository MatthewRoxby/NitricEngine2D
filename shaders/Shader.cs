using OpenTK.Graphics.OpenGL4;
using NitricEngine2D.loaders;
using OpenTK.Mathematics;

namespace NitricEngine2D.shaders
{
    public abstract class Shader
    {
        protected int program;
        private Dictionary<string, int> locationCache = new Dictionary<string, int>();

        public Shader(int program, string[] uniformNames)
        {
            this.program = program;
            CacheLocations(uniformNames);
        }

        private void CacheLocations(string[] uniformNames)
        {
            foreach(string name in uniformNames)
            {
                locationCache.Add(name, GL.GetUniformLocation(program, name));
            }
        }

        public void Use()
        {
            GL.UseProgram(program);
        }

        public void SetUniformMatrix(string name, bool transposed, Matrix4 value)
        {
            GL.UniformMatrix4(locationCache[name], transposed, ref value);
        }

        public void SetUniform4(string name, Color4 value)
        {
            GL.Uniform4(locationCache[name], value);
        }

        public void SetUniform1(string name, float value)
        {
            GL.Uniform1(locationCache[name], value);
        }

        public void SetUniform1(string name, int value)
        {
            GL.Uniform1(locationCache[name], value);
        }

        public void SetUniform1(string name, bool value)
        {
            GL.Uniform1(locationCache[name], value? 1:0);
        }

        public void SetUniform2(string name, Vector2 value)
        {
            GL.Uniform2(locationCache[name], value);
        }

        public void SetUniformTexture2D(string name, int textureBank, Texture value)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + textureBank);

            GL.BindTexture(TextureTarget.Texture2D, value.id);

            GL.Uniform1(locationCache[name], textureBank);
        }
    }

    public class DefaultSpriteShader : Shader
    {

        public DefaultSpriteShader() : base(ShaderLoader.LoadShader("shaders/defaultSprite.vert", "shaders/defaultSprite.frag"), new string[]
        {
            "aspect", "uv_offset", "uv_scale", "modulate", "transformation", "projection", "view", "albedo", "textureEnabled"
        })
        {

        }
    }

    public class DefaultViewportShader : Shader
    {
        public DefaultViewportShader() : base(ShaderLoader.LoadShader("shaders/defaultViewport.vert", "shaders/defaultViewport.frag"), new string[] {"aspect"})
        {

        }
    }

    public class DefaultParticleShader : Shader
    {
        public DefaultParticleShader() : base(ShaderLoader.LoadShader("shaders/defaultParticle.vert", "shaders/defaultParticle.frag"), new string[] { "aspect", "transformation", "projection", "view", "albedo", "textureEnabled", "modulate" })
        {

        }
    }
}