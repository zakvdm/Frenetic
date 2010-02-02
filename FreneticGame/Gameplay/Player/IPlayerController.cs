using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frenetic.Player
{
    public interface IPlayerController : IController
    {
        void RemoveDeadProjectiles();
    }
}
