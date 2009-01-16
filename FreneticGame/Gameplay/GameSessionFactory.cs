using System;
using Lidgren.Network;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Autofac.Builder;
using Autofac;
using Frenetic.Physics;
using FarseerGames.FarseerPhysics;
using FarseerGames.FarseerPhysics.Factories;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Collisions;

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
            _viewFactory = new ViewFactory(_graphicsDevice, _contentManager);
            _networkSession = _networkSessionFactory.MakeServerNetworkSession();
            _messageQueue = new MessageQueue(_networkSession);
            _gameSession = new GameSession();
            Player.Factory playerFactory = MakePlayerFactoryWithContainer(_gameSession);
            _gameSessionController = new GameSessionController(_gameSession, _messageQueue, _networkSession, _viewFactory, playerFactory);
            var gameSessionView = new GameSessionView(_gameSession);

            return new GameSessionControllerAndView(_gameSession, _gameSessionController, gameSessionView);
        }

        
        public GameSessionControllerAndView MakeClientGameSession()
        {
            _viewFactory = new ViewFactory(_graphicsDevice, _contentManager);
            _networkSession = _networkSessionFactory.MakeClientNetworkSession();
            _messageQueue = new MessageQueue(_networkSession);
            _gameSession = new GameSession();
            Player.Factory playerFactory = MakePlayerFactoryWithContainer(_gameSession);
            _gameSessionController = new GameSessionController(_gameSession, _messageQueue, _networkSession, _viewFactory, playerFactory);
            var gameSessionView = new GameSessionView(_gameSession);

            return new GameSessionControllerAndView(_gameSession, _gameSessionController, gameSessionView);
        }

        private Player.Factory MakePlayerFactoryWithContainer(IGameSession gameSession)
        {
            var builder = new ContainerBuilder();

            builder.Register<Player>().FactoryScoped();
            builder.RegisterGeneratedFactory<Player.Factory>(new TypedService(typeof(Player)));
            builder.Register<PhysicsValues>().SingletonScoped();
            builder.Register<VerletIntegrator>().As<IIntegrator>().FactoryScoped();
            builder.Register<WorldBoundaryCollider>().As<IBoundaryCollider>().FactoryScoped();
            
            PhysicsSimulator physicsSimulator = new PhysicsSimulator(new FarseerGames.FarseerPhysics.Mathematics.Vector2(0f, 1f));
            builder.Register<PhysicsSimulator>(physicsSimulator).SingletonScoped();
            // Body:
            builder.Register((c, p) => BodyFactory.Instance.CreateRectangleBody(c.Resolve<PhysicsSimulator>(), p.Named<float>("width"), p.Named<float>("height"), p.Named<float>("mass"))).FactoryScoped();
            // Geom:
            builder.Register((c, p) => GeomFactory.Instance.CreateRectangleGeom(p.Named<Body>("body"), p.Named<float>("width"), p.Named<float>("height"))).FactoryScoped();

            builder.Register((c, p) =>
            {
                var width = new NamedParameter("width", 100f);
                var height = new NamedParameter("height", 200f);
                var mass = new NamedParameter("mass", 100f);
                var bod = c.Resolve<Body>(width, height, mass);
                var body = new NamedParameter("body", bod);
                var geom = c.Resolve<Geom>(body, width, height, mass);
                return (IPhysicsComponent)new FarseerPhysicsComponent(bod, geom);
            }).FactoryScoped();

            builder.Register<FarseerPhysicsController>().SingletonScoped();

            var container = builder.Build();

            gameSession.Controllers.Add(container.Resolve<FarseerPhysicsController>());
            
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
