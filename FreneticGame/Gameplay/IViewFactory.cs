using System;

namespace Frenetic
{
    public interface IViewFactory
    {
        PlayerView MakePlayerView(Player player);
    }
}
