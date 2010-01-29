using System;
using Microsoft.Xna.Framework;

namespace Frenetic.Graphics
{
    public interface IEffect
    {
        int ShotsDrawn { get; set; }

        void Trigger(Vector2 startPoint, Vector2 endPoint);
        void Draw(ref Matrix transform);
        void Update(float totalSeconds, float elapsedSeconds);
    }
}
