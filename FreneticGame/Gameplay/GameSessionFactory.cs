using System;
using Lidgren.Network;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Autofac.Builder;
using Autofac;
using Frenetic.Physics;
using Frenetic.Level;
using FarseerGames.FarseerPhysics;
using FarseerGames.FarseerPhysics.Factories;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Collisions;
using Frenetic.Graphics;
using Microsoft.Xna.Framework;
using FarseerGames.GettingStarted;
using Frenetic.Network.Lidgren;
using Frenetic.Network;
using Frenetic.Autofac;
using Frenetic.UserInput;

namespace Frenetic
{
    public class GameSessionFactory : IGameSessionFactory
    {
        const int _screenWidth = 800;
        const int _screenHeight = 600;
        Vector2 _gravity = new Vector2(0, 2);

        public GameSessionFactory(GraphicsDevice graphicsDevice, ContentManager contentManager, IContainer container)
        {
            _graphicsDevice = graphicsDevice;
            _contentManager = contentManager;

            ClientContainer = container.CreateInnerContainer();
            ServerContainer = container.CreateInnerContainer();
        }
        #region IGameSessionFactory Members

        public GameSessionControllerAndView MakeServerGameSession()
        {
            // TODO: Move this somewhere more appropriate
            // Make queues for a SERVER network session:
            // *****************************************
            IServerNetworkSession serverNetworkSession = ServerContainer.Resolve<IServerNetworkSession>();
            IIncomingMessageQueue incomingMessageQueue = ServerContainer.Resolve<IIncomingMessageQueue>(
                    new TypedParameter(typeof(INetworkSession), serverNetworkSession));
            IOutgoingMessageQueue outgoingMessageQueue = ServerContainer.Resolve<IOutgoingMessageQueue>(
                    new TypedParameter(typeof(IClientNetworkSession), null));
            serverNetworkSession.Create(14242);
            // *****************************************

            //CreateGeneralComponents();
            IGameSession gameSession = ServerContainer.Resolve<IGameSession>();

            GameSessionController gameSessionController = ServerContainer.Resolve<GameSessionController>(new TypedParameter(typeof(IPlayer), null));
            GameSessionView gameSessionView = ServerContainer.Resolve<GameSessionView>();

            return new GameSessionControllerAndView(gameSession, gameSessionController, gameSessionView);
        }

        
        public GameSessionControllerAndView MakeClientGameSession()
        {
            // TODO: Move this somewhere more appropriate
            // Make queues for a CLIENT network session:
            // *****************************************
            IClientNetworkSession clientNetworkSession = ClientContainer.Resolve<IClientNetworkSession>();
            IIncomingMessageQueue incomingMessageQueue = ClientContainer.Resolve<IIncomingMessageQueue>(
                    new TypedParameter(typeof(INetworkSession), clientNetworkSession));
            IOutgoingMessageQueue outgoingMessageQueue = ClientContainer.Resolve<IOutgoingMessageQueue>(
                    new TypedParameter(typeof(IServerNetworkSession), null));
            clientNetworkSession.Join(14242);
            // *****************************************

            IGameSession gameSession = ClientContainer.Resolve<IGameSession>();
            
            IPlayer localPlayer = CreateClientComponents(gameSession);

            gameSession.Controllers.Add(ClientContainer.Resolve<FarseerPhysicsController>());
            Frenetic.Level.Level level = ClientContainer.Resolve<Frenetic.Level.Level>();

            gameSession.Controllers.Add(ClientContainer.Resolve<LevelController>(new TypedParameter(typeof(Frenetic.Level.Level), level)));

            GameSessionController gameSessionController = ClientContainer.Resolve<GameSessionController>(
                new TypedParameter(typeof(IPlayer), localPlayer),
                new TypedParameter(typeof(bool), false));
            GameSessionView gameSessionView = ClientContainer.Resolve<GameSessionView>();

            return new GameSessionControllerAndView(gameSession, gameSessionController, gameSessionView);
        }

        private IPlayer CreateClientComponents(IGameSession gameSession)
        {
            ITexture playerTexture = ClientContainer.Resolve<ITexture>(new TypedParameter(typeof(Texture2D), _contentManager.Load<Texture2D>("textures/ball")));
            PlayerSettings playerSettings = ClientContainer.Resolve<PlayerSettings>(new TypedParameter(typeof(ITexture), playerTexture));
            //IViewFactory viewFactory = ClientContainer.Resolve<IViewFactory>(new TypedParameter(typeof(ITexture), playerTexture));

            // Mediator Controllers:
            _mediatorPlayerController = ClientContainer.Resolve<MediatorPlayerSettingsController>();
            _mediatorPhysicsController = ClientContainer.Resolve<MediatorPhysicsSettingsController>();

            // Make local player:
            IPlayer localPlayer = ClientContainer.Resolve<IPlayer>(new TypedParameter(typeof(int), 0));

            gameSession.Controllers.Add(ClientContainer.Resolve<FarseerPhysicsController>());
            Frenetic.Level.Level level = ClientContainer.Resolve<Frenetic.Level.Level>();
            gameSession.Controllers.Add(ClientContainer.Resolve<LevelController>(new TypedParameter(typeof(Frenetic.Level.Level), level)));

            ICamera camera = ClientContainer.Resolve<ICamera>
                                            (
                                            new TypedParameter(typeof(IPlayer), localPlayer),
                                            new TypedParameter(typeof(Vector2), new Vector2(_screenWidth, _screenHeight))
                                            );
            gameSession.Controllers.Add(ClientContainer.Resolve<KeyboardPlayerController>(new TypedParameter(typeof(IPlayer), localPlayer)));
            gameSession.Views.Add(ClientContainer.Resolve<NetworkPlayerView>(new TypedParameter(typeof(IPlayer), localPlayer)));

            ITexture levelTexture = ClientContainer.Resolve<ITexture>(new TypedParameter(typeof(Texture2D), _contentManager.Load<Texture2D>("Textures/blank")));
            gameSession.Views.Add(ClientContainer.Resolve<LevelView>(new TypedParameter(typeof(ITexture), levelTexture)));

            ITexture crosshairTexture = ClientContainer.Resolve<ITexture>(new TypedParameter(typeof(Texture2D), _contentManager.Load<Texture2D>("Textures/cursor")));
            gameSession.Views.Add(ClientContainer.Resolve<CrosshairView>(new TypedParameter(typeof(ITexture), crosshairTexture)));

            ITexture consoleTexture = ClientContainer.Resolve<ITexture>(new TypedParameter(typeof(Texture2D), _contentManager.Load<Texture2D>("Textures/blank")));
            IFont consoleFont = ClientContainer.Resolve<IFont>(new TypedParameter(typeof(SpriteFont), _contentManager.Load<SpriteFont>("Fonts/detailsFont")));
            int edgeGap = 4;
            int inputWindowHeight = 24;
            ClientContainer.Resolve<IGameConsole>();
            gameSession.Views.Add(ClientContainer.Resolve<GameConsoleView>(
                            new NamedParameter("inputWindow", new Rectangle(edgeGap, _screenHeight - edgeGap - inputWindowHeight, _screenWidth - 2 * edgeGap, inputWindowHeight)),
                            new NamedParameter("commandWindow", new Rectangle(edgeGap, edgeGap, (_screenWidth / 2) - 30 - edgeGap, _screenHeight - inputWindowHeight - 3 * edgeGap)),
                            new NamedParameter("messageWindow", new Rectangle((_screenWidth / 2) + 30 + edgeGap, edgeGap, (_screenWidth / 2) - 30 - 2*edgeGap, (_screenHeight / 2) - 2*edgeGap)),
                            new TypedParameter(typeof(ITexture), consoleTexture), new TypedParameter(typeof(IFont), consoleFont)));
            gameSession.Controllers.Add(ClientContainer.Resolve<GameConsoleController>());
            
            // TEMP CODE:
            // *********************************************************************************
            //gameSession.Controllers.Add(ClientContainer.Resolve<DumbRayCasterTestController>());

            // DEBUG VIEW:  // TODO: Write a controller for this...
            // TODO: Instantiate with Autofac
            PhysicsSimulator physicsSimulator = ClientContainer.Resolve<PhysicsSimulator>();
            var debugView = new PhysicsSimulatorView(physicsSimulator, new SpriteBatch(_graphicsDevice), camera);
            physicsSimulator.EnableDiagnostics = true;
            debugView.LoadContent(_graphicsDevice, _contentManager);
            gameSession.Views.Add(debugView);
            // *********************************************************************************

            return localPlayer;
        }

        public void Dispose()
        {
            ClientContainer.Dispose();
            ServerContainer.Dispose();
        }

        #endregion

        GraphicsDevice _graphicsDevice;
        ContentManager _contentManager;
        IContainer ClientContainer { get; set; }
        IContainer ServerContainer { get; set; }

        // CAN BE REMOVED?
        MediatorPlayerSettingsController _mediatorPlayerController;
        MediatorPhysicsSettingsController _mediatorPhysicsController;
    }

    public class GameSessionControllerAndView
    {
        public GameSessionControllerAndView(IGameSession gameSession, IController controller, IView view)
        {
            GameSession = gameSession;
            GameSessionController = controller;
            GameSessionView = view;
        }

        public IGameSession GameSession { get; private set; }
        public IController GameSessionController { get; private set; }
        public IView GameSessionView { get; private set; }
    }
    
}
