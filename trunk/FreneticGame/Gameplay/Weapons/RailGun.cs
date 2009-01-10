using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frenetic
{
    public class RailGun : Weapon
    {
        SimpleRay ray;
        Line line;

        public RailGun()
            : base()
        {
            ray = new SimpleRay();
            line = new Line();

            line.Color = Color.Red;
        }

        public override bool Fire(Vector2 position, Vector2 mousePosition, PhysicsManager physicsManager)
        {
            if (base.Fire(position, mousePosition, physicsManager))
            {
                ray.Origin = position;
                ray.Direction = mousePosition - position;

                List<CollisionResult> collisions = physicsManager.ShootRay(ray);

                foreach (CollisionResult collision in collisions)
                {
                    collision.GameplayObject.Damage(this, damageAmount);
                }

                line.Origin = ray.Origin;
                line.End = ray.End;

                return true;
            }

            return false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                line.Draw(spriteBatch);
            }
        }

    }
}
