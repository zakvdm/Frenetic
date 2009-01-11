using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frenetic
{
    public interface IScreenFactory
    {
        GameplayScreen MakeGameplayScreen(GameSessionControllerAndView serverGameSessionCandV, GameSessionControllerAndView clientGameSessionCandV);
        GameplayScreen MakeGameplayScreen(GameSessionControllerAndView clientGameSessionCandV);
        MainMenuScreen MakeMainMenuScreen();
    }
}
