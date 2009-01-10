#region File Description
//-----------------------------------------------------------------------------
// GameplayObject.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Frenetic
{
    /// <summary>
    /// A base public class for all gameplay objects.
    /// </summary>
    abstract public class GameplayObject
    {
        #region Status Data
        


        /*
        /// <summary>
        /// If true, the object is active in the world.
        /// </summary>
        protected bool active = false;
        public bool Active
        {
            get { return active; }
        }
        */

        #endregion


        #region Physical Data


        protected Vector2 position = Vector2.Zero;
        public virtual Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;
            }
        }

        protected int width = 0;
        public int Width
        {
            get { return width; }
            set { width = value; }
        }
        protected int height = 0;
        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public int HalfWidth
        {
            get { return (int)(width / 2); }
        }
        public int HalfHeight
        {
            get { return (int)(height / 2); }
        }
        


        /*
        protected Vector2 velocity = Vector2.Zero;
        public Vector2 Velocity
        {
            get { return velocity; }
            set
            {
                if ((value.X == Single.NaN) || (value.Y == Single.NaN))
                {
                    throw new ArgumentException("Velocity was NaN");
                }
                velocity = value;
            }
        }
        */

        protected float rotation = 0f;
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        #endregion


        #region Collision Data

        /*
        protected float radius = 1f;
        public float Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        protected float mass = 1f;
        public float Mass
        {
            get { return mass; }
        }

        protected bool collidedThisFrame = false;
        public bool CollidedThisFrame
        {
            get { return collidedThisFrame; }
            set { collidedThisFrame = value; }
        }
        */

        #endregion


        #region Initialization Methods


        /// <summary>
        /// Constructs a new gameplay object.
        /// </summary>
        protected GameplayObject() { }

        /*
        /// <summary>
        /// Initialize the object to it's default gameplay states.
        /// </summary>
        public virtual void Initialize()
        {
            if (!active)
            {
                active = true;
                CollisionManager.Collection.Add(this);
            }
        }
        */

        #endregion


        #region Updating Methods

        /*
        /// <summary>
        /// Update the gameplay object.
        /// </summary>
        /// <param name="elapsedTime">The amount of elapsed time, in seconds.</param>
        public virtual void Update(float elapsedTime)
        {
            collidedThisFrame = false;
        }
        */

        #endregion


        #region Drawing Methods


        /// <summary>
        /// Draw the gameplay object.
        /// </summary>
        /// <param name="elapsedTime">The amount of elapsed time, in seconds.</param>
        /// <param name="spriteBatch">The SpriteBatch object used to draw.</param>
        /// <param name="sprite">The texture used to draw this object.</param>
        /// <param name="sourceRectangle">The source rectangle.</param>
        /// <param name="color">The color of the sprite.</param>
        public virtual void Draw(SpriteBatch spriteBatch,
            Texture2D sprite, Rectangle? sourceRectangle, Color color)
        {
            if ((spriteBatch != null) && (sprite != null))
            {
                /*
                spriteBatch.Draw(sprite, position, sourceRectangle, color, rotation,
                    new Vector2(sprite.Width / 2f, sprite.Height / 2f),
                    2f * 10 / MathHelper.Min(sprite.Width, sprite.Height),
                    SpriteEffects.None, 0f);
                */
                spriteBatch.Draw(sprite, position, sourceRectangle, color, rotation,
                    new Vector2(sprite.Width / 2f, sprite.Height / 2f),
                    new Vector2((float)width / (float)sprite.Width, (float)height / (float)sprite.Height),
                    SpriteEffects.None, 0f);
                 
            }
        }


        #endregion


        #region Interaction Methods


        /// <summary>
        /// Defines the interaction between this GameplayObject and 
        /// a target GameplayObject when they touch.
        /// </summary>
        /// <param name="target">The GameplayObject that is touching this one.</param>
        /// <returns>True if the objects meaningfully interacted.</returns>
        public virtual bool Touch(GameplayObject target)
        {
            return true;
        }


        /// <summary>
        /// Damage this object by the amount provided.
        /// </summary>
        /// <remarks>
        /// This function is provided in lieu of a Life mutation property to allow 
        /// classes of objects to restrict which kinds of objects may damage them,
        /// and under what circumstances they may be damaged.
        /// </remarks>
        /// <param name="source">The GameplayObject responsible for the damage.</param>
        /// <param name="damageAmount">The amount of damage.</param>
        /// <returns>If true, this object was damaged.</returns>
        public virtual bool Damage(GameplayObject source, float damageAmount)
        {
            return false;
        }


        /// <summary>
        /// Kills this object, in response to the given GameplayObject.
        /// </summary>
        /// <param name="source">The GameplayObject responsible for the kill.</param>
        /// <param name="cleanupOnly">
        /// If true, the object dies without any further effects.
        /// </param>
        public virtual void Die(GameplayObject source, bool cleanupOnly)
        {
            /*
            // deactivate the object
            if (active)
            {
                active = false;
                CollisionManager.Collection.QueuePendingRemoval(this);
            }
            */
        }


        #endregion
    }
}
