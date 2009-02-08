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

            #region ViewFactory
            builder.Register<ViewFactory>().As<IViewFactory>().SingletonScoped();
            #endregion

            #region Graphics
            builder.Register<SpriteBatch>(new SpriteBatch(_graphicsDevice));
            builder.Register<XNASpriteBatch>().As<ISpriteBatch>().FactoryScoped();
            builder.Register<XNATexture>().As<ITexture>().FactoryScoped();
            #endregion

            #region GameSession
            builder.Register<GameSession>().As<IGameSession>().SingletonScoped();
            builder.Register<GameSessionController>().SingletonScoped();
            builder.Register<GameSessionView>().SingletonScoped();
            #endregion

            #region Player
            builder.Register<Player>().FactoryScoped();
            builder.RegisterGeneratedFactory<Player.Factory>(new TypedService(typeof(Player)));
            builder.Register((c) => (IBoundaryCollider)new WorldBoundaryCollider(_screenWidth, _screenHeight));
            builder.Register<KeyboardPlayerController>().SingletonScoped();
            builder.Register<NetworkPlayerView>().SingletonScoped();
            #endregion

            #region Physics
            /*
            PhysicsSimulator physicsSimulator = new PhysicsSimulator(_gravity);
            builder.Register<PhysicsSimulator>(physicsSimulator).SingletonScoped();
            // Body:
            builder.Register((c, p) => BodyFactory.Instance.CreateRectangleBody(c.Resolve<PhysicsSimulator>(), p.Named<float>("width"), p.Named<float>("height"), p.Named<float>("mass"))).FactoryScoped();
            // Geom:
            builder.Register((c, p) => GeomFactory.Instance.CreateRectangleGeom(c.Resolve<PhysicsSimulator>(), p.Named<Body>("body"), p.Named<float>("width"), p.Named<float>("height"))).FactoryScoped();
            // IPhysicsComponent:
            builder.Register((c, p) =>
            {
                var width = new NamedParameter("width", 50f);
                var height = new NamedParameter("height", 50f);
                var mass = new NamedParameter("mass", 100f);
                var bod = c.Resolve<Body>(width, height, mass);
                var body = new NamedParameter("body", bod);
                var geom = c.Resolve<Geom>(body, width, height, mass);
                return (IPhysicsComponent)new FarseerPhysicsComponent(bod, geom);
            }).FactoryScoped();
            builder.Register<FarseerPhysicsController>().SingletonScoped();
             */
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
            builder.Register<Keyboard>().As<IKeyboard>().SingletonScoped();

            return builder.Build();
        }

        private Player.Factory CreateGeneralComponents(IContainer container)
        {
            ITexture playerTexture = container.Resolve<ITexture>(new TypedParameter(typeof(Texture2D), _contentManager.Load<Texture2D>("Content/textures/ball")));
            IViewFactory viewFactory = container.Resolve<IViewFactory>(new TypedParameter(typeof(ITexture), playerTexture));
            IGameSession gameSession = container.Resolve<IGameSession>();

            gameSession.Controllers.Add(container.Resolve<FarseerPhysicsController>());
            Frenetic.Level.Level level = container.Resolve<Frenetic.Level.Level>();
            
            gameSession.Controllers.Add(container.Resolve<LevelController>(new TypedParameter(typeof(Frenetic.Level.Level), level)));

            return container.Resolve<Player.Factory>();
        }

        private IPlayer CreateClientComponents(IContainer container)
        {
            // Make local player:
            IPlayer localPlayer = container.Resolve<Player>(new TypedParameter(typeof(int), 0));

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

            // TEMP CODE:
            // *********************************************************************************
            gameSession.Controllers.Add(container.Resolve<DumbRayCasterTestController>());


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
