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

namespace Frenetic
{
    public class GameSessionFactory : IGameSessionFactory
    {
        const int _screenWidth = 800;
        const int _screenHeight = 600;
        Vector2 _gravity = new Vector2(0, 2);

        public GameSessionFactory(GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            _graphicsDevice = graphicsDevice;
            _contentManager = contentManager;

            IContainer container = RegisterAllComponents();
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
        
        private IContainer RegisterAllComponents()
        {
            var builder = new ContainerBuilder();

            #region Networking
            builder.Register(new NetServer(new NetConfiguration("Frenetic")));
            builder.Register(new NetClient(new NetConfiguration("Frenetic")));
            builder.Register<NetServerWrapper>().As<INetServer>().ContainerScoped();
            builder.Register<NetClientWrapper>().As<INetClient>().ContainerScoped();
            builder.Register<LidgrenServerNetworkSession>().As<IServerNetworkSession>().ContainerScoped();
            builder.Register<LidgrenClientNetworkSession>().As<IClientNetworkSession>().ContainerScoped();
            builder.Register<IncomingMessageQueue>().As<IIncomingMessageQueue>().ContainerScoped();
            builder.Register<OutgoingMessageQueue>().As<IOutgoingMessageQueue>().ContainerScoped();
            builder.Register<XmlMessageSerializer>().As<IMessageSerializer>().ContainerScoped();
            #endregion

            #region ViewFactory
            builder.Register<ViewFactory>().As<IViewFactory>().ContainerScoped();
            #endregion

            #region Graphics
            builder.Register<SpriteBatch>(new SpriteBatch(_graphicsDevice));
            //builder.Register<SpriteBatch>().FactoryScoped();
            //builder.Register<GraphicsDevice>(_graphicsDevice).SingletonScoped();
            builder.Register<XNASpriteBatch>().As<ISpriteBatch>().FactoryScoped();
            builder.Register<XNATexture>().As<ITexture>().FactoryScoped();
            #endregion

            #region GameSession
            builder.Register<GameSession>().As<IGameSession>().ContainerScoped();
            builder.Register<GameSessionController>().ContainerScoped();
            builder.Register<GameSessionView>().ContainerScoped();
            #endregion

            #region Player
            builder.Register<Player>().FactoryScoped();
            builder.RegisterGeneratedFactory<Player.Factory>(new TypedService(typeof(Player)));
            builder.Register((c) => (IBoundaryCollider)new WorldBoundaryCollider(_screenWidth, _screenHeight));
            builder.Register<KeyboardPlayerController>().ContainerScoped();
            builder.Register<NetworkPlayerView>().FactoryScoped();
            #endregion

            #region Physics
            builder.RegisterModule(new PhysicsModule() { Gravity = _gravity });
            #endregion

            #region Level
            builder.Register<LevelPiece>().FactoryScoped();
            builder.RegisterGeneratedFactory<LevelPiece.Factory>(new TypedService(typeof(LevelPiece)));
            builder.Register<DumbLevelLoader>().As<ILevelLoader>().ContainerScoped();
            builder.Register<Frenetic.Level.Level>().ContainerScoped();
            builder.Register<LevelController>().ContainerScoped();
            builder.Register<LevelView>().ContainerScoped();
            #endregion

            // RAYCASTER:
            builder.Register<DumbRayCaster>().SingletonScoped();
            builder.Register<DumbRayCasterTestController>().ContainerScoped();

            // CAMERA:
            builder.Register((c, p) => (ICamera)new Camera(p.TypedAs<IPlayer>(), new Vector2(_screenWidth, _screenHeight))).ContainerScoped();
            //builder.Register<Camera>().As<ICamera>().FactoryScoped();

            // CROSSHAIR:
            builder.Register<Crosshair>().As<ICrosshair>().ContainerScoped();
            builder.Register<CrosshairView>().ContainerScoped();

            // KEYBOARD:
            builder.Register<Keyboard>().As<IKeyboard>().SingletonScoped();

            return builder.Build();
        }

        private IPlayer CreateClientComponents(IGameSession gameSession)
        {
            ITexture playerTexture = ClientContainer.Resolve<ITexture>(new TypedParameter(typeof(Texture2D), _contentManager.Load<Texture2D>("Content/textures/ball")));
            IViewFactory viewFactory = ClientContainer.Resolve<IViewFactory>(new TypedParameter(typeof(ITexture), playerTexture));

            // Make local player:
            IPlayer localPlayer = ClientContainer.Resolve<Player>(new TypedParameter(typeof(int), 0));

            gameSession.Controllers.Add(ClientContainer.Resolve<FarseerPhysicsController>());
            Frenetic.Level.Level level = ClientContainer.Resolve<Frenetic.Level.Level>();
            gameSession.Controllers.Add(ClientContainer.Resolve<LevelController>(new TypedParameter(typeof(Frenetic.Level.Level), level)));

            gameSession.Controllers.Add(ClientContainer.Resolve<KeyboardPlayerController>(new TypedParameter(typeof(IPlayer), localPlayer)));
            gameSession.Views.Add(ClientContainer.Resolve<NetworkPlayerView>(new TypedParameter(typeof(IPlayer), localPlayer)));
            ICamera camera = ClientContainer.Resolve<ICamera>
                                            (
                                            new TypedParameter(typeof(IPlayer), localPlayer),
                                            new TypedParameter(typeof(Vector2), new Vector2(_screenWidth, _screenHeight))
                                            );

            ITexture levelTexture = ClientContainer.Resolve<ITexture>(new TypedParameter(typeof(Texture2D), _contentManager.Load<Texture2D>("Content/Textures/blank")));
            gameSession.Views.Add(ClientContainer.Resolve<LevelView>(new TypedParameter(typeof(ITexture), levelTexture)));

            ITexture crosshairTexture = ClientContainer.Resolve<ITexture>(new TypedParameter(typeof(Texture2D), _contentManager.Load<Texture2D>("Content/Textures/cursor")));
            gameSession.Views.Add(ClientContainer.Resolve<CrosshairView>(new TypedParameter(typeof(ITexture), crosshairTexture)));

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

        #endregion

        GraphicsDevice _graphicsDevice;
        ContentManager _contentManager;
        IContainer ClientContainer { get; set; }
        IContainer ServerContainer { get; set; }
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
