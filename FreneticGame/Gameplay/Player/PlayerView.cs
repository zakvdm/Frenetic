using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Frenetic.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.Linq;
using Frenetic.Gameplay.Weapons;

namespace Frenetic.Player
{
    public class PlayerView : IView
    {
        public PlayerView(IPlayerList playerList, ITextureBank<PlayerTexture> playerTextureBank, ISpriteBatch spriteBatch, ICamera camera, IWeaponView weaponView, IBubbleTextDrawer bubbleText)
        {
            this.PlayerList = playerList;
            this.PlayerTextureBank = playerTextureBank;
            this.SpriteBatch = spriteBatch;
            this.Camera = camera;
            this.RailGunView = weaponView;
            this.BubbleText = bubbleText;

            this.PlayerList.PlayerAdded += RegisterNewPlayer;
        }
        #region IView Members

        public void Generate(float elapsedSeconds)
        {
            this.SpriteBatch.Begin(this.Camera.TranslationMatrix);
            foreach (IPlayer player in this.PlayerList.Players)
            {
                IPlayerSettings playerSettings = player.PlayerSettings;

                ITexture texture = this.PlayerTextureBank[playerSettings.Texture];

                if (this.SpriteBatch != null)
                {
                    if (player.Status == PlayerStatus.Alive)
                    {
                        this.SpriteBatch.Draw(texture, player.Position, null, playerSettings.Color, 0f,
                            new Vector2(texture.Width / 2f, texture.Height / 2f),
                            new Vector2(1, 1),
                            SpriteEffects.None, 1f);
                    }
                }
            }
            this.BubbleText.DrawText(this.SpriteBatch, elapsedSeconds);
            this.SpriteBatch.End();

            this.RailGunView.Draw(this.Camera.TranslationMatrix);
        }

        #endregion

        private void RegisterNewPlayer(IPlayer newPlayer)
        {
            newPlayer.HealthChanged += HandleHealthChanged;
        }
        private void HandleHealthChanged(IPlayer player, int changedAmount)
        {
            if (changedAmount > 0)
            {
                this.BubbleText.AddText(changedAmount.ToString(), player.Position, Color.Green, 1f);
            }
            else
            {
                this.BubbleText.AddText(changedAmount.ToString(), player.Position, Color.Red, 0.8f);
            }
        }

        IPlayerList PlayerList;

        ITextureBank<PlayerTexture> PlayerTextureBank;
        ISpriteBatch SpriteBatch;
        ICamera Camera;
        IWeaponView RailGunView;
        IBubbleTextDrawer BubbleText;
    }
}
