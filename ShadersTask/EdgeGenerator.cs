using SharpDX.Direct3D11;
using SharpDX;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace Shaders
{
    public class Edge
    {
        private Buffer _vertices;
        private Buffer _indices;
        private int _vertexCount;
        private int _indexCount;

        public Edge(Device device, Vector4 color, float size)
        {
            GenerateVerticesAndIndices(device, color, size);
        }

        private void GenerateVerticesAndIndices(Device device, Vector4 color, float size)
        {

            Vector4 normal = new Vector4(0.0f, 0.0f, 1.0f, 0.0f);

            Vector4 stepX = new Vector4(size, 0.0f, 0.0f, 0.0f);

            Vector4 stepY = new Vector4(0.0f, size, 0.0f, 0.0f);

            Vector4 start = new Vector4(-size / 2, -size / 2, size / 2, 1.0f);

            Vector4[] vertices = new Vector4[4 * 3];

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    vertices[(i * 2 + j) * 3 + 0] = start + j * stepX + i * stepY;

                    vertices[(i * 2 + j) * 3 + 1] = color;

                    vertices[(i * 2 + j) * 3 + 2] = normal;
                }
            }

            _vertexCount = vertices.Length / 3;

            // Индексы для TRIANGLELIST
            int[] indices = new int[3 * 2]
            {
                // Front face
                0, 2, 1,
                1, 2, 3,
            };

            _indexCount = indices.Length;

            // Создаем буфер вершин
            _vertices = Buffer.Create(device, BindFlags.VertexBuffer, vertices);

            // Создаем буфер индексов
            _indices = Buffer.Create(device, BindFlags.IndexBuffer, indices);
        }

        public Buffer GetVertices()
        {
            return _vertices;
        }

        public Buffer GetIndices()
        {
            return _indices;
        }

        public int GetVertexCount()
        {
            return _vertexCount;
        }

        public int GetIndexCount()
        {
            return _indexCount;
        }
    }
}
