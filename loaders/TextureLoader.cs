using OpenTK.Graphics.OpenGL4;
using StbImageSharp;
using System.Diagnostics;

namespace NitricEngine2D.loaders
{
    public class Texture
    {
        public int id { get; private set; }
        public int width { get; private set; }
        public int height { get; private set; }

        public Texture(int id, int width, int height)
        {
            this.id = id;
            this.width = width;
            this.height = height;
        }
    }



    public static class TextureLoader
    {
        private static Dictionary<string, Texture> texCache = new Dictionary<string, Texture>();

        public static void CleanUp()
        {
            foreach(Texture t in texCache.Values)
            {
                GL.DeleteTexture(t.id);
            }
        }

        public static Texture? LoadTexture(string path, bool flip_y = true, TextureMinFilter minFilter = TextureMinFilter.Nearest, TextureMagFilter magFilter = TextureMagFilter.Nearest, TextureWrapMode wrapMode = TextureWrapMode.Repeat, bool gen_mipmaps = false)
        {
            if (texCache.ContainsKey(path))
            {
                return texCache[path];
            }

            StbImage.stbi_set_flip_vertically_on_load(flip_y ? 1 : 0);

            if (!File.Exists(path))
            {
                Debug.WriteLine($"Texture file {path} not found!");
                return null;
            }

            ImageResult image = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);
            

            int texture = GL.GenTexture();
            

            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)wrapMode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)wrapMode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)wrapMode);

            if (gen_mipmaps) GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            Texture result = new Texture(texture, image.Width, image.Height);
            texCache.Add(path, result);
            return result;
        }
    }
}
