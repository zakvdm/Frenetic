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
            // SMOOTHING ONLY:
            var displacement = newestPosition - this.Position;

            if (displacement.Length() > 100f)
            {
                Console.WriteLine("Position too far out of sync, snapping...");
                // SNAP!
                this.Position = newestPosition;
                this.PhysicsComponent.LinearVelocity = Vector2.Zero;
                return;
            }

            var velocity = displacement / deliveryTime;
            this.PhysicsComponent.LinearVelocity = velocity;
        }

        // NOTE: THIS ISN'T WORKING WELL YET, SO FOR NOW I'M USING ONLY SMOOTHING
        public void UpdatePositionFromNetworkWithPrediction(Vector2 newestPosition, float deliveryTime)
        {
            // NOTE: We use the last delivery time to estimate how long before the next update... crude but meh.
            var adjustment_from_predicted_to_actual = newestPosition - this.Position;

            if (adjustment_from_predicted_to_actual.Length() > 100f)
            {
                Console.WriteLine("Prediction too far out, resetting...");
                this.PhysicsComponent.LinearVelocity = (newestPosition - this.LastReceivedPosition) / deliveryTime;
                this.Position = newestPosition;
                this.LastReceivedPosition = newestPosition;
                return;
            }
            var predicted_velocity = newestPosition - this.LastReceivedPosition;

            var final_velocity = (predicted_velocity + adjustment_from_predicted_to_actual) / deliveryTime;
            
            this.PhysicsComponent.LinearVelocity = final_velocity;

            this.LastReceivedPosition = newestPosition;
        }

        internal Vector2 LastReceivedPosition { get; set; }
    }
}
