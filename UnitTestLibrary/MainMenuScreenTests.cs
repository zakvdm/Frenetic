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
            var stubGSCServer = MockRepository.GenerateStub<IController>();
            var stubGSCClient = MockRepository.GenerateStub<IController>();

            stubInputState.Stub(x => x.MenuSelect).Return(true);
            stubGameSessionFactory.Stub(x => x.MakeServerGameSession()).Return(stubGSCServer);
            stubGameSessionFactory.Stub(x => x.MakeClientGameSession()).Return(stubGSCClient);

            mainMenuScreen.State = MainMenuScreen.MainMenuState.Network;
            mainMenuScreen.HandleInput(stubInputState);

            stubScreenFactory.AssertWasCalled(x => x.MakeGameplayScreen(stubGSCServer, stubGSCClient));
        }

        [Test]
        public void CanCreateClientGameSession()
        {
            var stubInputState = MockRepository.GenerateStub<IInputState>();
            var stubGSC = MockRepository.GenerateStub<IController>();

            // Create network session is option 2:
            stubInputState.Stub(x => x.MenuDown).Return(true).Repeat.Once();
            stubInputState.Stub(x => x.MenuSelect).Return(true).Repeat.Once();
            
            stubGameSessionFactory.Stub(x => x.MakeClientGameSession()).Return(stubGSC);

            mainMenuScreen.State = MainMenuScreen.MainMenuState.Network;
            mainMenuScreen.HandleInput(stubInputState);

            stubScreenFactory.AssertWasCalled(x => x.MakeGameplayScreen(Arg<GameSessionController>.Is.Equal(stubGSC)));
        }
    }
}
