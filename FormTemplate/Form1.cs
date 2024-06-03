using System.ComponentModel;
using ESCDX;
using SharpDX;
namespace FormTemplate
{
    public partial class DXForm : Form
    {

        protected Engine _engine;

        protected Scene _scene;

        protected Camera _camera;

        protected float _cameraSpeed = 0.1f;
        protected float _sensitivity = 0.1f;

        protected float _prevMouseX = 0;
        protected float _prevMouseY = 0;

        public DXForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _camera = new Camera(Vector3.UnitZ * 3, ClientSize.Width / (float)ClientSize.Height);


        }


        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            if (e.KeyChar == 'w' || e.KeyChar == 'ц' || e.KeyChar == 'W' || e.KeyChar == 'Ц')
            {
                _camera.Position += _camera.Front * _cameraSpeed;
                _scene.UpdateBuffers();
            }

            if (e.KeyChar == 'a' || e.KeyChar == 'ф' || e.KeyChar == 'A' || e.KeyChar == 'Ф')
            {
                _camera.Position += _camera.Right * _cameraSpeed;
                _scene.UpdateBuffers();
            }

            if (e.KeyChar == 's' || e.KeyChar == 'ы' || e.KeyChar == 'S' || e.KeyChar == 'Ы')
            {
                _camera.Position -= _camera.Front * _cameraSpeed;
                _scene.UpdateBuffers();
            }

            if (e.KeyChar == 'd' || e.KeyChar == 'в' || e.KeyChar == 'D' || e.KeyChar == 'В')
            {
                _camera.Position -= _camera.Right * _cameraSpeed;
                _scene.UpdateBuffers();
            }

            if (e.KeyChar == (char)Keys.Space)
            {
                _camera.Position -= -_camera.Up * _cameraSpeed;
                _scene.UpdateBuffers();
            }

            if (e.KeyChar == 'z' || e.KeyChar == 'Z' || e.KeyChar == 'Я' || e.KeyChar == 'я')
            {
                _camera.Position -= _camera.Up * _cameraSpeed;
                _scene.UpdateBuffers();
            }

            if (e.KeyChar == (char)Keys.Escape)
            {
                this.Close();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            float deltaX = e.X - _prevMouseX;
            float deltaY = e.Y - _prevMouseY;


            _camera.Yaw -= deltaX * _sensitivity;
            _camera.Pitch -= deltaY * _sensitivity;



            _prevMouseX = e.X;
            _prevMouseY = e.Y;

            _scene.UpdateBuffers();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            Cursor.Position = PointToScreen(new System.Drawing.Point(ClientSize.Width / 2, ClientSize.Height / 2));
            base.OnMouseLeave(e);
            _prevMouseX = ClientSize.Width / 2;
            _prevMouseY = ClientSize.Height / 2;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            Cursor.Position = PointToScreen(new System.Drawing.Point(ClientSize.Width / 2, ClientSize.Height / 2));
            base.OnMouseEnter(e);
            _prevMouseX = ClientSize.Width / 2;
            _prevMouseY = ClientSize.Height / 2;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            // Release all resources
            _engine.Dispose();
            Cursor.Show();
            base.OnClosing(e);
        }
    }
}

