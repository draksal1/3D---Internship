using ESCDX;
using SharpDX.Direct3D11;
using SharpDX;
using Shaders;
using SharpDX.DXGI;

namespace Geometry
{
    internal class GridScene : Scene
    {
        MashGridGenerator _grid;

        private Vector3 _objPos = new Vector3(-0.5f, -0.5f, -1.0f);
        Matrix _mvp;

        DepthStencilView _depthStencilView;


        public GridScene(Engine engine, Camera camera)
        {
            _engine = engine;
            _camera = camera;

            InputElement[] input = new[]
{
                        new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                        new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
            };
            _engine.SetInputFormat(input);

            OnLoad();
        }
        protected override void OnLoad()
        {
            // Prepare All the stages
            _grid = new MashGridGenerator(_engine.GetDevice(), 10);

            _engine.SetVerticesAndIndeces(new VertexBufferBinding(_grid.GetVertices(), 32, 0), _grid.GetIndeces(), 
                SharpDX.Direct3D.PrimitiveTopology.TriangleList);

            // Set constant buffer on 0 register


            _engine.CreateConstantBuffer(0, Utilities.SizeOf<Matrix>());

            _mvp = Matrix.Translation(_objPos) * _camera.GetViewMatrix() * _camera.GetProjectionMatrix();
            _engine.UpdateMatrixBuffer(_mvp, 0);


            var rasterizerStateDesc = new RasterizerStateDescription()
            {
                CullMode = CullMode.None,
                FillMode = FillMode.Solid
            };

            var depthBufferDesc = new Texture2DDescription()
            {
                Width = _engine.GetWidth(),
                Height = _engine.GetWidth(),
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.D32_Float,
                SampleDescription = new SampleDescription(4, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            };

            using (var depthBuffer = new Texture2D(_engine.GetDevice(), depthBufferDesc))
            {
                var depthStencilDesc = new DepthStencilViewDescription()
                {
                    Format = Format.D32_Float,
                    Dimension = DepthStencilViewDimension.Texture2DMultisampled,
                    Flags = DepthStencilViewFlags.None
                };

                _depthStencilView = new DepthStencilView(_engine.GetDevice(), depthBuffer, depthStencilDesc);
            }

            _engine.SetDepthBuffer(_depthStencilView);

            // Creating Rasterizer with CullMode etc
            //RasterizerState rasterizerState = new RasterizerState(_engine.GetDevice(), rasterizerStateDesc);
            //_engine.SetRasterizer(rasterizerState);
            // Main loop
            UpdateMVP();
        }

        public override void Draw()
        {
            _engine.Clear();

            //TODO Drawing

            _engine.ClearDepth(_depthStencilView);

            _engine.DrawIndexed(_grid.GetIndexCount());

            _engine.SwapChain();
        }

        public override void UpdateBuffers()
        {
            UpdateMVP();
        }

        private void UpdateMVP()
        {
            _mvp = Matrix.Translation(_objPos) * _camera.GetViewMatrix() * _camera.GetProjectionMatrix();
            _engine.UpdateMatrixBuffer(_mvp, 0);
        }

        public override void Rotate(Matrix Rotation, bool s) { }

    }
}

