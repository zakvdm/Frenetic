using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Frenetic
{
    public class PlayerView : IView
    {
        
        public PlayerView(Player player, SpriteBatch spriteBatch, Texture2D texture)
        {
            _player = player;
            _spriteBatch = spriteBatch;
            _texture = texture;

            Random rnd = new Random();
            _color = new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble());
        }
        #region IView Members

        int count = 0;
        public void Generate()
        {
            if (_spriteBatch != null)
            {
                _spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.None);
                _spriteBatch.Draw(_texture, _player.Position, null, _color, 0f,
                    new Vector2(_texture.Width / 2f, _texture.Height / 2f),
                    new Vector2(1, 1),
                    SpriteEffects.None, 0f);
                _spriteBatch.End();
            }
            
            if (count > 100)
            {
                Console.WriteLine("CLIENT: Player " + _player.ID.ToString() + " position is: " + _player.Position.ToString());
                count = 0;
            }
            count++;
        }

        #endregion

        private Player _player;
        private SpriteBatch _spriteBatch;
        private Texture2D _texture;
        private Color _color;
    }
}
