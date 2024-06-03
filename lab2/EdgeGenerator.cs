using OpenTK.Mathematics;

namespace Edge
{
    internal class EdgeGenerator
    {
        private float[] verticles;
        private int[] indices;
        private int N;
        public EdgeGenerator(int K)
        {
            verticles = new float[N * N * 6];
            indices = new int[(N - 1) * (N - 1) * 3 * 2];
            RegeneratEdge(K);
        }

        private void RegeneratEdge(int K)
        {
            if (N != K)
            {
                N = K;
                verticles = new float[N * N * 6];
                indices = new int[(N - 1) * (N - 1) * 3 * 2];
            }
            Vector3 start = new Vector3(-0.5f, -0.5f, 0.5f);
            Vector3 point1 = new Vector3(0.5f, -0.5f, 0.5f);
            Vector3 point2 = new Vector3(-0.5f, 0.5f, 0.5f);
            GenerateVerticles(ref start, ref point1, ref point2, N);
            GenerateIndices();
        }

        private void GenerateVerticles(ref Vector3 start, ref Vector3 point1, ref Vector3 point2, int N)
        {
            Vector3 normal = Vector3.Normalize(Vector3.Cross(point1 - start, point2 - start));

            float Xstart = start.X;
            float Ystart = start.Y;
            float Zstart = start.Z;

            float DX1 = (point1.X - Xstart) / (N - 1);
            float DY1 = (point1.Y - Ystart) / (N - 1);
            float DZ1 = (point1.Z - Zstart) / (N - 1);

            float DX2 = (point2.X - Xstart) / (N - 1);
            float DY2 = (point2.Y - Ystart) / (N - 1);
            float DZ2 = (point2.Z - Zstart) / (N - 1);

            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    verticles[(i * N + j) * 6] = Xstart + DX1 * i + DX2 * j;
                    verticles[(i * N + j) * 6 + 1] = Ystart + DY1 * i + DY2 * j;
                    verticles[(i * N + j) * 6 + 2] = Zstart + DZ1 * i + DZ2 * j;


                    verticles[(i * N + j) * 6 + 3] = normal.X;
                    verticles[(i * N + j) * 6 + 4] = normal.Y;
                    verticles[(i * N + j) * 6 + 5] = normal.Z;

                }
            }
        }

        private void GenerateIndices()
        {
            for (int i = 0; i <= N - 2; i++)
            {
                for (int j = 0; j <= N - 2; j++)
                {
                    indices[((N - 1) * i + j) * 3] = i * N + j;
                    indices[((N - 1) * i + j) * 3 + 2] = i * N + j + 1;
                    indices[((N - 1) * i + j) * 3 + 1] = (i + 1) * N + j;
                }
            }
            for (int i = 0; i <= N - 2; i++)
            {
                for (int j = 0; j <= N - 2; j++)
                {
                    indices[((N - 1) * i + j) * 3 + (N - 1) * (N - 1) * 3] = (i + 1) * N + j;
                    indices[((N - 1) * i + j) * 3 + 2 + (N - 1) * (N - 1) * 3] =
                        i * N + j + 1;
                    indices[((N - 1) * i + j) * 3 + 1 + (N - 1) * (N - 1) * 3] =
                        (i + 1) * N + j + 1;
                }
            }
        }

        public float[] GetVerticles() { return verticles; }

        public int[] GetIndices() { return indices; }
    }
}
