using ClickableTransparentOverlay;
using ImGuiNET;
using System.Numerics;

namespace Overlay
{
    public enum LightType
    {
        FlashLight,
        PointLight,
        DirectionalLight
    }

    public class ImGuiOverlay : ClickableTransparentOverlay.Overlay
    {
        private Vector3 colorCube = new Vector3(1.0f, 0.5f, 0.5f); // Красный цвет по умолчанию
        private Vector3 colorLight = new Vector3(0.0f, 1.0f, 0.0f); // Зеленый цвет по умолчанию
        private Vector3 colorBackLight = new Vector3(0.0f, 0.0f, 1.0f); // Синий цвет по умолчанию

        private LightType selectedLight = LightType.FlashLight;

        private long _fps = 0;

        private int _countOfCubes = 1;
        public LightType GetSelectedLight()
        {
            return selectedLight;
        }

        public void SetFps(long fps)
        {
            _fps = fps;
        }

        // Методы get для каждого вектора
        public OpenTK.Mathematics.Vector3 GetColorCube()
        {
            return new OpenTK.Mathematics.Vector3(colorCube.X, colorCube.Y, colorCube.Z);
        }

        public OpenTK.Mathematics.Vector3 GetColorLight()
        {
            return new OpenTK.Mathematics.Vector3(colorLight.X, colorLight.Y, colorLight.Z);
        }

        public OpenTK.Mathematics.Vector3 GetColorBackLight()
        {
            return new OpenTK.Mathematics.Vector3(colorBackLight.X, colorBackLight.Y, colorBackLight.Z);
        }

        public int GetCount()
        {
            return _countOfCubes;
        }

        protected override void Render()
        {
            ImGui.Begin("Color Settings");

            // Палитра для выбора цвета для вектора colorCube
            ImGui.ColorEdit3("ColorCube", ref colorCube);

            // Палитра для выбора цвета для вектора colorLight
            ImGui.ColorEdit3("ColorLight", ref colorLight);

            // Палитра для выбора цвета для вектора colorBackLight
            ImGui.ColorEdit3("ColorBackLight", ref colorBackLight);

            ImGui.Text("Light Type:");
            if (ImGui.RadioButton("Flash Light", selectedLight == LightType.FlashLight))
            {
                selectedLight = LightType.FlashLight;
            }
            ImGui.SameLine();
            if (ImGui.RadioButton("Point Light", selectedLight == LightType.PointLight))
            {
                selectedLight = LightType.PointLight;
            }
            ImGui.SameLine();
            if (ImGui.RadioButton("Directional Light", selectedLight == LightType.DirectionalLight))
            {
                selectedLight = LightType.DirectionalLight;
            }

            ImGui.Text("Count of Cubes: ");
            ImGui.SameLine();
            ImGui.InputInt("##CountOfCubesInput", ref _countOfCubes);
            _countOfCubes = Math.Max(1, _countOfCubes);

            ImGui.Text("FPS: ");
            ImGui.SameLine();
            ImGui.Text(_fps.ToString());

            ImGui.End();
        }
    }
}


