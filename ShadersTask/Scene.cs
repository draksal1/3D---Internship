using SharpDX;
using SharpDX.Direct3D11;
using ESCDX;
using SharpDX.DXGI;


namespace Shaders
{
    internal class MyScene : Scene
    {
        private Edge _cube;

        LightBufferData _light;

        private MVPData _mvp;

        private Vector4 _cameraPos;

        DepthStencilView _depthStencilView;

        public MyScene(Engine engine, Camera camera)
        {
            _engine = engine;
            _camera = camera;
            _cube = new Edge(_engine.GetDevice(), new Vector4(1.0f, 1.0f, 1.0f, 1.0f), 1.0f);

            InputElement[] input = new[]
{
                        new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                        new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0),
                        new InputElement("NORMAL", 0, Format.R32G32B32A32_Float, 32, 0)
             };

            _engine.SetInputFormat(input);

            OnLoad();
        }
        protected override void OnLoad()
        {

            _engine.SetVerticesAndIndeces(new VertexBufferBinding(_cube.GetVertices(), 48, 0), _cube.GetIndices(), SharpDX.Direct3D.PrimitiveTopology.TriangleList);

            _engine.CreateConstantBuffer(0, Utilities.SizeOf<MVPData>());

            _engine.CreateConstantBuffer(1, Utilities.SizeOf<LightBufferData>());
            _light = new LightBufferData();
            _light.LightColor = new Vector4(0.0f, 0.5f, 1.0f, 1.0f);
            _light.LightPos = new Vector4(1.0f, 1.0f, 3.0f, 1.0f);
            _engine.UpdateLightBuffer(_light, 1);

            _engine.CreateConstantBuffer(2, Utilities.SizeOf<Vector4>());
            UpdateCameraPosBuffer();


            RasterizerStateDescription rasterizerStateDesc = new RasterizerStateDescription()
            {
                CullMode = CullMode.Front,
                FillMode = FillMode.Solid,
                IsFrontCounterClockwise = false
            };

            var depthBufferDesc = new Texture2DDescription()
            {
                Width = _engine.GetWidth(),
                Height = _engine.GetHeight(),
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
                    Flags = DepthStencilViewFlags.None,
                    
                    
                };

                _depthStencilView = new DepthStencilView(_engine.GetDevice(), depthBuffer, depthStencilDesc);
            }
            _engine.SetDepthBuffer(_depthStencilView);
            // Creating Rasterizer with CullMode etc
            RasterizerState rasterizerState = new RasterizerState(_engine.GetDevice(), rasterizerStateDesc);
            _engine.SetRasterizer(rasterizerState);
            // Main loop
            UpdateMVP();
        }

        public override void Draw()
        {
            _engine.Clear();

            _engine.ClearDepth(_depthStencilView);

            Vector3 objPos = new Vector3(0.0f, 0.0f, 1.0f);

            DrawCube(objPos);

            objPos = new Vector3(2.0f, 0.0f, 1.0f);

            DrawCube(objPos);

            _engine.SwapChain();
        }

        public override void UpdateBuffers()
        {
            UpdateMVP();
            UpdateCameraPosBuffer();
        }

        private void UpdateMVP()
        {

            _mvp.Projection = _camera.GetProjectionMatrix();
            _mvp.View = _camera.GetViewMatrix();
            _engine.UpdateMVPBuffer(ref _mvp, 0);
            _cameraPos = new Vector4(_camera.Position, 1.0f);
            _engine.UpdateVector4Buffer(ref _cameraPos, 2);
        }

        private void UpdateCameraPosBuffer()
        {
            _cameraPos = new Vector4(_camera.Position, 1.0f);
            _engine.UpdateVector4Buffer(ref _cameraPos, 2);
        }

        private void DrawCube(Vector3 objPos)
        {
            _mvp.Model = Matrix.Translation(objPos);
            UpdateMVP();
            _engine.DrawIndexed(_cube.GetIndexCount());


            for (int i = 0; i < 3; i++)
            {
                _mvp.Model = Matrix.RotationY(MathUtil.DegreesToRadians(90.0f)) * _mvp.Model;
                UpdateMVP();
                _engine.DrawIndexed(_cube.GetIndexCount());
            }

            _mvp.Model = Matrix.RotationX(MathUtil.DegreesToRadians(90.0f)) * _mvp.Model;
            UpdateMVP();
            _engine.DrawIndexed(_cube.GetIndexCount());

            _mvp.Model = Matrix.RotationX(MathUtil.DegreesToRadians(180.0f)) * _mvp.Model;
            UpdateMVP();
            _engine.DrawIndexed(_cube.GetIndexCount());
        }

        public override void Rotate(Matrix Rotation, bool setRotate = false){}
    }
}
