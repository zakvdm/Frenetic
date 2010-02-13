using System;
using Microsoft.Xna.Framework;
using FarseerGames.FarseerPhysics.Dynamics;
using Frenetic.Gameplay.Weapons;
using Frenetic.Gameplay;

namespace Frenetic.Player
{
    public enum PlayerStatus
    {
        Alive,
        Dead
    }
    public interface IPlayer 
    {
        // PROPERTIES:
        PlayerStatus? PendingStatus { get; set; }
        Vector2? PendingShot { get; set; }

        int Health { get; set; }

        Vector2 Position { get; set; }

        PlayerStatus Status { get; set; }

        IWeapons Weapons { get; }

        IPlayerSettings PlayerSettings { get; }
        PlayerScore PlayerScore { get; }

        bool InContactWithLevel { get; set; }

        // METHODS:
        void Update();
        bool Jump();
        void MoveLeft();
        void MoveRight();
        void Shoot(Vector2 targetPosition);

        // NETWORK:
        void UpdatePositionFromNetwork(Vector2 newestPosition, float deliveryTime);

        // EVENTS:
        event Action Died;
        event Action<IPlayer, int> HealthChanged;
    }
}
