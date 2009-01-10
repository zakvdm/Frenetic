using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Frenetic
{
    public class Line
    {
        private static Texture2D texture;

        private Vector2 origin;
        private Vector2 end;
        private Color color = Color.White;

        public Vector2 Origin
        {
            get { return origin; }
            set { origin = value; }
        }
        public Vector2 End
        {
            get { return end; }
            set { end = value; }
        }
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        public Line(Vector2 origin, Vector2 end)
        {
            this.origin = origin;
            this.end = end;
        }
        public Line()
        {
            origin = new Vector2();
            end = new Vector2();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 TextureOrigin = new Vector2(0.5f, 0.0f);
            Vector2 difference = end - origin;
            Vector2 scale = new Vector2(1.0f, difference.Length() / texture.Height);

            float angle = (float)(Math.Atan2(difference.Y, difference.X)) - MathHelper.PiOver2;

            spriteBatch.Draw(texture, origin, null, color, angle, TextureOrigin, scale, SpriteEffects.None, 1.0f);
        }

        /// <summary>
        /// Load all of the static graphics content for this class.
        /// </summary>
        /// <param name="contentManager">The content manager to load with.</param>
        public static void LoadContent(ContentManager contentManager)
        {
            // safety-check the parameters
            if (contentManager == null)
            {
                throw new ArgumentNullException("contentManager");
            }

            // load the texture
            texture = contentManager.Load<Texture2D>("Textures/blank");
        }

        /// <summary>
        /// Unload all of the static graphics content for this class.
        /// </summary>
        public static void UnloadContent()
        {
            texture = null;
        }
    }
}
