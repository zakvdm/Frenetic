using System;
using Microsoft.Xna.Framework;
namespace Frenetic
{
    public interface ICrosshair
    {
        int Size { get; set; }
        Vector2 ViewPosition { get; }
        Vector2 WorldPosition { get; }
    }
}
