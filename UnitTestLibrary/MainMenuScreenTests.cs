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
            mainMenuScreen = new MainMenuScreen(new Viewport(), null, null, stubGameSessionFactory, stubScreenFactory, null);
        }
        
        [Test]
        public void CanCreateAndStartServerGameSession()
        {
            var serverGSCandV = new GameSessionControllerAndView(null, null, null);
            var clientGSCandV = new GameSessionControllerAndView(null, null, null);

            stubGameSessionFactory.Stub(x => x.MakeServerGameSession()).Return(serverGSCandV);
            stubGameSessionFactory.Stub(x => x.MakeClientGameSession()).Return(clientGSCandV);

            mainMenuScreen.State = MainMenuScreen.MainMenuState.Network;
            mainMenuScreen.OnSelectEntry(0);    // Create a session selected

            stubScreenFactory.AssertWasCalled(x => x.MakeGameplayScreen(clientGSCandV, serverGSCandV));
        }

        [Test]
        public void CanCreateClientGameSession()
        {
            var clientGSCandV = new GameSessionControllerAndView(null, null, null);

            stubGameSessionFactory.Stub(x => x.MakeClientGameSession()).Return(clientGSCandV);

            mainMenuScreen.State = MainMenuScreen.MainMenuState.Network;
            mainMenuScreen.OnSelectEntry(1);    // Join a session selected

            stubScreenFactory.AssertWasCalled(x => x.MakeGameplayScreen(Arg<GameSessionControllerAndView>.Is.Equal(clientGSCandV), Arg<GameSessionControllerAndView>.Is.Null));
        }

        [Test]
        public void CanExitGame()
        {
            MessageBoxScreen mbs = new MessageBoxScreen("Exit Frenetic?", false, null, null, new Viewport(), null, null);
            stubScreenFactory.Stub(x => x.MakeMessageBoxScreen(Arg<string>.Is.Anything)).Return(mbs);

            mainMenuScreen.OnCancel();

            stubScreenFactory.AssertWasCalled(x => x.MakeMessageBoxScreen(Arg<string>.Is.Equal("Exit Frenetic?")));
            // TODO: How can i check that MainMenuScreen correctly registers event handlers?
        }
    }
}
