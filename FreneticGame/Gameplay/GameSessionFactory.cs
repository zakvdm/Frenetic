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
        Vector2 _gravity = new Vector2(0, 6);

        public GameSessionFactory(GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            _graphicsDevice = graphicsDevice;
            _contentManager = contentManager;
        }
        #region IGameSessionFactory Members

        public GameSessionControllerAndView MakeServerGameSession()
        {
            IContainer container = RegisterAllComponents();

            // TODO: Move this somewhere more appropriate
            // Make queues for a SERVER network session:
            // *****************************************
            IServerNetworkSession serverNetworkSession = container.Resolve<IServerNetworkSession>();
            NetworkSessionManager networkSessionManager = container.Resolve<NetworkSessionManager>(
                    new TypedParameter(typeof(IClientNetworkSession), null));
            IIncomingMessageQueue incomingMessageQueue = container.Resolve<IIncomingMessageQueue>(
                    new TypedParameter(typeof(INetworkSession), serverNetworkSession));
            IOutgoingMessageQueue outgoingMessageQueue = container.Resolve<IOutgoingMessageQueue>(
                    new TypedParameter(typeof(IClientNetworkSession), null));
            networkSessionManager.Start(14242);
            // *****************************************

            CreateGeneralComponents(container);


            GameSessionController gameSessionController = container.Resolve<GameSessionController>
                            (
                            new TypedParameter(typeof(IPlayer), null),
                            new TypedParameter(typeof(ICamera), null),
                            new TypedParameter(typeof(bool), true)
                            );
            GameSessionView gameSessionView = container.Resolve<GameSessionView>();

            return new GameSessionControllerAndView(container.Resolve<IGameSession>(), gameSessionController, gameSessionView);
        }

        
        public GameSessionControllerAndView MakeClientGameSession()
        {
            IContainer container = RegisterAllComponents();

            // TODO: Move this somewhere more appropriate
            // Make queues for a CLIENT network session:
            // *****************************************
            IClientNetworkSession clientNetworkSession = container.Resolve<IClientNetworkSession>();
            NetworkSessionManager networkSessionManager = container.Resolve<NetworkSessionManager>(
                    new TypedParameter(typeof(IServerNetworkSession), null));
            IIncomingMessageQueue incomingMessageQueue = container.Resolve<IIncomingMessageQueue>(
                    new TypedParameter(typeof(INetworkSession), clientNetworkSession));
            IOutgoingMessageQueue outgoingMessageQueue = container.Resolve<IOutgoingMessageQueue>(
                    new TypedParameter(typeof(IServerNetworkSession), null));
            networkSessionManager.Join(14242);
            // *****************************************

            IPlayer localPlayer = CreateClientComponents(container);

            CreateGeneralComponents(container);

            GameSessionController gameSessionController = container.Resolve<GameSessionController>(
                                new TypedParameter(typeof(IPlayer), localPlayer),
                                new TypedParameter(typeof(bool), false));
            GameSessionView gameSessionView = container.Resolve<GameSessionView>();

            return new GameSessionControllerAndView(container.Resolve<IGameSession>(), gameSessionController, gameSessionView);
        }
        
        private IContainer RegisterAllComponents()
        {
            var builder = new ContainerBuilder();

            #region Networking
            builder.Register(new NetServer(new NetConfiguration("Frenetic"))).SingletonScoped();
            builder.Register(new NetClient(new NetConfiguration("Frenetic"))).SingletonScoped();
            builder.Register<NetServerWrapper>().As<INetServer>().SingletonScoped();
            builder.Register<NetClientWrapper>().As<INetClient>().SingletonScoped();
            builder.Register<LidgrenServerNetworkSession>().As<IServerNetworkSession>().SingletonScoped();
            builder.Register<LidgrenClientNetworkSession>().As<IClientNetworkSession>().SingletonScoped();
            builder.Register<NetworkSessionManager>().SingletonScoped();
            builder.Register<IncomingMessageQueue>().As<IIncomingMessageQueue>().SingletonScoped();
            builder.Register<OutgoingMessageQueue>().As<IOutgoingMessageQueue>().SingletonScoped();
            builder.Register<XmlMessageSerializer>().As<IMessageSerializer>().SingletonScoped();
            #endregion

            #region Graphics
            builder.Register<SpriteBatch>(new SpriteBatch(_graphicsDevice));
            builder.Register<XnaSpriteBatch>().As<ISpriteBatch>().FactoryScoped();
            builder.Register<XnaTexture>().As<ITexture>().FactoryScoped();
            builder.Register<XnaFont>().As<IFont>().FactoryScoped();
            #endregion

            #region GameSession
            builder.Register<GameSession>().As<IGameSession>().SingletonScoped();
            builder.Register<GameSessionController>().SingletonScoped();
            builder.Register<GameSessionView>().SingletonScoped();
            #endregion

            #region Player
            builder.Register<PlayerSettings>().SingletonScoped();
            builder.Register<Player>().As<IPlayer>().FactoryScoped();
            builder.Register<PlayerView>().FactoryScoped();
            builder.RegisterGeneratedFactory<Player.Factory>(new TypedService(typeof(IPlayer)));
            builder.RegisterGeneratedFactory<PlayerView.Factory>(new TypedService(typeof(PlayerView)));
            builder.Register((c) => (IBoundaryCollider)new WorldBoundaryCollider(_screenWidth, _screenHeight));
            builder.Register<KeyboardPlayerController>().SingletonScoped();
            builder.Register<NetworkPlayerView>().SingletonScoped();
            #endregion

            #region Physics
            builder.RegisterModule(new PhysicsModule() { Gravity = _gravity });
            #endregion

            #region Level
            builder.Register<LevelPiece>().FactoryScoped();
            builder.RegisterGeneratedFactory<LevelPiece.Factory>(new TypedService(typeof(LevelPiece)));
            builder.Register<DumbLevelLoader>().As<ILevelLoader>().SingletonScoped();
            builder.Register<Frenetic.Level.Level>().SingletonScoped();
            builder.Register<LevelController>();
            builder.Register<LevelView>();
            #endregion

            // RAYCASTER:
            builder.Register<DumbRayCaster>().SingletonScoped();
            builder.Register<DumbRayCasterTestController>().FactoryScoped();

            // CAMERA:
            builder.Register((c, p) => (ICamera)new Camera(p.TypedAs<IPlayer>(), new Vector2(_screenWidth, _screenHeight))).SingletonScoped();
            builder.Register<Camera>().As<ICamera>().SingletonScoped();

            // CROSSHAIR:
            builder.Register<Crosshair>().As<ICrosshair>().SingletonScoped();
            builder.Register<CrosshairView>().SingletonScoped();

            // KEYBOARD:
            builder.Register<XNAKeyboard>().As<IKeyboard>().SingletonScoped();

            // CONSOLE:
            builder.Register<Mediator>().As<IMediator>().SingletonScoped();
            builder.Register<GameConsole>().As<IGameConsole>().SingletonScoped();
            builder.Register<GameConsoleView>().SingletonScoped();
            builder.Register<GameConsoleController>().SingletonScoped();
            // MEDIATOR CONTROLLERS:
            builder.Register<MediatorPlayerSettingsController>().SingletonScoped();
            builder.Register<MediatorPhysicsSettingsController>().SingletonScoped();

            return builder.Build();
        }

        private Player.Factory CreateGeneralComponents(IContainer container)
        {
            ITexture playerTexture = container.Resolve<ITexture>(new TypedParameter(typeof(Texture2D), _contentManager.Load<Texture2D>("Content/textures/ball")));
            PlayerSettings playerSettings = container.Resolve<PlayerSettings>(new TypedParameter(typeof(ITexture), playerTexture));

            // Mediator Controllers:
            _mediatorPlayerController = container.Resolve<MediatorPlayerSettingsController>();
            _mediatorPhysicsController = container.Resolve<MediatorPhysicsSettingsController>();
            
            IGameSession gameSession = container.Resolve<IGameSession>();

            gameSession.Controllers.Add(container.Resolve<FarseerPhysicsController>());
            Frenetic.Level.Level level = container.Resolve<Frenetic.Level.Level>();
            
            gameSession.Controllers.Add(container.Resolve<LevelController>(new TypedParameter(typeof(Frenetic.Level.Level), level)));

            return container.Resolve<Player.Factory>();
        }

        private IPlayer CreateClientComponents(IContainer container)
        {
            // Make local player:
            IPlayer localPlayer = container.Resolve<IPlayer>(new TypedParameter(typeof(int), 0));

            IGameSession gameSession = container.Resolve<IGameSession>();

            gameSession.Controllers.Add(container.Resolve<KeyboardPlayerController>(new TypedParameter(typeof(IPlayer), localPlayer)));
            gameSession.Views.Add(container.Resolve<NetworkPlayerView>
                            (
                            new TypedParameter(typeof(IPlayer), localPlayer)
                            ));
            ICamera camera = container.Resolve<ICamera>
                                            (
                                            new TypedParameter(typeof(IPlayer), localPlayer),
                                            new TypedParameter(typeof(Vector2), new Vector2(_screenWidth, _screenHeight))
                                            );

            ITexture levelTexture = container.Resolve<ITexture>(new TypedParameter(typeof(Texture2D), _contentManager.Load<Texture2D>("Content/Textures/blank")));
            gameSession.Views.Add(container.Resolve<LevelView>(new TypedParameter(typeof(ITexture), levelTexture)));

            ITexture crosshairTexture = container.Resolve<ITexture>(new TypedParameter(typeof(Texture2D), _contentManager.Load<Texture2D>("Content/Textures/cursor")));
            gameSession.Views.Add(container.Resolve<CrosshairView>(new TypedParameter(typeof(ITexture), crosshairTexture)));

            ITexture consoleTexture = container.Resolve<ITexture>(new TypedParameter(typeof(Texture2D), _contentManager.Load<Texture2D>("Content/Textures/blank")));
            IFont consoleFont = container.Resolve<IFont>(new TypedParameter(typeof(SpriteFont), _contentManager.Load<SpriteFont>("Content/Fonts/detailsFont")));
            int edgeGap = 4;
            int inputWindowHeight = 24;
            container.Resolve<IGameConsole>();
            gameSession.Views.Add(container.Resolve<GameConsoleView>(
                            new NamedParameter("inputWindow", new Rectangle(edgeGap, _screenHeight - edgeGap - inputWindowHeight, _screenWidth - 2 * edgeGap, inputWindowHeight)),
                            new NamedParameter("commandWindow", new Rectangle(edgeGap, edgeGap, (_screenWidth / 2) - 30 - edgeGap, _screenHeight - inputWindowHeight - 3 * edgeGap)),
                            new NamedParameter("messageWindow", new Rectangle((_screenWidth / 2) + 30 + edgeGap, edgeGap, (_screenWidth / 2) - 30 - 2*edgeGap, (_screenHeight / 2) - 2*edgeGap)),
                            new TypedParameter(typeof(ITexture), consoleTexture), new TypedParameter(typeof(IFont), consoleFont)));
            gameSession.Controllers.Add(container.Resolve<GameConsoleController>());
            
            // TEMP CODE:
            // *********************************************************************************
            //gameSession.Controllers.Add(container.Resolve<DumbRayCasterTestController>());


            // DEBUG VIEW:  // TODO: Write a controller for this...
            // TODO: Instantiate with Autofac
            PhysicsSimulator physicsSimulator = container.Resolve<PhysicsSimulator>();
            var debugView = new PhysicsSimulatorView(physicsSimulator, new SpriteBatch(_graphicsDevice), camera);
            physicsSimulator.EnableDiagnostics = true;
            debugView.LoadContent(_graphicsDevice, _contentManager);
            gameSession.Views.Add(debugView);
            // *********************************************************************************

            return localPlayer;
        }

        MediatorPlayerSettingsController _mediatorPlayerController;
        MediatorPhysicsSettingsController _mediatorPhysicsController;
        #endregion

        GraphicsDevice _graphicsDevice;
        ContentManager _contentManager;
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
