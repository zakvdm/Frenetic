using System;

namespace Frenetic.Physics
{
    public class DummyPhysicsComponent : IPhysicsComponent
    {
        public DummyPhysicsComponent()
        {
            Position = Microsoft.Xna.Framework.Vector2.Zero;
        }
        #region IPhysicsComponent Members

        public Microsoft.Xna.Framework.Vector2 Position { get; set; }

        #endregion
    }
}