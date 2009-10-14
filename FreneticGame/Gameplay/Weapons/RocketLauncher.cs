using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frenetic.Physics;
using Microsoft.Xna.Framework;

namespace Frenetic.Weapons
{
    public class RocketLauncher : IWeapon
    {
        public RocketLauncher(Rocket.Factory rocketFactory)
        {
            this.RocketFactory = rocketFactory;

            this.Shots = new Shots();
            this.Rockets = new List<Rocket>();
        }

        public List<Rocket> Rockets { get; private set; }

        public int Damage { get; set; }
        public Shots Shots { get; private set; }

        public void Shoot(Vector2 origin, Vector2 direction)
        {
            // Record the Shot:
            this.Shots.Add(new Shot(origin, direction));

            // Create a rocket:
            this.Rockets.Add(this.RocketFactory(origin + (50f * Vector2.Normalize(direction - origin)), Vector2.Normalize(direction - origin)));
            Console.WriteLine("Creating rocket at position " + origin.ToString() + " with velocity " + this.Rockets.Last<Rocket>().Velocity.ToString());
        }

        public event Action<IPhysicsComponent> DamagedAPlayer;

        Rocket.Factory RocketFactory;
    }
}
