using SharpDX;
using System;

namespace GridVisualization.GridClasses
{
    public class Grid
    {
        private CellGridLayer[] _layers;

        private int[] _size;

        private Dictionary<string, float[,,]> _properties;

        private string _name;

        private int _indexCount;

        public Grid()
        {
            _properties = new Dictionary<string, float[,,]>();
        }

        public void SetSize(int[] size)
        {
            _size = size;
            _layers = new CellGridLayer[GetSizeY()];
        }

        public void SetLayer(CellGridLayer layer, int k)
        {
            if (k < 0 && k >= GetSizeY())
            {
                return;
            }

            _layers[k] = layer;
        }

        public int GetSizeX()
        {
            return _size[0];
        }

        public int GetSizeY()
        {
            return _size[2];
        }

        public int GetSizeZ()
        {
            return _size[1];
        }

        public string[] GetProperties()
        {
            return _properties.Keys.ToArray();
        }

        public Vector3 GetCenter()
        {
            Vector4 firstPoint = _layers[GetSizeY() - 1].cells[GetSizeX() - 1, GetSizeZ() - 1].verticles[5];

            Vector4 secondPoint = _layers[0].cells[0, 0].verticles[0];

            return new Vector3((firstPoint.X + secondPoint.X) / 2, (firstPoint.Y + secondPoint.Y) / 2,
                (firstPoint.Z + secondPoint.Z) / 2);
        }

        public void SetProperty(string name)
        {
            _name = name;
        }

        public void AddProperty(string name, float[,,] values)
        {
            _properties.Add(name, Normalize(values));
        }

        public Vector4[] GetGridVerticles(string name)
        {
            _name = name;

            int index = 0;

            Vector4[] verticles = new Vector4[(GetSizeX() * GetSizeZ() +
                GetSizeX() * GetSizeY() +
                GetSizeZ() * GetSizeY()) * 2 * 16];

            GetYSliceVerticles(0).CopyTo(verticles, index);

            index += GetSizeZ() * GetSizeX() * 8 * 2;

            GetYSliceVerticles(GetSizeY() - 1).CopyTo(verticles, index);

            index += GetSizeZ() * GetSizeX() * 8 * 2;

            GetXSliceVerticles(0).CopyTo(verticles, index);

            index += GetSizeZ() * GetSizeY() * 8 * 2;

            GetXSliceVerticles(GetSizeX() - 1).CopyTo(verticles, index);

            index += GetSizeZ() * GetSizeY() * 8 * 2;

            GetZSliceVerticles(0).CopyTo(verticles, index);

            index += GetSizeY() * GetSizeX() * 8 * 2;

            GetZSliceVerticles(GetSizeZ() - 1).CopyTo(verticles, index);

            return verticles;
        }

        public Vector4[] GetSlicesVerticles(int sliceX, int sliceY, int sliceZ)
        {
            Vector4[] verticles = new Vector4[(GetSizeX() * GetSizeZ() * ((sliceY == -1) ? 0 : 1) +
                GetSizeX() * GetSizeY() * ((sliceZ == -1) ? 0 : 1) +
                GetSizeZ() * GetSizeY() * ((sliceX == -1) ? 0 : 1)) * 16];

            int index = 0;
            if (sliceY != -1)
            {
                GetYSliceVerticles(sliceY).CopyTo(verticles, index);
                index += GetSizeZ() * GetSizeX() * 8 * 2;
            }

            if (sliceX != -1)
            {
                GetXSliceVerticles(sliceX).CopyTo(verticles, index);
                index += GetSizeZ() * GetSizeY() * 8 * 2;
            }

            if (sliceZ != -1)
            {
                GetZSliceVerticles(sliceZ).CopyTo(verticles, index);
            }
            return verticles;

        }

        public int[] GetIndeces(int XslicesCount, int YslicesCount, int ZslicesCount)
        {
            int size = 36 * (GetSizeX() * GetSizeZ() * YslicesCount
                + GetSizeX() * GetSizeY() * ZslicesCount
                + GetSizeY() * GetSizeZ() * XslicesCount);
            _indexCount = size;
            int[] indeces = new int[size];

            int increase = 0;
            int shift = 0;

            GetSliceIndexes(YslicesCount, GetSizeX(), GetSizeZ(), shift).CopyTo(indeces, increase);

            increase += GetSizeX() * GetSizeZ() * YslicesCount * 8;

            shift += GetSizeX() * GetSizeZ() * YslicesCount * 36;

            GetSliceIndexes(XslicesCount, GetSizeZ(), GetSizeY(), increase).CopyTo(indeces, shift);

            increase += GetSizeX() * GetSizeY() * XslicesCount * 8;

            shift += GetSizeX() * GetSizeY() * XslicesCount * 36;

            GetSliceIndexes(ZslicesCount, GetSizeX(), GetSizeY(), increase).CopyTo(indeces, shift);

            return indeces;
        }

        public int GetIndexCount()
        {
            return _indexCount;
        }

        public Vector4[] GetYSliceVerticles(int k)
        {
            return _layers[k].GetVerticles(k, _properties[_name]);
        }

        public Vector4[] GetXSliceVerticles(int i)
        {
            Vector4[] verticles = new Vector4[GetSizeY() * GetSizeZ() * 16];

            for (int k = 0; k < _layers.Length; k++)
            {
                _layers[k].GetLineZ(i, k, _properties[_name]).CopyTo(verticles, k * GetSizeZ() * 16);
            }

            return verticles;
        }

        public Vector4[] GetZSliceVerticles(int j)
        {
            Vector4[] verticles = new Vector4[GetSizeY() * GetSizeX() * 16];

            for (int k = 0; k < _layers.Length; k++)
            {
                _layers[k].GetLineX(j, k, _properties[_name]).CopyTo(verticles, k * GetSizeX() * 16);
            }

            return verticles;
        }


        private int[] GetSliceIndexes(int countSlices, int width, int height, int startIncrease)
        {
            int[] indeces = new int[countSlices * width * height * 36];
            int shift, term;
            for (int k = 0; k < countSlices; k++)
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {

                        term = (k * width * height + i * height + j) * 8;

                        shift = term * 36 / 8;
                        GenerateQuadIndeces(indeces, shift, term + startIncrease);
                    }
                }
            }
            return indeces;
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

        private float[,,] Normalize(float[,,] values)
        {
            int depth = values.GetLength(0);
            int rows = values.GetLength(1);
            int cols = values.GetLength(2);

            float max = FindMaxValue(values);

            float[,,] normalizedValues = new float[depth, rows, cols];

            for (int d = 0; d < depth; d++)
            {
                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        normalizedValues[d, r, c] = values[d, r, c] / max;
                    }
                }
            }
            return normalizedValues;
        }

        private float FindMaxValue(float[,,] array)
        {
            float max = float.MinValue;
            foreach (float value in array)
            {
                if (value > max)
                {
                    max = value;
                }
            }
            return max;
        }
    }
}
