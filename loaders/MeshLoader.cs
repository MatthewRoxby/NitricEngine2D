using OpenTK.Graphics.OpenGL4;

namespace NitricEngine2D.loaders
{
    public static class MeshLoader
    {
        private static List<int> arrays = new List<int>();
        private static List<int> buffers = new List<int>();

        public static void CleanUp()
        {
            GL.DeleteVertexArrays(arrays.Count, arrays.ToArray());
            GL.DeleteBuffers(buffers.Count, buffers.ToArray());
        }

        public static int LoadMesh(float[] vertices, float[] uvs, uint[] indices)
        {
            int vao = GL.GenVertexArray();
            arrays.Add(vao);

            GL.BindVertexArray(vao);

            LoadAttribute(0, vertices, 3);
            LoadAttribute(1, uvs, 2);

            int ebo = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            return vao;
        }

        private static void LoadAttribute(int attribute, float[] data, int numItems, bool normalised = false)
        {
            int vbo = GL.GenBuffer();

            arrays.Add(vbo);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

            GL.EnableVertexAttribArray(attribute);

            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(attribute, numItems, VertexAttribPointerType.Float, normalised, numItems * sizeof(float), 0);
        }
    }
}
