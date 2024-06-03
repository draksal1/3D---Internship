using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;
using Buffer = SharpDX.Direct3D11.Buffer;
using SharpDX.D3DCompiler;

namespace ESCDX
{
    public struct LightBufferData
    {
        public Vector4 LightPos;
        public Vector4 LightColor;
    }

    public struct MVPData
    {
        public Matrix Model;
        public Matrix View;
        public Matrix Projection;
    }

    public class Engine
    {

        private Device _device;

        private SwapChain _swapChain;

        private Shader _shader;

        private DeviceContext _context;

        private Dictionary<int, Buffer> _buffers = new Dictionary<int, Buffer>();

        private Texture2D _backBuffer;

        private InputLayout _layout;

        private RenderTargetView _renderView;

        private int _width;
        private int _height;
        public Engine(nint handle, string shaderWay, int width, int height, int antialiasing)
        {
            var desc = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription = new ModeDescription(width, height,
                            new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = handle,
                SampleDescription = new SampleDescription(antialiasing, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };
            _width = width;

            _height = height;

            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, desc,
            out _device, out _swapChain);

            _context = _device.ImmediateContext;

            _shader = new Shader(shaderWay, _device);

            _backBuffer = Texture2D.FromSwapChain<Texture2D>(_swapChain, 0);

            _renderView = new RenderTargetView(_device, _backBuffer);

            _context.OutputMerger.SetTargets(_renderView);

            _context.Rasterizer.SetViewport(new Viewport(0, 0, width,
                height, 0.0f, 1.0f));

            _context.VertexShader.Set(_shader.GetVertexShader());

            _context.PixelShader.Set(_shader.GetPixelShader());

        }

        public void SetInputFormat(InputElement[] input)
        {
            _layout = new InputLayout(
            _device,
            ShaderSignature.GetInputSignature(_shader.GetVertexShaderByteCode()),
            input);

            _context.InputAssembler.InputLayout = _layout;
        }

        public void SetVerticesAndIndeces(VertexBufferBinding vertices, Buffer indeces, PrimitiveTopology topology)
        {
            _context.InputAssembler.PrimitiveTopology = topology;
            _context.InputAssembler.SetVertexBuffers(0, vertices);
            _context.InputAssembler.SetIndexBuffer(indeces, Format.R32_UInt, 0);
        }

        public void SetVerticles(VertexBufferBinding vertices)
        {
            _context.InputAssembler.SetVertexBuffers(0, vertices);
        }

        public void CreateConstantBuffer(int reg, int size)
        {

            Buffer constantBuffer = new Buffer(_device,
            size,
            ResourceUsage.Default,
            BindFlags.ConstantBuffer,
            CpuAccessFlags.None,
            ResourceOptionFlags.None,
            0);

            _buffers.Add(reg, constantBuffer);

            _context.VertexShader.SetConstantBuffer(reg, _buffers[reg]);
        }

        public void UpdateMVPBuffer(ref MVPData matrix, int reg)
        {
            _context.UpdateSubresource(ref matrix, _buffers[reg]);
        }

        public void UpdateLightBuffer(LightBufferData light, int reg)
        {
            _context.UpdateSubresource(ref light, _buffers[reg]);
        }

        public void UpdateMatrixBuffer(Matrix mat, int reg)
        {
            _context.UpdateSubresource(ref mat, _buffers[reg]);
        }

        public void UpdateVector4Buffer(ref Vector4 vect, int reg)
        {
            _context.UpdateSubresource(ref vect, _buffers[reg]);
        }

        public void SetRasterizer(RasterizerState rasterizerState)
        {
            _context.Rasterizer.State = rasterizerState;
        }

        public void SetDepthBuffer(DepthStencilView depth)
        {
            _context.OutputMerger.SetTargets(depth, _renderView);

        }
        public void ClearDepth(DepthStencilView depth)
        {
            _context.ClearDepthStencilView(depth, DepthStencilClearFlags.Depth, 1.0f, 0);
        }

        public void Clear()
        {
            _context.ClearRenderTargetView(_renderView, SharpDX.Color.White);
        }

        public void DrawVerticels(int count, int start)
        {
            _context.Draw(count, start);
        }

        public void DrawIndexed(int count)
        {
            _context.DrawIndexed(count, 0, 0);
        }

        public void SwapChain()
        {
            _swapChain.Present(0, PresentFlags.None);
        }

        public ref Device GetDevice()
        {
            return ref _device;
        }

        public int GetWidth()
        {
            return _width;
        }

        public int GetHeight()
        {
            return _height;
        }

        public void Dispose()
        {
            _layout.Dispose();
            _renderView.Dispose();
            _backBuffer.Dispose();
            _context.Dispose();
            _swapChain.Dispose();
            _shader.Dispose();
            _device.Dispose();
        }
    }
}