using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frenetic
{
    public interface IScreenFactory
    {
        GameplayScreen MakeGameplayScreen(IController serverGameSessionController, IController clientGameSessionController);
        GameplayScreen MakeGameplayScreen(IController clientGameSessionController);
        MainMenuScreen MakeMainMenuScreen();
    }
}
