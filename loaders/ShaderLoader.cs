using OpenTK.Graphics.OpenGL4;
using System.Diagnostics;

namespace NitricEngine2D.loaders
{

    public static class ShaderLoader
    {
        private static Dictionary<string, int> programCache = new Dictionary<string, int>();

        public static void CleanUp()
        {
            foreach(int program in programCache.Values)
            {
                GL.DeleteProgram(program);
            }
        }

        public static int LoadShader(string vertexPath, string fragmentPath)
        {
            if(programCache.ContainsKey(vertexPath + "_" + fragmentPath))
            {
                return programCache[vertexPath + "_" + fragmentPath];
            }


            string vertexCode;

            using(StreamReader reader = new StreamReader(vertexPath))
            {
                vertexCode = reader.ReadToEnd();
            }

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexCode);

            GL.CompileShader(vertexShader);

            string vertexError = GL.GetShaderInfoLog(vertexShader);

            if(vertexError != String.Empty)
            {
                Debug.WriteLine(vertexError);
            }

            string fragmentCode;

            using (StreamReader reader = new StreamReader(fragmentPath))
            {
                fragmentCode = reader.ReadToEnd();
            }

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentCode);

            GL.CompileShader(fragmentShader);

            string fragmentError = GL.GetShaderInfoLog(fragmentShader);

            if (fragmentError != String.Empty)
            {
                Debug.WriteLine(fragmentError);
            }

            int program = GL.CreateProgram();

            

            GL.AttachShader(program, vertexShader);
            GL.AttachShader(program, fragmentShader);
            GL.LinkProgram(program);

            string programError = GL.GetProgramInfoLog(program);

            if(programError != String.Empty)
            {
                Debug.WriteLine(programError);
            }

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            programCache.Add(vertexPath + "_" + fragmentPath, program);

            return program;
        }
    }
}
