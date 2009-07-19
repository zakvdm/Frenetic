using System;

namespace Frenetic.Graphics
{
    public interface IEffect
    {
        void Update(float totalSeconds, float elapsedSeconds);
    }
}
