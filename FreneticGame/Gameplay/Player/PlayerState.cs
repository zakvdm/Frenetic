using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Frenetic.Weapons;
using Frenetic.Gameplay;

namespace Frenetic.Player
{
    public interface IPlayerState
    {
        PlayerStatus Status { get; set; }
        Vector2 Position { get; set; }
        List<Shot> NewShots { get; set; }

        PlayerScore Score { get; set; }

        void RefreshPlayerValuesFromState(IPlayer player);
    }

    public class PlayerState : IPlayerState
    {
        public PlayerState()
        {
            NewShots = new List<Shot>();
            this.Score = new PlayerScore();
        }
        public PlayerState(IPlayer player)
            : this()
        {
            if (player != null)
            {
                this.Status = player.Status;
                this.Position = player.Position;
                this.Score = player.PlayerScore;

                if (player.CurrentWeapon.Shots.IsDirty)
                {
                    this.NewShots.AddRange(player.CurrentWeapon.Shots.GetDiff());
                    player.CurrentWeapon.Shots.Clean();
                }
            }
        }

        public PlayerStatus Status { get; set; }
        public Vector2 Position { get; set; }
        public List<Shot> NewShots { get; set; }
        public PlayerScore Score { get; set; }

        public void RefreshPlayerValuesFromState(IPlayer player)
        {
            // TODO: Implement a rolling average
            player.UpdatePositionFromNetwork(this.Position, 0.1f);

            player.Status = this.Status;
            if (player.Status == player.PendingStatus)
            {
                // When the PendingStatus is achieved, we can reset it
                player.PendingStatus = null;
            }

            player.CurrentWeapon.Shots.AddRange(this.NewShots);

            player.PlayerScore.Kills = this.Score.Kills;
            player.PlayerScore.Deaths = this.Score.Deaths;
        }
    }
}
