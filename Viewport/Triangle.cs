using SharpDX.Direct3D11;
using SharpDX;
using ESCDX;
using SharpDX.DXGI;

using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;


namespace MyViewport
{
    public class Triangle
    {
        // Angle of rotation for the object.
        private float _rotationAngle = 0.0f;

        // Vertex buffer for storing vertex data in the graphics process.
        private Buffer _vertices;

        private Buffer _indexes;

        public Triangle(Engine engine)
        {
            _vertices = Buffer.Create(engine.GetDevice(), BindFlags.VertexBuffer, new[]
                {   // Position                          // Color
                    new Vector4(0.0f, 0.5f, 0.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                    new Vector4(0.5f, -0.5f, 0.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                    new Vector4(-0.5f, -0.5f, 0.0f, 1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f)
                }); ;

            _indexes = Buffer.Create(engine.GetDevice(), BindFlags.IndexBuffer, new[] { 0, 1, 2 });

            InputElement[] input = new[]
{
                        new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                        new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
            };
            engine.SetInputFormat(input);

            engine.SetVerticesAndIndeces(new VertexBufferBinding(_vertices, 32, 0), _indexes, 
                SharpDX.Direct3D.PrimitiveTopology.TriangleList);
        }

        public ref Buffer GetVertices()
        {
            return ref _vertices;
        }

        public ref Buffer GetIndexes()
        {
            return ref _indexes;
        }

        public float GetAngle()
        {
            return _rotationAngle;
        }

        public void UpdateTriangle()
        {
            _rotationAngle += 0.01f;
        }
    }
}
