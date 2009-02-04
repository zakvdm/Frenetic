using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Frenetic
{
    public class Crosshair
    {
        private const int CROSSHAIR_DEFAULT_SIZE = 16;

        public Crosshair(ICamera camera)
        {
            _camera = camera;
            Size = CROSSHAIR_DEFAULT_SIZE;
        }

        public Vector2 ViewPosition 
        {
            get
            {
                return new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            }
        }
        public Vector2 WorldPosition
        {
            get
            {
                return _camera.ConvertToWorldCoordinates(ViewPosition);
            }
        }

        public int Size { get; set; }

        ICamera _camera;
    }
}
