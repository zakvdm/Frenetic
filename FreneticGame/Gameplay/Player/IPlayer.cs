using System;
using Microsoft.Xna.Framework;
using FarseerGames.FarseerPhysics.Dynamics;
using Frenetic.Weapons;
using Frenetic.Gameplay;
namespace Frenetic.Player
{
    public interface IPlayer : IPlayerInput
    {
        bool IsAlive { get; set; }

        IRailGun CurrentWeapon { get; }

        IPlayerSettings PlayerSettings { get; }
        PlayerScore PlayerScore { get; }

        void Update();
        bool Jump();
        void MoveLeft();
        void MoveRight();
        void Shoot(Vector2 targetPosition);

        // NETWORK:
        void UpdatePositionFromNetwork(Vector2 newestPosition, float deliveryTime);

        // EVENTS:
        event Action Died;
    }
}
