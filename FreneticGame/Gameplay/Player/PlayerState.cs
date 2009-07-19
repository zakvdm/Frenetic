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
        List<Shot> Shots { get; set; }

        PlayerScore Score { get; set; }

        void RefreshPlayerValuesFromState(IPlayer player, PlayerType playerType);
    }

    public class PlayerState : IPlayerState
    {
        public PlayerState()
        {
            Shots = new List<Shot>();
            this.Score = new PlayerScore();
        }
        public PlayerState(IPlayer player)
        {
            if (player != null)
            {
                this.IsAlive = player.IsAlive;
                this.Position = player.Position;
                this.Shots = player.CurrentWeapon.Shots;

                this.Score = player.PlayerScore;
            }
        }

        public bool IsAlive { get; set; }
        public Vector2 Position { get; set; }
        public List<Shot> Shots { get; set; }
        public PlayerScore Score { get; set; }

        public void RefreshPlayerValuesFromState(IPlayer player, PlayerType playerType)
        {
            if (playerType == PlayerType.Network)
            {
                player.Position = this.Position;
            }

            player.IsAlive = this.IsAlive;
            player.CurrentWeapon.Shots.Clear();
            player.CurrentWeapon.Shots.AddRange(this.Shots);

            player.PlayerScore.Kills = this.Score.Kills;
            player.PlayerScore.Deaths = this.Score.Deaths;
        }
    }
}
