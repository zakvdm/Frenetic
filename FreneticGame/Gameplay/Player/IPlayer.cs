﻿using System;
using Microsoft.Xna.Framework;
using FarseerGames.FarseerPhysics.Dynamics;
namespace Frenetic
{
    public interface IPlayer
    {
        Vector2 Position { get; set; }

        void Update();
        bool Jump();
        void MoveLeft();
        void MoveRight();
        void Shoot(Vector2 targetPosition);
    }
}
