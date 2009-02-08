using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Frenetic.Graphics;

namespace Frenetic
{
    public class ViewFactory : IViewFactory
    {
        public ViewFactory(ISpriteBatch spriteBatch, ITexture texture)
        {
            _spriteBatch = spriteBatch;
            _playerTexture = texture;
        }

        #region IViewFactory Members
        
        public PlayerView MakePlayerView(IPlayer player, ICamera camera)
        {
            return new PlayerView(player, _spriteBatch, _playerTexture, camera);
        }

        #endregion

        ISpriteBatch _spriteBatch;
        ITexture _playerTexture;
    }
}
