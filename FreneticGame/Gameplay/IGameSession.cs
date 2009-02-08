using System;
using System.Collections.Generic;

namespace Frenetic
{
    public interface IGameSession
    {
        List<IView> Views { get; }
        List<IController> Controllers { get; }

        IController GameSessionController { get; set; }
        IView GameSessionView { get; set; }
    }
}
