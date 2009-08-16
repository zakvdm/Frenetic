using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Frenetic.Weapons;
using Frenetic.Gameplay;

namespace Frenetic.Player
{
    public enum PlayerType
    {
        Local,
        Network
    }

    public interface IPlayerState
    {
        bool IsAlive { get; set; }
        Vector2 Position { get; set; }
        List<Shot> NewShots { get; set; }

        PlayerScore Score { get; set; }

        void RefreshPlayerValuesFromState(IPlayer player, PlayerType playerType);
    }

    public class PlayerState : IPlayerState
    {
        public PlayerState()
        {
            NewShots = new List<Shot>();
            this.Score = new PlayerScore();
        }
        public PlayerState(IPlayer player)
        {
            if (player != null)
            {
                this.IsAlive = player.IsAlive;
                this.Position = player.Position;
                this.Score = player.PlayerScore;

                if (player.CurrentWeapon.Shots.IsDirty)
                {
                    this.NewShots = new List<Shot>(player.CurrentWeapon.Shots.GetDiff());
                    player.CurrentWeapon.Shots.Clean();
                }
            }
        }

        public bool IsAlive { get; set; }
        public Vector2 Position { get; set; }
        public List<Shot> NewShots { get; set; }
        public PlayerScore Score { get; set; }

        public void RefreshPlayerValuesFromState(IPlayer player, PlayerType playerType)
        {
            if (playerType == PlayerType.Network)
            {
                player.Position = this.Position;
            }

            player.IsAlive = this.IsAlive;
            player.CurrentWeapon.Shots.AddRange(this.NewShots);

            player.PlayerScore.Kills = this.Score.Kills;
            player.PlayerScore.Deaths = this.Score.Deaths;
        }
    }
}
