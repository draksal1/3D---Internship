using System.ComponentModel;
using SharpDX;
using SharpDX.Windows;
using ESCDX;
using FormTemplate;

namespace GridVisualization
{
    public partial class Form1 : DXForm
    {

        private string _shaderWay = "..//..//..//..//Geometry/Shader.fx";

        private bool _isDragging = false;

        private string _closedMenuText = "Open menu";

        private string _openedMenuText = "Close menu";

        List<TrackBar> _trackBars;
        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _engine = new Engine(this.Handle, _shaderWay, this.ClientSize.Width, this.ClientSize.Height, 4);

            _scene = new GridScene(_engine, _camera);
            string[] properties = ((GridScene)_scene).GetProperties();
            comboBox1.DataSource = properties;

            checkedListBox1.Items.Add("X slice", false);
            checkedListBox1.Items.Add("Y slice", false);
            checkedListBox1.Items.Add("Z slice", false);

            _trackBars = new List<TrackBar>
            {
                trackBar1,
                trackBar2,
                trackBar3
            };

            Vector3 sizes = ((GridScene)_scene).GetSizes();

            trackBar1.Maximum = (int)sizes.X - 1;
            trackBar2.Maximum = (int)sizes.Y - 1;
            trackBar3.Maximum = (int)sizes.Z - 1;

            ((GridScene)_scene).SetMeshState(checkBox1.Checked);

            RenderLoop.Run(this, () =>
            {
                _scene.Draw();
            });
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ((GridScene)_scene).ChangeProperty(comboBox1.Text);

            if (radioButton2.Checked)
            {
                RequestSlices();
            }
        }


        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                _isDragging = true;

                _prevMouseX = e.X;
                _prevMouseY = e.Y;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                _isDragging = false;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_isDragging)
            {
                float deltaX = -(e.X - _prevMouseX) * _sensitivity;
                float deltaY = (e.Y - _prevMouseY) * _sensitivity;

                _scene.Rotate(Matrix.RotationX(MathUtil.DegreesToRadians(deltaY)) * Matrix.RotationY(MathUtil.DegreesToRadians(deltaX)));

                _scene.UpdateBuffers();

                _prevMouseX = e.X;
                _prevMouseY = e.Y;
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            _camera.Fov -= e.Delta * _sensitivity;

            _scene.UpdateBuffers();
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if (e.KeyChar == 'i' || e.KeyChar == 'I' || e.KeyChar == 'ш' || e.KeyChar == 'Ш')
            {
                _scene.Rotate(Matrix.RotationY(-MathUtil.DegreesToRadians(55.0f)), true);
                _scene.UpdateBuffers();
            }

            if (e.KeyChar == 'o' || e.KeyChar == 'O' || e.KeyChar == 'щ' || e.KeyChar == 'Щ')
            {
                _scene.Rotate(Matrix.RotationY(-MathUtil.DegreesToRadians(55.0f)) * Matrix.RotationX(-MathUtil.DegreesToRadians(90.0f)), true);
                _scene.UpdateBuffers();
            }

            if (e.KeyChar == 'p' || e.KeyChar == 'P' || e.KeyChar == 'з' || e.KeyChar == 'З')
            {
                _scene.Rotate(Matrix.RotationY(-MathUtil.DegreesToRadians(-35.0f)), true);
                _scene.UpdateBuffers();
            }
        }
        protected override void OnMouseLeave(EventArgs e) { }

        protected override void OnMouseEnter(EventArgs e) { }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            bool val = radioButton2.Checked;

            checkedListBox1.Visible = val;

            trackBar1.Visible = val;
            trackBar2.Visible = val;
            trackBar3.Visible = val;

            label3.Visible = val;
            label4.Visible = val;
            label5.Visible = val;
            label6.Visible = val;
            label7.Visible = val;
            label8.Visible = val;

            RequestSlices();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel1.Visible = !panel1.Visible;

            if (button1.Text == _closedMenuText)
            {
                button1.Text = _openedMenuText;
            }
            else
            {
                button1.Text = _closedMenuText;
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label6.Text = trackBar1.Value.ToString();
            RequestSlices();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            label7.Text = trackBar2.Value.ToString();
            RequestSlices();
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            label8.Text = trackBar3.Value.ToString();
            RequestSlices();
        }

        private void RequestSlices()
        {
            int x = trackBar1.Enabled ? trackBar1.Value : -1;

            int y = trackBar2.Enabled ? trackBar2.Value : -1;

            int z = trackBar3.Enabled ? trackBar3.Value : -1;

            ((GridScene)_scene).LoadSlices(x, y, z);
        }


        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            ((GridScene)_scene).LoadGrid();
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            _trackBars[e.Index].Enabled = e.NewValue == CheckState.Checked;
            RequestSlices();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            ((GridScene)_scene).SetMeshState(checkBox1.Checked);
        }
    }
}

