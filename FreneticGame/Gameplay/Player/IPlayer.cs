using System;
using Microsoft.Xna.Framework;
using FarseerGames.FarseerPhysics.Dynamics;
using Frenetic.Weapons;
namespace Frenetic.Player
{
    public interface IPlayer
    {
        bool IsAlive { get; set; }
        Vector2 Position { get; set; }

        IRailGun CurrentWeapon { get; }

        void Update();
        bool Jump();
        void MoveLeft();
        void MoveRight();
        void Shoot(Vector2 targetPosition);

        Vector2? PendingShot { get; set; }
    }
}
