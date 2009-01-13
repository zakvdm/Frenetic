using System;
using Microsoft.Xna.Framework;

namespace Frenetic.Physics
{
    public class VerletIntegrator : IIntegrator
    {
        public VerletIntegrator(PhysicsValues physicsValues)
        {
            _values = physicsValues;
        }

        public Vector2 LastPosition { get; set; }

        #region IIntegrator Members

        public Vector2 Integrate(Vector2 position)
        {
            Vector2 oldPosition = LastPosition;
            //Vector2 position = position;

            float oldX = oldPosition.X;
            float oldY = oldPosition.Y;

            oldPosition.X = position.X;
            oldPosition.Y = position.Y;

            position.X += (_values.Drag * position.X) - (_values.Drag * oldX);
            position.Y += (_values.Drag * position.Y) - (_values.Drag * oldY) + _values.Gravity;

            //_player.Position = position;
            LastPosition = oldPosition;

            return position;
        }

        #endregion

        PhysicsValues _values;
    }

    public class PhysicsValues
    {
        // TODO: Add value range checking to properties

        public PhysicsValues()
        {
            Gravity = 0.2f;//.3 is a bit much, .1 is a bit "on the moon"..
            Drag = 0.999999f;//0 means full drag, 1 is no drag
            Bounce = 0.3f;//must be in [0,1], where 1 means full bounce. but 1 seems to incite "the flubber effect" so use 0.9 as a practical upper bound
            Friction = 0.05f;
        }
        public float Gravity { get; set; }
        public float Drag { get; set; }
        public float Bounce { get; set; }
        public float Friction { get; set; }
    }
}
