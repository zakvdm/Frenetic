using System;
using Microsoft.Xna.Framework;
using FarseerGames.FarseerPhysics.Dynamics;
using Frenetic.Weapons;
namespace Frenetic.Player
{
    public interface IPlayer
    {
        Vector2 Position { get; set; }

        IRailGun CurrentWeapon { get; }

        void Update();
        void AddWeapon(IRailGun railGun);
        bool Jump();
        void MoveLeft();
        void MoveRight();
        void Shoot(Vector2 targetPosition);
    }
}
