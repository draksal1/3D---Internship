using ESCDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX;
using GridVisualization.GridClasses;
using GridVisualization.Readers;
using Buffer = SharpDX.Direct3D11.Buffer;
using System;
using System.Reflection;

namespace GridVisualization
{
    internal class GridScene : Scene
    {


        private Vector3 _objPos = new Vector3(0.0f, 0.0f, 0.0f);

        private Matrix _mvp;

        private Matrix _rotation = Matrix.Identity;

        private DepthStencilView _depthStencilView;

        private string _gridPath = "..//..//..//Data/grid.bin";

        private string _propertiesPath = "..//..//..//Data/grid.binprops.txt";

        private Grid _grid;

        private MeshGenerator _mesh;

        private Buffer _gridVerts;

        private Buffer _gridIndexes;

        private Buffer _meshVerts;

        private Buffer _meshIndexes;

        private string _property;

        private bool _isMeshEnabled;



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
            _grid = GridBinReader.Read(_gridPath, 0.001f);

            _mesh = new MeshGenerator();

            GridBinReader.ReadProperties(_propertiesPath, _grid);

            _property = "Imported_PPRS";

            LoadGrid();

            // Set constant buffer on 0 register
            _objPos = -_grid.GetCenter();

            _engine.CreateConstantBuffer(0, Utilities.SizeOf<Matrix>());

            _engine.UpdateMatrixBuffer(_mvp, 0);


            var rasterizerStateDesc = new RasterizerStateDescription()
            {
                CullMode = CullMode.Front,
                FillMode = FillMode.Solid,
                IsFrontCounterClockwise = false,
                IsAntialiasedLineEnabled = true,
                IsMultisampleEnabled = true

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



            // Creating Rasterizer with CullMode etc
            RasterizerState rasterizerState = new RasterizerState(_engine.GetDevice(), rasterizerStateDesc);
            _engine.SetRasterizer(rasterizerState);
            // Main loop
            UpdateMVP();
            _engine.SetDepthBuffer(_depthStencilView);
        }

        public override void Draw()
        {
            _engine.Clear();

            _engine.ClearDepth(_depthStencilView);
            _engine.DrawIndexed(_grid.GetIndexCount());

            if (_isMeshEnabled)
            {
                _engine.SetVerticesAndIndeces(new VertexBufferBinding(_meshVerts, 32, 0), _meshIndexes, 
                    SharpDX.Direct3D.PrimitiveTopology.LineList);
                _engine.DrawIndexed(_mesh.GetIndexCount());

                _engine.SetVerticesAndIndeces(new VertexBufferBinding(_gridVerts, 32, 0), _gridIndexes,
                SharpDX.Direct3D.PrimitiveTopology.TriangleList);
            }
            _engine.SwapChain();
        }

        public void LoadGrid()
        {
            Vector4[] _verticles = _grid.GetGridVerticles(_property);

            _gridVerts = Buffer.Create(_engine.GetDevice(), BindFlags.VertexBuffer, _verticles);
            int[] _indexesTriangle = _grid.GetIndeces(2, 2, 2);
            _gridIndexes = Buffer.Create(_engine.GetDevice(), BindFlags.IndexBuffer, _indexesTriangle);


            _mesh.Update(_verticles, _grid.GetIndexCount() / 36);

            BindMesh();

            _engine.SetVerticesAndIndeces(new VertexBufferBinding(_gridVerts, 32, 0), _gridIndexes, 
                SharpDX.Direct3D.PrimitiveTopology.TriangleList);
        }

        public void LoadSlices(int sliceX, int sliceY, int sliceZ)
        {
            if (sliceX == -1 && sliceY == -1 && sliceZ == -1)
            {
                Vector4[] emptyVertices = new Vector4[1];
                _gridVerts = Buffer.Create(_engine.GetDevice(), BindFlags.VertexBuffer, emptyVertices);
                _gridIndexes = null;

                _meshVerts = Buffer.Create(_engine.GetDevice(), BindFlags.VertexBuffer, emptyVertices);
                _meshIndexes = null;

                _engine.SetVerticesAndIndeces(new VertexBufferBinding(_gridVerts, 32, 0), _gridIndexes,
                    SharpDX.Direct3D.PrimitiveTopology.TriangleList);

                return;
            }

            Vector4[] v = _grid.GetSlicesVerticles(sliceX, sliceY, sliceZ);
            _gridVerts = Buffer.Create(_engine.GetDevice(), BindFlags.VertexBuffer, v);

            int[] _indexesTriangle = _grid.GetIndeces((sliceX == -1) ? 0 : 1, (sliceY == -1) ? 0 : 1,
                (sliceZ == -1) ? 0 : 1);
            _gridIndexes = Buffer.Create(_engine.GetDevice(), BindFlags.IndexBuffer, _indexesTriangle);

            _mesh.Update(v, _grid.GetIndexCount() / 36);

            BindMesh();
            _engine.SetVerticesAndIndeces(new VertexBufferBinding(_gridVerts, 32, 0), _gridIndexes, 
                SharpDX.Direct3D.PrimitiveTopology.TriangleList);
        }

        public override void UpdateBuffers()
        {
            UpdateMVP();
        }

        public override void Rotate(Matrix Rotation, bool SetRotate = false)
        {
            if (SetRotate)
            {
                _rotation = Rotation;
            }
            else
            {
                _rotation *= Rotation;
            }
        }

        public string[] GetProperties()
        {
            return _grid.GetProperties();
        }

        public void ChangeProperty(string name)
        {
            _property = name;

            Vector4[] v = _grid.GetGridVerticles(_property);

            _gridVerts = Buffer.Create(_engine.GetDevice(), BindFlags.VertexBuffer, v);

            _engine.SetVerticles(new VertexBufferBinding(_gridVerts, 32, 0));
        }

        public Vector3 GetSizes()
        {
            return new Vector3(_grid.GetSizeX(), _grid.GetSizeY(), _grid.GetSizeZ());
        }

        private void UpdateMVP()
        {
            _mvp = Matrix.Translation(_objPos) * _rotation * _camera.GetViewMatrix() * _camera.GetProjectionMatrix();
            _engine.UpdateMatrixBuffer(_mvp, 0);
        }

        private void BindMesh()
        {
            _meshVerts = Buffer.Create(_engine.GetDevice(), BindFlags.VertexBuffer, _mesh.GetVerticles());
            _meshIndexes = Buffer.Create(_engine.GetDevice(), BindFlags.VertexBuffer, _mesh.GetIndexes());
        }

        public void SetMeshState(bool state)
        {
            _isMeshEnabled = state;
        }
    }
}