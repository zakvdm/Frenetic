using System;
using Microsoft.Xna.Framework;

namespace Frenetic
{
    public class Player
    {
        public Player(int ID)
        {
            this.ID = ID;
        }
        public Player(){}

        public int ID { get; set; }
        public Vector2 Position { get; set; }
    }
}
