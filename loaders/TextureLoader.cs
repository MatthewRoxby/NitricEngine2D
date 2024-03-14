using OpenTK.Graphics.OpenGL4;
using StbImageSharp;
using System.Diagnostics;

namespace NitricEngine2D.loaders
{
    /// <summary>
    /// class to hold texture data
    /// </summary>
    public class Texture
    {
        public int id { get; private set; }
        public int width { get; private set; }
        public int height { get; private set; }

        /// <summary>
        /// creates an instance of a texture
        /// </summary>
        /// <param name="id">texture ID integer</param>
        /// <param name="width">texture width</param>
        /// <param name="height">texture height</param>
        public Texture(int id, int width, int height)
        {
            this.id = id;
            this.width = width;
            this.height = height;
        }
    }


    /// <summary>
    /// static loader to load images as OpenGL textures
    /// </summary>
    public static class TextureLoader
    {
        //cached texture IDs by file path
        private static Dictionary<string, Texture> texCache = new Dictionary<string, Texture>();

        /// <summary>
        /// delete all cached textures
        /// </summary>
        public static void CleanUp()
        {
            foreach(Texture t in texCache.Values)
            {
                GL.DeleteTexture(t.id);
            }
        }

        /// <summary>
        /// loads an image as an OpenGL texture object
        /// </summary>
        /// <param name="path">the path to the image</param>
        /// <param name="flip_y">should the image be flipped on y?</param>
        /// <param name="minFilter">the min filter of the image</param>
        /// <param name="magFilter">the mag filter of the image</param>
        /// <param name="wrapMode">the wrap mode of the image</param>
        /// <param name="gen_mipmaps">should mipmaps be generated?</param>
        /// <returns>the integer texture ID</returns>
        public static Texture? LoadTexture(string path, bool flip_y = true, TextureMinFilter minFilter = TextureMinFilter.Nearest, TextureMagFilter magFilter = TextureMagFilter.Nearest, TextureWrapMode wrapMode = TextureWrapMode.Repeat, bool gen_mipmaps = false)
        {
            //return the cached texture ID if it exists
            if (texCache.ContainsKey(path))
            {
                return texCache[path];
            }

            //set STBImage to flip on load
            StbImage.stbi_set_flip_vertically_on_load(flip_y ? 1 : 0);

            //check for file error
            if (!File.Exists(path))
            {
                Debug.WriteLine($"Texture file {path} not found!");
                return null;
            }

            //load the image
            ImageResult image = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);
            

            int texture = GL.GenTexture();
            
            //set texture data
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)wrapMode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)wrapMode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)wrapMode);

            //generate mipmaps
            if (gen_mipmaps) GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            Texture result = new Texture(texture, image.Width, image.Height);
            texCache.Add(path, result);
            return result;
        }
    }
}
