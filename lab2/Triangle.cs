using Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using Edge;

namespace Window3D
{

    public class Window : GameWindow
    {
        private Vector3 _lightPos = new Vector3(1.0f, 0.4f, 1.3f);

        private readonly string _shadersWay = ".//..//..//..//Shaders//";

        private readonly string _commonFilesWay = ".//..//..//..//..//CommonFiles//";

        private int _vertexBufferObject;

        private readonly Shader _shader;

        private readonly Camera _camera;

        private int _vaoModel;

        private int _vaoLamp;

        private Shader _lightShader;

        private bool _firstMove = true;

        private Vector2 _lastPos;

        private readonly EdgeGenerator _edgeGenerator = new EdgeGenerator(5);

        readonly ImGuiOverlay _imGuiWindow = new ImGuiOverlay();

        private LightType _selectedLight = LightType.FlashLight;

        int EBO;

        public Window(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings()
        { ClientSize = (width, height), Title = title }) 
        {
            _camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);
            _lightShader = new Shader(_shadersWay + "flashLight.vert", _shadersWay + "ModelShader.frag");
            _shader = new Shader(_shadersWay + "PointLight.vert", _commonFilesWay + "shader.frag");
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            _imGuiWindow.Run();

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            GL.Enable(EnableCap.DepthTest);

            float[] _vertices = _edgeGenerator.GetVerticles();
            int[] _indices = _edgeGenerator.GetIndices();

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            EBO = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO); // Corrected binding target
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(int), _indices, BufferUsageHint.StaticDraw);

            // Load the two different shaders, they use the same vertex shader program. However they have two different fragment shaders.
            // This is because the lamp only uses a basic shader to turn it white, it wouldn't make sense to have the lamp lit in other colors.
            // The lighting shaders uses the lighting.frag shader which is what a large part of this chapter will be about

            {
                // Initialize the vao for the model
                _vaoModel = GL.GenVertexArray();
                GL.BindVertexArray(_vaoModel);

                var vertexLocation = _lightShader.GetAttribLocation("aPosition");
                GL.EnableVertexAttribArray(vertexLocation);
                GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

                var normalLocation = _lightShader.GetAttribLocation("aNormal");
                GL.EnableVertexAttribArray(normalLocation);
                GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, 
                    false, 6 * sizeof(float), 3 * sizeof(float));
            }

            {
                // Initialize the vao for the lamp, this is mostly the same as the code for the model cube
                _vaoLamp = GL.GenVertexArray();
                GL.BindVertexArray(_vaoLamp);

                // Set the vertex attributes (only position data for our lamp)
                var vertexLocation = _shader.GetAttribLocation("aPosition");
                GL.EnableVertexAttribArray(vertexLocation);
                GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            }

            CursorState = CursorState.Grabbed;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _lightShader.Use();
            _lightShader.SetMatrix4("view", _camera.GetViewMatrix());
            _lightShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            LightType temp = _imGuiWindow.GetSelectedLight();
            if (temp != _selectedLight)
            {
                _selectedLight = temp;
                СhangeShader();
            }

            // Draw the model/cube with the lighting shader
            GL.BindVertexArray(_vaoModel);
            _lightShader.SetMatrix4("model", Matrix4.Identity); // Identity matrix since we don't apply any extra transformations
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO); // Bind the EBO

            _lightShader.SetVector3("material.ambient", _imGuiWindow.GetColorCube());
            _lightShader.SetVector3("material.diffuse", _imGuiWindow.GetColorCube());
            _lightShader.SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
            _lightShader.SetFloat("material.shininess", 32.0f);


            _lightShader.SetVector3("light.ambient", _imGuiWindow.GetColorBackLight());
            _lightShader.SetVector3("light.diffuse", _imGuiWindow.GetColorLight());
            _lightShader.SetVector3("light.specular", new Vector3(1.0f, 1.0f, 1.0f));

            Matrix4 cube = Matrix4.Identity;
            _lightShader.SetMatrix4("matrix", cube);

            GL.DrawElements(PrimitiveType.Triangles, _edgeGenerator.GetIndices().Length, DrawElementsType.UnsignedInt, 0);

            for (int i = 0; i < 3; i++)
            {
                cube *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(90.0f));
                _lightShader.SetMatrix4("matrix", cube);
                GL.DrawElements(PrimitiveType.Triangles, _edgeGenerator.GetIndices().Length, DrawElementsType.UnsignedInt, 0);
            }

            cube *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(90.0f));
            _lightShader.SetMatrix4("matrix", cube);
            GL.DrawElements(PrimitiveType.Triangles, _edgeGenerator.GetIndices().Length, DrawElementsType.UnsignedInt, 0);

            cube *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(180.0f));
            _lightShader.SetMatrix4("matrix", cube);
            GL.DrawElements(PrimitiveType.Triangles, _edgeGenerator.GetIndices().Length, DrawElementsType.UnsignedInt, 0);

            if (_selectedLight == LightType.FlashLight)
            {

                _lightShader.SetVector3("light.position", _camera.Position);
                _lightShader.SetVector3("light.direction", _camera.Front);
                _lightShader.SetFloat("light.cutOff", (float)Math.Cos(MathHelper.DegreesToRadians(12.5)));
                _lightShader.SetFloat("light.outerCutOff", MathF.Cos(MathHelper.DegreesToRadians(17.5f)));
            }

            // Draw the lamp
            _lightShader.SetVector3("viewPos", _camera.Position);
            _shader.Use();
            _shader.SetMatrix4("view", _camera.GetViewMatrix());
            _shader.SetMatrix4("projection", _camera.GetProjectionMatrix());
            _shader.SetMatrix4("matrix", Matrix4.Identity);
            Matrix4 lampMatrix = Matrix4.CreateScale(0.2f) * Matrix4.CreateTranslation(_lightPos); // Создаем матрицу преобразования для лампы
            _shader.SetMatrix4("model", lampMatrix); // Устанавливаем матрицу модели лампы

            GL.BindVertexArray(_vaoLamp); // Привязываем массив вершин лампы
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO); // Привязываем буфер индексов (EBO) для лампы
            GL.DrawElements(PrimitiveType.Triangles, _edgeGenerator.GetIndices().Length, DrawElementsType.UnsignedInt, 0);

            SwapBuffers();
        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (!IsFocused) // Check to see if the window is focused
            {
                return;
            }

            var input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                _imGuiWindow.Close();
                Close();
            }

            const float cameraSpeed = 1.5f;
            const float sensitivity = 0.2f;

            if (input.IsKeyDown(Keys.W))
            {
                _camera.Position += _camera.Front * cameraSpeed * (float)e.Time; // Forward
            }

            if (input.IsKeyDown(Keys.S))
            {
                _camera.Position -= _camera.Front * cameraSpeed * (float)e.Time; // Backwards
            }
            if (input.IsKeyDown(Keys.A))
            {
                _camera.Position -= _camera.Right * cameraSpeed * (float)e.Time; // Left
            }
            if (input.IsKeyDown(Keys.D))
            {
                _camera.Position += _camera.Right * cameraSpeed * (float)e.Time; // Right
            }
            if (input.IsKeyDown(Keys.Space))
            {
                _camera.Position += _camera.Up * cameraSpeed * (float)e.Time; // Up
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                _camera.Position -= _camera.Up * cameraSpeed * (float)e.Time; // Down
            }

            // Get the mouse state
            var mouse = MouseState;

            if (_firstMove) // This bool variable is initially set to true.
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                // Calculate the offset of the mouse position
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);

                // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
                _camera.Yaw += deltaX * sensitivity;
                _camera.Pitch -= deltaY * sensitivity; // Reversed since y-coordinates range from bottom to top
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            _camera.Fov -= e.OffsetY;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
            _camera.AspectRatio = Size.X / (float)Size.Y;
        }

        private void СhangeShader()
        {
            if (_selectedLight == LightType.PointLight)
            {
                _lightShader = new Shader(_shadersWay + "pointLight.vert", _shadersWay + "ModelShader.frag");
                //FOR POINT LIGHT
                _lightShader.SetFloat("light.constant", 1.0f);
                _lightShader.SetFloat("light.linear", 0.1f);
                _lightShader.SetFloat("light.quadratic", 0.02f);
                _lightShader.SetVector3("lightPos", _lightPos);
            }
            else if (_selectedLight == LightType.DirectionalLight)
            {
                _lightShader = new Shader(_shadersWay + "directionalLight.vert", _shadersWay + "ModelShader.frag");
                //FOR DIRECTIONAL LIGHT
                _lightShader.SetVector3("light.direction", new Vector3(-0.1f, 0.3f, -1.0f));
            }
            else
            {
                _lightShader = new Shader(_shadersWay + "flashLight.vert", _shadersWay + "ModelShader.frag");
            }
        }
    }
}