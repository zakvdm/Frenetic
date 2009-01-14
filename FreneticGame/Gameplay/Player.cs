using System;
using Microsoft.Xna.Framework;

using Frenetic.Physics;

namespace Frenetic
{
    public class Player : IPlayer
    {
        public delegate IPlayer Factory(int ID);

        public Player(int ID, IIntegrator integrator)
        {
            this.ID = ID;
            _integrator = integrator;
        }
        public Player() { } // For XmlSerializer

        public int ID { get; set; }
        public Vector2 Position { get; set; }

        public void Update()
        {
            Position = _integrator.Integrate(Position);
        }

        IIntegrator _integrator;
    }
}
