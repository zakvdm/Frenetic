using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frenetic
{
    public abstract class Weapon : GameplayObject
    {
        /// <summary>
        /// The amount of time remaining before this weapon can fire again.
        /// </summary>
        private float timeToNextFire = 0f;

        /// <summary>
        /// The minimum amount of time between each firing of this weapon.
        /// </summary>
        private float fireDelay = 10.0f;

        /// <summary>
        /// The amount of time that the weapon will still be drawn.
        /// </summary>
        private float timeStillVisible = 0f;
        /// <summary>
        /// The total time that the weapon is drawn for.
        /// </summary>
        private float visibleTime = 1.0f;

        public bool Visible
        {
            get { return (timeStillVisible > 0f); }
        }

        protected float damageAmount = 0f;
        public float DamageAmount
        {
            get { return damageAmount; }
            set { damageAmount = value; }
        }

        public Weapon()
        {}

        /// <summary>
        /// Update the weapon.
        /// </summary>
        /// <param name="elapsedTime">The amount of elapsed time, in seconds.</param>
        public virtual void Update()
        {
            // count down to when the weapon can fire again
            if (timeToNextFire > 0f)
            {
                timeToNextFire = MathHelper.Max(timeToNextFire - 0.1f, 0f);
            }

            if (timeStillVisible > 0f)
            {
                timeStillVisible = MathHelper.Max(timeStillVisible - 0.1f, 0f);
            }
        }

        public virtual bool Fire(Vector2 position, Vector2 mousePosition, PhysicsManager physicsManager)
        {
            // if we can't fire yet, then we're done
            if (timeToNextFire > 0f)
            {
                return false;
            }

            // set the timers
            timeToNextFire = fireDelay;
            timeStillVisible = visibleTime;

            return true;
        }

        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
