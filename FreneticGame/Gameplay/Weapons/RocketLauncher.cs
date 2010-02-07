using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frenetic.Physics;
using Microsoft.Xna.Framework;

namespace Frenetic.Gameplay.Weapons
{
    public class RocketLauncher : IProjectileWeapon
    {
        public const int RocketOffset = 50;
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
            this.Rockets.Add(this.RocketFactory(origin + (RocketLauncher.RocketOffset * Vector2.Normalize(direction - origin)), Vector2.Normalize(direction - origin)));
            Console.WriteLine("Creating rocket at position " + origin.ToString() + " with velocity " + this.Rockets.Last<Rocket>().Velocity.ToString());
        }

        public void RemoveDeadProjectiles()
        {
            foreach (var rocket in this.Rockets)
            {
                if (!rocket.IsAlive)
                {
                    rocket.Destroy();
                }
            }
            this.Rockets.RemoveAll(rocket => !rocket.IsAlive);
        }

        public event Action<IPhysicsComponent> DamagedAPlayer;

        Rocket.Factory RocketFactory;
    }
}
