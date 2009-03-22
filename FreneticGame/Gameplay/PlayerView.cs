using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Frenetic.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Frenetic
{
    public class PlayerView : IView
    {
        public delegate PlayerView Factory(IPlayer player);
        
        public PlayerView(IPlayer player, PlayerSettings playerSettings, ITextureBank<PlayerTextures> playerTextureBank, ISpriteBatch spriteBatch, ICamera camera)
        {
            _player = player;
            _playerSettings = playerSettings;
            _playerTextureBank = playerTextureBank;
            _spriteBatch = spriteBatch;
            _camera = camera;
        }
        #region IView Members

        int count = 0;
        public void Generate()
        {
            ITexture texture = _playerTextureBank[_playerSettings.Texture];
            if (_spriteBatch != null)
            {
                _spriteBatch.Begin(_camera.TranslationMatrix);
                _spriteBatch.Draw(texture, _player.Position, null, _playerSettings.Color, 0f,
                    new Vector2(texture.Width / 2f, texture.Height / 2f),
                    new Vector2(1, 1),
                    SpriteEffects.None, 1f);
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

        private IPlayer _player;
        private PlayerSettings _playerSettings;
        private ITextureBank<PlayerTextures> _playerTextureBank;
        private ISpriteBatch _spriteBatch;
        private ICamera _camera;

    }
}
