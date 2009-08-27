using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Frenetic.Physics;
using Frenetic.Weapons;
using Frenetic.Engine;
using Frenetic.Gameplay;

namespace Frenetic.Player
{
    public class NetworkPlayer : BasePlayer
    {
        public NetworkPlayer(IPlayerSettings playerSettings, IPhysicsComponent physicsComponent, IRailGun weapon, ITimer timer)
            : base(playerSettings, physicsComponent, null, weapon, timer)
        {
            LastReceivedPosition = this.Position;
        }

        public override void Update()
        {
            // We're not in control of the NetworkPlayer's Position, so nothing to update...
        }

        public override void UpdatePositionFromNetwork(Vector2 newestPosition, float deliveryTime)
        {
            // NOTE: We use the last delivery time to estimate how long before the next update... crude but meh.
            var adjustment = newestPosition - this.Position;
            var velocity = newestPosition - this.LastReceivedPosition;

            var final_velocity = (velocity + adjustment) / deliveryTime;
            
            this.PhysicsComponent.LinearVelocity = final_velocity;

            this.LastReceivedPosition = newestPosition;

            if (this.PhysicsComponent.LinearVelocity.Length() > 100f)
            {
                Console.WriteLine("Velocity is " + this.PhysicsComponent.LinearVelocity.Length() + "!");
            }
        }

        internal Vector2 LastReceivedPosition { get; set; }
    }
}
