using SharpDX;

namespace GridVisualization.GridClasses
{
    public class Cell
    {
        public bool active;

        public Vector4[] verticles;

        public Cell()
        {
            verticles = new Vector4[8];
        }

        public Vector4[] GetVerticles(float property)
        {
            Vector4[] v = new Vector4[8 * 2];

            Vector4 color = getColor(property);

            for (int i = 0; i < verticles.Length; i++)
            {
                v[i * 2] = verticles[i];

                v[i * 2 + 1] = color;
            }

            return v;
        }

        private Vector4 getColor(float color)
        {
            float hue = ((1.0f - color) * 240f) % 240f;
            float saturation = 1.0f;
            float value = 1.0f;

            int hi = (int)Math.Floor(hue / 60) % 6;
            float f = hue / 60 - (float)Math.Floor(hue / 60);

            float v = value;
            float p = value * (1 - saturation);
            float q = value * (1 - f * saturation);
            float t = value * (1 - (1 - f) * saturation);

            switch (hi)
            {
                case 0:
                    return new Vector4(v, t, p, 1.0f);
                case 1:
                    return new Vector4(q, v, p, 1.0f);
                case 2:
                    return new Vector4(p, v, t, 1.0f);
                case 3:
                    return new Vector4(p, q, v, 1.0f);
                case 4:
                    return new Vector4(t, p, v, 1.0f);
                default:
                    return new Vector4(v, p, q, 1.0f);
            }
        }
    }
}
