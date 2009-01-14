using System;
using Lidgren.Network;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Autofac.Builder;
using Autofac;
using Frenetic.Physics;

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
            Player.Factory playerFactory = MakePlayerFactoryWithContainer();
            _gameSessionController = new GameSessionController(_gameSession, _messageQueue, _networkSession, _viewFactory, playerFactory);
            var gameSessionView = new GameSessionView(_gameSession);

            return new GameSessionControllerAndView(_gameSessionController, gameSessionView);
        }

        private Player.Factory MakePlayerFactoryWithContainer()
        {
            var builder = new ContainerBuilder();

            builder.Register<Player>().FactoryScoped();
            builder.RegisterGeneratedFactory<Player.Factory>(new TypedService(typeof(Player)));
            builder.Register<PhysicsValues>().SingletonScoped();
            builder.Register<VerletIntegrator>().As<IIntegrator>().FactoryScoped();
            builder.Register<WorldBoundaryCollider>().As<IBoundaryCollider>().FactoryScoped();

            var container = builder.Build();

            return container.Resolve<Player.Factory>();
        }

        public GameSessionControllerAndView MakeClientGameSession()
        {
            _viewFactory = new ViewFactory(_graphicsDevice, _contentManager);
            _networkSession = _networkSessionFactory.MakeClientNetworkSession();
            _messageQueue = new MessageQueue(_networkSession);
            _gameSession = new GameSession();
            Player.Factory playerFactory = MakePlayerFactoryWithContainer();
            _gameSessionController = new GameSessionController(_gameSession, _messageQueue, _networkSession, _viewFactory, playerFactory);
            var gameSessionView = new GameSessionView(_gameSession);

            return new GameSessionControllerAndView(_gameSessionController, gameSessionView);
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
        public GameSessionControllerAndView(IController controller, IView view)
        {
            GameSessionController = controller;
            GameSessionView = view;
        }

        public IController GameSessionController { get; private set; }
        public IView GameSessionView { get; private set; }
    }
    
}
