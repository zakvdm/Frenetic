using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Frenetic.Physics;

namespace Frenetic.Level
{
    public class LevelPiece
    {
        public delegate LevelPiece Factory(Vector2 position, Vector2 size);

        public LevelPiece(IPhysicsComponent physicsComponent)
        {
            _physicsComponent = physicsComponent;
            _physicsComponent.IsStatic = true;

            Random rnd = new Random((int)(physicsComponent.Size.X + physicsComponent.Size.Y));
            Color = new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble());
        }

        public Vector2 Position 
        {
            get
            {
                return _physicsComponent.Position;
            }
        }
        public Vector2 Size
        {
            get
            {
                return _physicsComponent.Size;
            }
        }

        public float TopEdge
        {
            get
            {
                return Position.Y - (Size.Y / 2);
            }
        }
        public float BottomEdge
        {
            get
            {
                return Position.Y + (Size.Y / 2);
            }
        }
        public float LeftEdge
        {
            get
            {
                return Position.X - (Size.X / 2);
            }
        }
        public float RightEdge
        {
            get
            {
                return Position.X + (Size.X / 2);
            }
        }

        public Color Color { get; private set; }    // TODO: This should rather be a property on the View...

        IPhysicsComponent _physicsComponent;
    }
}
