using System.ComponentModel;
using SharpDX;
using SharpDX.Windows;
using ESCDX;
using FormTemplate;
namespace Geometry
{
    public partial class Form1 : DXForm
    {
        private string _shaderWay = "..//..//..//Shader.fx";

        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _engine = new Engine(this.Handle, _shaderWay, this.ClientSize.Width, this.ClientSize.Height, 4);

            _scene = new GridScene(_engine, _camera);

            this.Cursor = new Cursor(Cursor.Current.Handle);
            Cursor.Hide();
            this.Focus();
            Cursor.Clip = this.RectangleToScreen(this.ClientRectangle);

            RenderLoop.Run(this, () =>
            {
                _scene.Draw();
            });
        }
    }
}

