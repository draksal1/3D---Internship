using SharpDX;

namespace GridVisualization.GridClasses
{
    public class CellGridLayer
    {
        public Cell[,] cells;

        public CellGridLayer() { }

        public Vector4[] GetVerticles(int k, float[,,] properties)
        {
            int size = cells.Length;
            Vector4[] verticles = new Vector4[size * 16];

            for (int i = 0; i < cells.GetLength(0); i++)
            {
                for (int j = 0; j < cells.GetLength(1); j++)
                {
                    cells[i, j].GetVerticles(properties[i, k, j]).CopyTo(verticles, 
                        (i * cells.GetLength(1) + j) * 16);
                }
            }
            return verticles;
        }
        public Vector4[] GetLineZ(int i, int k, float[,,] properties)
        {
            int size = cells.GetLength(1);
            Vector4[] verticles = new Vector4[size * 16];

            for (int j = 0; j < size; j++)
            {
                cells[i, j].GetVerticles(properties[i, k, j]).CopyTo(verticles, j * 16);
            }

            return verticles;
        }

        public Vector4[] GetLineX(int j, int k, float[,,] properties)
        {
            int size = cells.GetLength(0);
            Vector4[] verticles = new Vector4[size * 16];

            for (int i = 0; i < size; i++)
            {
                cells[i, j].GetVerticles(properties[i, k, j]).CopyTo(verticles, i * 16);
            }

            return verticles;
        }

    }
}
