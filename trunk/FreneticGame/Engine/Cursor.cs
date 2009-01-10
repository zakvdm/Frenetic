using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frenetic
{
    public class Cursor
    {
        private Vector2 position;
        private Texture2D texture;

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Cursor(Texture2D texture)
        {
            position = new Vector2();
            this.texture = texture;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle cursorRectangle = new Rectangle((int)(position.X - (texture.Width / 2)), (int)(position.Y - (texture.Height / 2)), texture.Width, texture.Height);
            spriteBatch.Draw(texture, cursorRectangle, Color.White);
        }
    }
}
