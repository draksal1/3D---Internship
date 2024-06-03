using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;

namespace ESCDX
{
    public class Shader
    {
        private CompilationResult _pixelShaderByteCode;

        private CompilationResult _vertexShaderByteCode;

        private VertexShader _vertexShader;

        private PixelShader _pixelShader;



        public Shader(string path, Device device)
        {
            _vertexShaderByteCode = ShaderBytecode.CompileFromFile(path, "VS", "vs_4_0",
            ShaderFlags.None, EffectFlags.None);

            _vertexShader = new VertexShader(device, _vertexShaderByteCode);

            _pixelShaderByteCode = ShaderBytecode.CompileFromFile(path, "PS", "ps_4_0",
                ShaderFlags.None, EffectFlags.None);

            _pixelShader = new PixelShader(device, _pixelShaderByteCode);
        }

        public CompilationResult GetVertexShaderByteCode()
        {
            return _vertexShaderByteCode;
        }

        public CompilationResult GetPixelShaderByteCode()
        {
            return _pixelShaderByteCode;
        }

        public VertexShader GetVertexShader()
        {
            return _vertexShader;
        }

        public PixelShader GetPixelShader()
        {
            return _pixelShader;
        }

        public void Dispose()
        {
            _vertexShaderByteCode.Dispose();
            _vertexShader.Dispose();
            _pixelShaderByteCode.Dispose();
            _pixelShader.Dispose();
        }
    }
}