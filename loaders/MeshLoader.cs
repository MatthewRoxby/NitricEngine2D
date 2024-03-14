using OpenTK.Graphics.OpenGL4;

namespace NitricEngine2D.loaders
{
    /// <summary>
    /// static loader class to load meshes as OpenGL Vertex Array Objects
    /// </summary>
    public static class MeshLoader
    {
        //cached array and buffer indices that will be discarded later
        private static List<int> arrays = new List<int>();
        private static List<int> buffers = new List<int>();

        /// <summary>
        /// deletes any used vertex arrays and buffers
        /// </summary>
        public static void CleanUp()
        {
            GL.DeleteVertexArrays(arrays.Count, arrays.ToArray());
            GL.DeleteBuffers(buffers.Count, buffers.ToArray());
        }

        /// <summary>
        /// Loads mesh data into a Vertex Array Object
        /// </summary>
        /// <param name="vertices">list of vertex coordinates as floats</param>
        /// <param name="uvs">list of texture coordinates as floats</param>
        /// <param name="indices">list of element indices as uints</param>
        /// <returns>the VAO id as an integer</returns>
        public static int LoadMesh(float[] vertices, float[] uvs, uint[] indices)
        {
            //create a VAO
            int vao = GL.GenVertexArray();
            arrays.Add(vao);

            GL.BindVertexArray(vao);

            //Load each attribute
            LoadAttribute(0, vertices, 3);
            LoadAttribute(1, uvs, 2);

            //create an EBO
            int ebo = GL.GenBuffer();

            //load EBO data
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            return vao;
        }

        /// <summary>
        /// Loads an attribute to a VAO through a buffer
        /// </summary>
        /// <param name="attribute">the integer position of the attribute</param>
        /// <param name="data">the data to be loaded</param>
        /// <param name="numItems">the number of elements per item e.g. a vec3 has 3 elements</param>
        /// <param name="normalised">is the data normalised?</param>
        private static void LoadAttribute(int attribute, float[] data, int numItems, bool normalised = false)
        {
            //create a VBO
            int vbo = GL.GenBuffer();

            arrays.Add(vbo);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

            GL.EnableVertexAttribArray(attribute);

            //load VBO data
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, BufferUsageHint.StaticDraw);
            //assign VBO to currently bound VAO
            GL.VertexAttribPointer(attribute, numItems, VertexAttribPointerType.Float, normalised, numItems * sizeof(float), 0);
        }
    }
}
