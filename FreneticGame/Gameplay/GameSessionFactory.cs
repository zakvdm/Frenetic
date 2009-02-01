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

namespace Frenetic
{
    public class GameSessionFactory : IGameSessionFactory
    {
        public GameSessionFactory(INetworkSessionFactory networkSessionFactory, GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            _networkSessionFactory = networkSessionFactory;
            _graphicsDevice = graphicsDevice;
            _contentManager = contentManager;
        }
        #region IGameSessionFactory Members

        public GameSessionControllerAndView MakeServerGameSession()
        {
            SpriteBatch spriteBatch = new SpriteBatch(_graphicsDevice);
            Texture2D playerTexture = _contentManager.Load<Texture2D>("Content/Textures/ball");
            _viewFactory = new ViewFactory(spriteBatch, playerTexture);
            _networkSession = _networkSessionFactory.MakeServerNetworkSession();
            _messageQueue = new MessageQueue(_networkSession);
            _gameSession = new GameSession();
            IPlayer localPlayer;
            ICamera camera;
            Player.Factory playerFactory = MakePlayerFactoryWithContainer(_gameSession, _networkSession, false, out localPlayer, out camera);
            _gameSessionController = new GameSessionController(_gameSession, _messageQueue, _networkSession, _viewFactory, playerFactory, localPlayer, camera);
            var gameSessionView = new GameSessionView(_gameSession);

            return new GameSessionControllerAndView(_gameSession, _gameSessionController, gameSessionView);
        }

        
        public GameSessionControllerAndView MakeClientGameSession()
        {
            SpriteBatch spriteBatch = new SpriteBatch(_graphicsDevice);
            Texture2D playerTexture = _contentManager.Load<Texture2D>("Content/Textures/ball");
            _viewFactory = new ViewFactory(spriteBatch, playerTexture);
            _networkSession = _networkSessionFactory.MakeClientNetworkSession();
            _messageQueue = new MessageQueue(_networkSession);
            _gameSession = new GameSession();
            IPlayer localPlayer;
            ICamera camera;
            Player.Factory playerFactory = MakePlayerFactoryWithContainer(_gameSession, _networkSession, true, out localPlayer, out camera);
            _gameSessionController = new GameSessionController(_gameSession, _messageQueue, _networkSession, _viewFactory, playerFactory, localPlayer, camera);
            var gameSessionView = new GameSessionView(_gameSession);

            return new GameSessionControllerAndView(_gameSession, _gameSessionController, gameSessionView);
        }

        private Player.Factory MakePlayerFactoryWithContainer(IGameSession gameSession, INetworkSession networkSession, bool IsClient, out IPlayer localPlayer, out ICamera camera)
        {
            int screenWidth = 800;
            int screenHeight = 600;

            var builder = new ContainerBuilder();

            // PLAYER:
            builder.Register<Player>().FactoryScoped();
            builder.RegisterGeneratedFactory<Player.Factory>(new TypedService(typeof(Player)));
            builder.Register((c) => (IBoundaryCollider)new WorldBoundaryCollider(screenWidth, screenHeight));
            builder.Register<KeyboardPlayerController>().SingletonScoped();
            builder.Register<NetworkPlayerView>().SingletonScoped();
            
            // PHYSICS:
            PhysicsSimulator physicsSimulator = new PhysicsSimulator(new Vector2(0f, 1f));
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

            // LEVEL:
            builder.Register<LevelPiece>().FactoryScoped();
            builder.RegisterGeneratedFactory<LevelPiece.Factory>(new TypedService(typeof(LevelPiece)));
            builder.Register<DumbLevelLoader>().As<ILevelLoader>().SingletonScoped();
            builder.Register<Frenetic.Level.Level>();
            builder.Register<LevelController>();
            builder.Register<LevelView>();
            SpriteBatch realSpriteBatch = new SpriteBatch(_graphicsDevice);
            ISpriteBatch spriteBatch = new XNASpriteBatch(realSpriteBatch);
            builder.Register<ISpriteBatch>(spriteBatch);
            ITexture texture = new XNATexture(_contentManager.Load<Texture2D>("Content/Textures/blank"));
            builder.Register<ITexture>(texture);

            // RAYCASTER:
            builder.Register<DumbRayCaster>().SingletonScoped();
            builder.Register<DumbRayCasterTestController>().FactoryScoped();

            // CAMERA:
            builder.Register((c, p) => (ICamera)new Camera(p.TypedAs<IPlayer>(), new Vector2(screenWidth, screenHeight))).SingletonScoped();

            var container = builder.Build();

            gameSession.Controllers.Add(container.Resolve<FarseerPhysicsController>());
            Frenetic.Level.Level level = container.Resolve<Frenetic.Level.Level>();
            if (IsClient)
            {
                // Make local player:
                localPlayer = container.Resolve<Player>(new TypedParameter(typeof(int), 0));
                gameSession.Controllers.Add(container.Resolve<KeyboardPlayerController>(new TypedParameter(typeof(IPlayer), localPlayer)));
                gameSession.Views.Add(container.Resolve<NetworkPlayerView>(
                                new TypedParameter(typeof(IPlayer), localPlayer),
                                new TypedParameter(typeof(INetworkSession), networkSession)));
                camera = container.Resolve<ICamera>(new TypedParameter(typeof(IPlayer), localPlayer));

                gameSession.Views.Add(container.Resolve<LevelView>(new TypedParameter(typeof(Frenetic.Level.Level), level)));


                // TEMP CODE:
                // *********************************************************************************
                gameSession.Controllers.Add(container.Resolve<DumbRayCasterTestController>());


                // DEBUG VIEW:  // TODO: Write a controller for this...
                var debugView = new PhysicsSimulatorView(physicsSimulator, realSpriteBatch, camera);
                physicsSimulator.EnableDiagnostics = true;
                debugView.LoadContent(_graphicsDevice, _contentManager);
                gameSession.Views.Add(debugView);
                // *********************************************************************************

            }
            else
            {
                localPlayer = null;
                camera = null;
            }

            gameSession.Controllers.Add(container.Resolve<LevelController>(new TypedParameter(typeof(Frenetic.Level.Level), level)));

            
            return container.Resolve<Player.Factory>();
        }


        #endregion

        GraphicsDevice _graphicsDevice;
        ContentManager _contentManager;

        INetworkSessionFactory _networkSessionFactory;
        IViewFactory _viewFactory;
        INetworkSession _networkSession;
        MessageQueue _messageQueue;
        IGameSession _gameSession;
        GameSessionController _gameSessionController;

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
