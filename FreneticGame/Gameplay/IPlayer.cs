using System;
using Microsoft.Xna.Framework;
namespace Frenetic
{
    public interface IPlayer
    {
        int ID { get; set; }
        Vector2 Position { get; set; }
        
        void Update();
    }
}
