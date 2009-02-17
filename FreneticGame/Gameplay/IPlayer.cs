using System;
using Microsoft.Xna.Framework;
using FarseerGames.FarseerPhysics.Dynamics;
namespace Frenetic
{
    public interface IPlayer
    {
        int ID { get; set; }
        Vector2 Position { get; set; }

        void Update();
        void Jump(long time);
        void MoveLeft();
        void MoveRight();
        void Shoot(Vector2 targetPosition);
    }
}
