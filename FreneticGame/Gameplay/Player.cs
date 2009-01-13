using System;
using Microsoft.Xna.Framework;

using Frenetic.Physics;

namespace Frenetic
{
    public class Player
    {
        public delegate Player Factory(int ID);

        public Player(int ID, IIntegrator integrator)
        {
            this.ID = ID;
            _integrator = integrator;
        }
        // For XmlSerializer:
        public Player(){}

        public int ID { get; set; }
        public Vector2 Position { get; set; }

        public void Update()
        {
            Position = _integrator.Integrate(Position);
        }

        IIntegrator _integrator;
    }
}
