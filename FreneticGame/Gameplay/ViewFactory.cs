using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Frenetic
{
    public class ViewFactory : IViewFactory
    {
        const string playerTexturePath = "Content/Textures/ball";

        public ViewFactory(GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            if (graphicsDevice != null)
            {
                _graphicsDevice = graphicsDevice;
                _spriteBatch = new SpriteBatch(_graphicsDevice);
            }

            if (contentManager != null)
            {
                _contentManager = contentManager;
                _playerTexture = _contentManager.Load<Texture2D>(playerTexturePath);
            }
        }

        #region IViewFactory Members
        
        public PlayerView MakePlayerView(IPlayer player)
        {
            return new PlayerView(player, _spriteBatch, _playerTexture);
        }

        #endregion

        SpriteBatch _spriteBatch;
        Texture2D _playerTexture;
        GraphicsDevice _graphicsDevice;
        ContentManager _contentManager;
    }
}
