using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Frenetic.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.Linq;
using Frenetic.Weapons;

namespace Frenetic.Player
{
    public class PlayerView : IView
    {
        public PlayerView(List<IPlayer> playerList, ITextureBank<PlayerTexture> playerTextureBank, ISpriteBatch spriteBatch, ICamera camera, IRailGunView railGunView)
        {
            _players = playerList;
            _playerTextureBank = playerTextureBank;
            _spriteBatch = spriteBatch;
            _camera = camera;
            _railGunView = railGunView;
        }
        #region IView Members

        public void Generate()
        {
            foreach (IPlayer player in _players)
            {
                IPlayerSettings playerSettings = player.PlayerSettings;

                ITexture texture = _playerTextureBank[playerSettings.Texture];

                if (_spriteBatch != null)
                {
                    if (player.IsAlive)
                    {
                        _spriteBatch.Begin(_camera.TranslationMatrix);
                        _spriteBatch.Draw(texture, player.Position, null, playerSettings.Color, 0f,
                            new Vector2(texture.Width / 2f, texture.Height / 2f),
                            new Vector2(1, 1),
                            SpriteEffects.None, 1f);
                        _spriteBatch.End();
                    }
                }
            }

            _railGunView.Draw(_camera.TranslationMatrix);
        }

        #endregion

        List<IPlayer> _players;

        ITextureBank<PlayerTexture> _playerTextureBank;
        ISpriteBatch _spriteBatch;
        ICamera _camera;
        IRailGunView _railGunView;
    }
}
