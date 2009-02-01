using System;

namespace Frenetic
{
    public interface IViewFactory
    {
        PlayerView MakePlayerView(IPlayer player, ICamera camera);
    }
}
