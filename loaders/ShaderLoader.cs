using OpenTK.Graphics.OpenGL4;
using System.Diagnostics;

namespace NitricEngine2D.loaders
{
    /// <summary>
    /// A static loader for loading shaders
    /// </summary>
    public static class ShaderLoader
    {
        //cached shader program IDs by filename
        private static Dictionary<string, int> programCache = new Dictionary<string, int>();

        /// <summary>
        /// Deletes all cached program objects
        /// </summary>
        public static void CleanUp()
        {
            foreach(int program in programCache.Values)
            {
                GL.DeleteProgram(program);
            }
        }

        /// <summary>
        /// loads an OpenGL shader program
        /// </summary>
        /// <param name="vertexPath">path to the vertex shader</param>
        /// <param name="fragmentPath">path to the fragment shader</param>
        /// <returns>the program ID as an integer</returns>
        public static int LoadShader(string vertexPath, string fragmentPath)
        {
            //returns the cached program if it exists
            if(programCache.ContainsKey(vertexPath + "_" + fragmentPath))
            {
                return programCache[vertexPath + "_" + fragmentPath];
            }


            string vertexCode;
            //loads vertex code
            using(StreamReader reader = new StreamReader(vertexPath))
            {
                vertexCode = reader.ReadToEnd();
            }

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexCode);

            GL.CompileShader(vertexShader);

            string vertexError = GL.GetShaderInfoLog(vertexShader);

            //check for compilation error
            if(vertexError != String.Empty)
            {
                Debug.WriteLine(vertexError);
            }

            string fragmentCode;
            //loads fragment code
            using (StreamReader reader = new StreamReader(fragmentPath))
            {
                fragmentCode = reader.ReadToEnd();
            }

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentCode);

            GL.CompileShader(fragmentShader);

            string fragmentError = GL.GetShaderInfoLog(fragmentShader);

            //check for compilation error
            if (fragmentError != String.Empty)
            {
                Debug.WriteLine(fragmentError);
            }

            int program = GL.CreateProgram();

            
            //attach shaders to program
            GL.AttachShader(program, vertexShader);
            GL.AttachShader(program, fragmentShader);
            GL.LinkProgram(program);

            string programError = GL.GetProgramInfoLog(program);

            //check for link error
            if(programError != String.Empty)
            {
                Debug.WriteLine(programError);
            }

            //delete shaders once linked
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            programCache.Add(vertexPath + "_" + fragmentPath, program);

            return program;
        }
    }
}
