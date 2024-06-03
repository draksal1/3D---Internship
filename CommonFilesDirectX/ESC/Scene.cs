using System.Drawing.Drawing2D;
using System.Xml.Serialization;

namespace ESCDX
{
    public abstract class Scene
    {

        protected Engine _engine;

        protected Camera _camera;


        protected abstract void OnLoad();

        public abstract void Draw();

        public abstract void UpdateBuffers();

        public abstract void Rotate(SharpDX.Matrix rotation, bool setRotation = false);
    }
}
