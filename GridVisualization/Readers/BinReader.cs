using GridVisualization.GridClasses;
using SharpDX;
using System.Globalization;

namespace GridVisualization.Readers
{
    public class GridBinReader
    {
        public static Grid Read(string path, float scale, float verticalScale = 1.0f)
        {

            int[] size = new int[3];

            Grid grid = new Grid();

            using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                size[0] = reader.ReadInt32();
                size[1] = reader.ReadInt32();
                size[2] = reader.ReadInt32();

                grid.SetSize(size);
                float x, y, z;

                for (int k = 0; k < size[2]; k++)
                {
                    CellGridLayer layer = new CellGridLayer();
                    layer.cells = new Cell[size[0], size[1]];

                    for (int i = 0; i < size[0]; i++)
                    {
                        for (int j = 0; j < size[1]; j++)
                        {
                            layer.cells[i, j] = new Cell();
                            layer.cells[i, j].active = reader.ReadBoolean();
                            for (int corner = 0; corner < 4; corner++)
                            {
                                x = reader.ReadSingle();
                                y = -reader.ReadSingle();
                                z = -reader.ReadSingle();

                                layer.cells[i, j].verticles[corner] = new Vector4(x * scale,
                                    y * scale * verticalScale,
                                    z * scale, 1.0f);

                                x = reader.ReadSingle();
                                y = -reader.ReadSingle();
                                z = -reader.ReadSingle();

                                layer.cells[i, j].verticles[corner + 4] = new Vector4(x * scale,
                                    y * scale * verticalScale,
                                    z * scale, 1.0f);

                            }
                        }

                    }
                    grid.SetLayer(layer, k);
                }
            }


            return grid;
        }

        public static void ReadProperties(string path, Grid grid)
        {
            var lines = File.ReadAllLines(path);
            int lineId = 0;
            int count = int.Parse(lines[lineId++], NumberStyles.Number, CultureInfo.InvariantCulture);
            for (int counter = 0; counter < count; counter++)
            {
                string name = lines[lineId++];
                var values = new float[grid.GetSizeX(), grid.GetSizeY(), grid.GetSizeZ()];
                for (int i = 0; i < grid.GetSizeX(); i++)
                {
                    for (int j = 0; j < grid.GetSizeZ(); j++)
                    {
                        for (int k = 0; k < grid.GetSizeY(); k++)
                        {
                            float value = float.Parse(lines[lineId++], NumberStyles.AllowExponent
                                | NumberStyles.Float,
                                CultureInfo.InvariantCulture);
                            values[i, k, j] = value;
                        }
                    }
                }

                grid.AddProperty(name, values);
            }
        }
    }
}