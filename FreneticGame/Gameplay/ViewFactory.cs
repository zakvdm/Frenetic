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
            _graphicsDevice = graphicsDevice;
            _contentManager = contentManager;

            _spriteBatch = new SpriteBatch(_graphicsDevice);
            _playerTexture = _contentManager.Load<Texture2D>(playerTexturePath);
        }

        #region IViewFactory Members
        
        public PlayerView MakePlayerView(Player player)
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
