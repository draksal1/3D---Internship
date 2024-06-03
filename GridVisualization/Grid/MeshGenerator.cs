using SharpDX;

namespace GridVisualization.GridClasses
{
    public class MeshGenerator
    {
        private Vector4[] _verticles;

        private int[] _indexes;

        public void Update(Vector4[] verts, int count)
        {
            GenerateMesh(verts);
            GenerateIndexes(count);
        }

        public Vector4[] GetVerticles() { return _verticles; }

        public int[] GetIndexes() { return _indexes; }

        public int GetIndexCount() { return _indexes.Length; }
        private void GenerateMesh(Vector4[] verts)
        {
            _verticles = verts;

            for (int i = 1; i < verts.Length; i+=2)
            {
                _verticles[i] = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
            }
        }

        private void GenerateIndexes(int count)
        {
            _indexes = new int[count * 24];
            int index = 0;
            for (int i = 0; i < (count); i++)
            {
                for (int j = 0; j < 5; j +=4)
                {
                    _indexes[index++] = j + i * 8;
                    _indexes[index++] = j + 1 + i * 8;

                    _indexes[index++] = j + 1 + i * 8;
                    _indexes[index++] = j + 2 + i * 8;

                    _indexes[index++] = j + 2 + i * 8;
                    _indexes[index++] = j + 3 + i * 8;

                    _indexes[index++] = j + 3 + i * 8;
                    _indexes[index++] = j + 0 + i * 8;
                }

                for(int  j = 0; j < 4; j++)
                {
                    _indexes[index++] = j + i * 8;
                    _indexes[index++] = j + 4 + i * 8;
                }
            }
        }

    }
}
