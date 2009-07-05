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
        public PlayerView(ITextureBank<PlayerTexture> playerTextureBank, ISpriteBatch spriteBatch, ICamera camera, RailGunView.Factory railGunViewFactory)
        {
            _playerTextureBank = playerTextureBank;
            _spriteBatch = spriteBatch;
            _camera = camera;
            _railGunViewFactory = railGunViewFactory;
        }
        #region IView Members

        public void Generate()
        {
            foreach (KeyValuePair<IPlayer, PlayerDrawInfo> playerInfo in _players)
            {
                IPlayer player = playerInfo.Key;
                IPlayerSettings playerSettings = playerInfo.Value.PlayerSettings;
                IRailGunView weaponView = playerInfo.Value.WeaponView;

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

                weaponView.Draw(player.CurrentWeapon, _camera.TranslationMatrix);
            }
        }

        #endregion

        public List<IPlayer> Players
        {
            get
            {
                return _players.Keys.ToList();
            }
        }

        public void AddPlayer(IPlayer player, IPlayerSettings settings)
        {
            _players.Add(player, new PlayerDrawInfo(settings, _railGunViewFactory()));
        }

        public void RemovePlayer(IPlayer player)
        {
            _players.Remove(player);
        }

        Dictionary<IPlayer, PlayerDrawInfo> _players = new Dictionary<IPlayer,PlayerDrawInfo>();

        ITextureBank<PlayerTexture> _playerTextureBank;
        ISpriteBatch _spriteBatch;
        ICamera _camera;
        RailGunView.Factory _railGunViewFactory;

        class PlayerDrawInfo
        {
            public PlayerDrawInfo(IPlayerSettings settings, IRailGunView weaponView)
            {
                this.PlayerSettings = settings;
                this.WeaponView = weaponView;
            }

            public IPlayerSettings PlayerSettings { get; set; }
            public IRailGunView WeaponView { get; set; }
        }
    }
}
