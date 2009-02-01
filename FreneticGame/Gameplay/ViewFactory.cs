using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Frenetic.Graphics;

namespace Frenetic
{
    public class ViewFactory : IViewFactory
    {
        const string playerTexturePath = "Content/Textures/ball";

        public ViewFactory(SpriteBatch spriteBatch, Texture2D texture)
        {
            if (spriteBatch != null)
            {
                _spriteBatch = spriteBatch;
            }
            if (texture != null)
            {
                _playerTexture = texture;
            }
            /*
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
            */
        }

        #region IViewFactory Members
        
        public PlayerView MakePlayerView(IPlayer player, ICamera camera)
        {
            return new PlayerView(player, new XNASpriteBatch(_spriteBatch), new XNATexture(_playerTexture), camera);
        }

        #endregion

        SpriteBatch _spriteBatch;
        Texture2D _playerTexture;
        //GraphicsDevice _graphicsDevice;
        //ContentManager _contentManager;
    }
}
