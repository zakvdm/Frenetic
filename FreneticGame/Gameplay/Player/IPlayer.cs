﻿using System;
using Microsoft.Xna.Framework;
using FarseerGames.FarseerPhysics.Dynamics;
using Frenetic.Weapons;
using Frenetic.Gameplay;
namespace Frenetic.Player
{
    public interface IPlayer
    {
        bool IsAlive { get; set; }
        Vector2 Position { get; set; }

        IRailGun CurrentWeapon { get; }

        IPlayerSettings PlayerSettings { get; }
        PlayerScore PlayerScore { get; }

        void Update();
        bool Jump();
        void MoveLeft();
        void MoveRight();
        void Shoot(Vector2 targetPosition);

        event Action OnDeath;

        Vector2? PendingShot { get; set; }
    }
}
