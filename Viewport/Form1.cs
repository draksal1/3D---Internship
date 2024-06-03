using System.ComponentModel;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Windows;
using ESCDX;

using Device = SharpDX.Direct3D11.Device;

namespace MyViewport
{
    public partial class Form1 : Form
    {
        // Path to the shader file.
        private string _shaderWay = "..//..//..//Shader.fx";

        // Engine for processing graphics operations.
        private Engine _engine;

        public Triangle _triangle;
        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _engine = new Engine(this.Handle, _shaderWay, this.ClientSize.Width, this.ClientSize.Height, 4);

            Device device = _engine.GetDevice();

            _triangle = new Triangle(_engine);

            // Prepare All the stages
            _engine.SetVerticesAndIndeces(new VertexBufferBinding(_triangle.GetVertices(), 32, 0), _triangle.GetIndexes(), SharpDX.Direct3D.PrimitiveTopology.TriangleStrip);

            Matrix worldViewProjectionMatrix = Matrix.RotationX(0.0f);

            // Set constant buffer on 0 register
            _engine.CreateConstantBuffer(0, Utilities.SizeOf<Matrix>());

            _engine.UpdateMatrixBuffer(worldViewProjectionMatrix, 0);


            RasterizerStateDescription rasterizerStateDesc = new RasterizerStateDescription()
            {
                CullMode = CullMode.None,
                FillMode = FillMode.Solid,
                IsFrontCounterClockwise = false
            };

            // Creating Rasterizer with CullMode etc
            RasterizerState rasterizerState = new RasterizerState(device, rasterizerStateDesc);



            _engine.SetRasterizer(rasterizerState);
            // Main loop
            RenderLoop.Run(this, () =>
            {
                Matrix worldViewProjectionMatrix = Matrix.RotationY(_triangle.GetAngle());
                _triangle.UpdateTriangle();

                _engine.UpdateMatrixBuffer(worldViewProjectionMatrix, 0);

                _engine.Clear();
                _engine.DrawIndexed(3);
                _engine.SwapChain();
            });
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            // Release all resources
            _engine.Dispose();

            base.OnClosing(e);
        }
    }
}
