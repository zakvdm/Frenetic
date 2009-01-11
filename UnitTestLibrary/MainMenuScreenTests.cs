using System;
using System.Collections.Generic;
using System.Text;

using Frenetic;
using Microsoft.Xna.Framework.Graphics;

using NUnit.Framework;
using Rhino.Mocks;

namespace UnitTestLibrary
{
    [TestFixture]
    public class MainMenuScreenTests
    {
        IGameSessionFactory stubGameSessionFactory;
        IScreenFactory stubScreenFactory;
        MainMenuScreen mainMenuScreen;
        
        [SetUp]
        public void Setup()
        {
            stubGameSessionFactory = MockRepository.GenerateStub<IGameSessionFactory>();
            stubScreenFactory = MockRepository.GenerateStub<IScreenFactory>();
            mainMenuScreen = new MainMenuScreen(new Viewport(), null, null, stubGameSessionFactory, stubScreenFactory);
        }
        
        [Test]
        public void CanCreateAndStartServerGameSession()
        {
            var stubInputState = MockRepository.GenerateStub<IInputState>();
            var serverGSCandV = new GameSessionControllerAndView(null, null);
            var clientGSCandV = new GameSessionControllerAndView(null, null);

            stubInputState.Stub(x => x.MenuSelect).Return(true);
            stubGameSessionFactory.Stub(x => x.MakeServerGameSession()).Return(serverGSCandV);
            stubGameSessionFactory.Stub(x => x.MakeClientGameSession()).Return(clientGSCandV);

            mainMenuScreen.State = MainMenuScreen.MainMenuState.Network;
            mainMenuScreen.HandleInput(stubInputState);

            stubScreenFactory.AssertWasCalled(x => x.MakeGameplayScreen(serverGSCandV, clientGSCandV));
        }

        [Test]
        public void CanCreateClientGameSession()
        {
            var stubInputState = MockRepository.GenerateStub<IInputState>();
            var clientGSCandV = new GameSessionControllerAndView(null, null);

            // Create network session is option 2:
            stubInputState.Stub(x => x.MenuDown).Return(true).Repeat.Once();
            stubInputState.Stub(x => x.MenuSelect).Return(true).Repeat.Once();
            
            stubGameSessionFactory.Stub(x => x.MakeClientGameSession()).Return(clientGSCandV);

            mainMenuScreen.State = MainMenuScreen.MainMenuState.Network;
            mainMenuScreen.HandleInput(stubInputState);

            stubScreenFactory.AssertWasCalled(x => x.MakeGameplayScreen(Arg<GameSessionControllerAndView>.Is.Equal(clientGSCandV)));
        }
    }
}
