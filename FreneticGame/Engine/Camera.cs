using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frenetic
{
    public class Camera
    {
        private Vector2 cameraPosition;
        private Vector2 cameraOffset;
        private Matrix transformMatrix;

        public Vector2 Position
        {
            get { return cameraPosition; }
            set 
            {
                cameraPosition = value;
                transformMatrix = Matrix.CreateTranslation(-cameraPosition.X + cameraOffset.X, -cameraPosition.Y + cameraOffset.Y, 0.0f);
            }
        }
        public Matrix TransformMatrix
        {
            get { return transformMatrix; }
        }

        public Camera(Vector2 offset)
        {
            cameraOffset = offset;
            cameraPosition = new Vector2();
        }
        public Camera(float x, float y)
        {
            cameraOffset = new Vector2(x, y);
            cameraPosition = new Vector2();
        }

        /*
        public Vector2 TransformPosition(Vector2 position)
        {
            return ((position - cameraPosition) + cameraOffset);
        }
        

        public Vector2 TransformPosition(float x, float y)
        {
            return TransformPosition(new Vector2(x, y));
        }
        */
    }
}