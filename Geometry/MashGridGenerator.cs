using SharpDX.Direct3D11;
using SharpDX;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;


namespace Shaders
{
    internal class MashGridGenerator
    {
        private Buffer _vertices;
        private Buffer _indeces;
        private int _vertexCount;
        private int _indexCount;
        private int _gridSize;

        public MashGridGenerator(Device device, int gridSize)
        {
            _gridSize = gridSize;
            GenerateVertices(device, 0.1f);
            GenerateIndices(device);
        }

        public Buffer GetVertices()
        {
            return _vertices;
        }

        public Buffer GetIndeces()
        {
            return _indeces;
        }

        public int GetVertexCount()
        {
            return _vertexCount;
        }

        public int GetIndexCount()
        {
            return _indexCount;
        }


        private void GenerateVertices(Device device, float size)
        {
            Vector4[] vertices = new Vector4[_gridSize * _gridSize * _gridSize * 8 * 2];

            Vector4 stepK = new Vector4(0.0f, size * 2, 0.0f, 0.0f);
            Vector4 stepi = new Vector4(size * 2, 0.0f, 0.0f, 0.0f);
            Vector4 stepj = new Vector4(0.0f, 0.0f, size * 2, 0.0f);
            Vector4 color;
            Vector4 step;

            int shift;

            for (int k = 0; k < _gridSize; k++)
            {
                for (int i = 0; i < _gridSize; i++)
                {
                    for (int j = 0; j < _gridSize; j++)
                    {

                        step = k * stepK + i * stepi + j * stepj;
                        color = new Vector4((float)(k + 1) / (float)_gridSize,
                            1.0f - (float)(j + 1) / (float)_gridSize,
                            (float)(k + 1) / (float)_gridSize, 1.0f);

                        shift = (k * _gridSize * _gridSize + i * _gridSize + j) * 16;

                        vertices[shift + 0] = new Vector4(size, size, size, 1.0f) + step;
                        vertices[shift + 1] = color;

                        vertices[shift + 2] = new Vector4(-size, size, size, 1.0f) + step;
                        vertices[shift + 3] = color;

                        vertices[shift + 4] = new Vector4(-size, size, -size, 1.0f) + step;
                        vertices[shift + 5] = color;

                        vertices[shift + 6] = new Vector4(size, size, -size, 1.0f) + step;
                        vertices[shift + 7] = color;

                        vertices[shift + 8] = new Vector4(size, -size, size, 1.0f) + step;
                        vertices[shift + 9] = color;

                        vertices[shift + 10] = new Vector4(-size, -size, size, 1.0f) + step;
                        vertices[shift + 11] = color;

                        vertices[shift + 12] = new Vector4(-size, -size, -size, 1.0f) + step;
                        vertices[shift + 13] = color;

                        vertices[shift + 14] = new Vector4(size, -size, -size, 1.0f) + step;
                        vertices[shift + 15] = color;
                    }
                }
            }
            _vertexCount = vertices.Length;
            _vertices = Buffer.Create(device, BindFlags.VertexBuffer, vertices);
        }

        private void GenerateIndices(Device device)
        {
            int[] indeces = new int[6 * 36 * _gridSize * _gridSize];

            int i, j, k;

            int shift;

            for (int side = 0; side < 2; side++)
            {
                for (int f = 0; f < _gridSize; f++)
                {
                    for (int s = 0; s < _gridSize; s++)
                    {
                        k = side * (_gridSize - 1);
                        i = f;
                        j = s;

                        shift = (side * _gridSize * _gridSize + f * _gridSize + s) * 36 * 3;

                        int term = 8 * (k * _gridSize * _gridSize + i * _gridSize + j);
                        GenerateQuadIndeces(indeces, shift, term);

                        k = f;
                        i = side * (_gridSize - 1);
                        j = s;
                        term = 8 * (k * _gridSize * _gridSize + i * _gridSize + j);
                        GenerateQuadIndeces(indeces, shift + 36, term);

                        k = f;
                        i = s;
                        j = side * (_gridSize - 1);
                        term = 8 * (k * _gridSize * _gridSize + i * _gridSize + j);
                        GenerateQuadIndeces(indeces, shift + 72, term);
                    }
                }
            }

            _indexCount = indeces.Length;
            _indeces = Buffer.Create(device, BindFlags.IndexBuffer, indeces);
        }


        private void GenerateQuadIndeces(int[] indeces, int startIndex, int increase)
        {


            int index = startIndex;

            for (int j = 0; j < 5; j += 4)
            {
                int i = j == 0 ? 1 : 0;
                indeces[index++] = j + increase;
                indeces[index++] = j + 1 + i + increase;
                indeces[index++] = j + 2 - i + increase;

                indeces[index++] = j + increase;
                indeces[index++] = j + 2 + i + increase;
                indeces[index++] = j + 3 - i + increase;
            }

            for (int i = 0; i < 4; i++)
            {
                indeces[index++] = i + increase;
                indeces[index++] = ((i + 5) > 7 ? 4 : (i + 5)) + increase;
                indeces[index++] = i + 4 + increase;

                indeces[index++] = i + increase;
                indeces[index++] = ((i + 1) > 3 ? 0 : (i + 1)) + increase;
                indeces[index++] = ((i + 5) > 7 ? 4 : (i + 5)) + increase;
            }


        }
    }
}