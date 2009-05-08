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
using Frenetic.Player;

namespace Frenetic
{
    public class GameSessionFactory : IGameSessionFactory
    {
        const int _screenWidth = 800;
        const int _screenHeight = 600;

        public GameSessionFactory(GraphicsDevice graphicsDevice, ContentManager contentManager, IContainer parentContainer)
        {
            _graphicsDevice = graphicsDevice;
            _contentManager = contentManager;

            _parentContainer = parentContainer;

        }
        #region IGameSessionFactory Members

        public GameSessionControllerAndView MakeServerGameSession()
        {
            ServerContainer = _parentContainer.CreateInnerContainer();

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

            // Server needs a new PhysicsSimulator (can't use the client's... -- NOTE: the parameter value is irrelevant, all that matters is that it exists...)
            ServerContainer.Resolve<IPhysicsSimulator>(new NamedParameter("isServer", true));

            IGameSession gameSession = ServerContainer.Resolve<IGameSession>();

            GameSessionController gameSessionController = ServerContainer.Resolve<GameSessionController>();
            GameSessionView gameSessionView = ServerContainer.Resolve<GameSessionView>();

            SnapCounter snapCounter = (SnapCounter)ServerContainer.Resolve<ISnapCounter>();
            gameSession.Controllers.Add(snapCounter);

            Log<ChatMessage> serverLog = ServerContainer.Resolve<Log<ChatMessage>>();
            IChatLogDiffer chatLogDiffer = ServerContainer.Resolve<IChatLogDiffer>(new TypedParameter(typeof(Log<ChatMessage>), serverLog));

            // THINGS TO SYNC OVER NETWORK:
            gameSession.Views.Add(ServerContainer.Resolve<ChatLogSender>(new TypedParameter(typeof(Log<ChatMessage>), serverLog)));
            gameSession.Controllers.Add(ServerContainer.Resolve<ClientInputProcessor>(new TypedParameter(typeof(Log<ChatMessage>), serverLog)));
            // ****************************


            return new GameSessionControllerAndView(gameSession, gameSessionController, gameSessionView);
        }

        
        public GameSessionControllerAndView MakeClientGameSession()
        {
            ClientContainer = _parentContainer.CreateInnerContainer();
            
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

            SnapCounter snapCounter = (SnapCounter)ClientContainer.Resolve<ISnapCounter>();
            gameSession.Controllers.Add(snapCounter);

            IPlayer localPlayer = CreateClientComponents(gameSession);

            gameSession.Controllers.Add(ClientContainer.Resolve<PhysicsController>());
            
            ILevel level = ClientContainer.Resolve<ILevel>();
            gameSession.Controllers.Add(ClientContainer.Resolve<LevelController>(new TypedParameter(typeof(ILevel), level)));

            GameSessionController gameSessionController = ClientContainer.Resolve<GameSessionController>(
                new TypedParameter(typeof(bool), false));
            GameSessionView gameSessionView = ClientContainer.Resolve<GameSessionView>();

            
            // TEMP:
            //TestVisibilityView tmp = new TestVisibilityView(_graphicsDevice, level, localPlayer, ClientContainer.Resolve<ICamera>());
            //gameSession.Views.Insert(0, tmp);


            // THINGS TO SYNC OVER NETWORK:
            Log<ChatMessage> chatLog = _parentContainer.Resolve<IMessageConsole>().Log;
            gameSession.Controllers.Add(ClientContainer.Resolve<ChatLogProcessor>(new TypedParameter(typeof(Log<ChatMessage>), chatLog), new TypedParameter(typeof(IIncomingMessageQueue), incomingMessageQueue)));
            gameSession.Views.Add(ClientContainer.Resolve<ClientInputSender>(new TypedParameter(typeof(Log<ChatMessage>), chatLog)));
            // ****************************


            return new GameSessionControllerAndView(gameSession, gameSessionController, gameSessionView);
        }


        private IPlayer CreateClientComponents(IGameSession gameSession)
        {
            // Make local player:
            IPlayer localPlayer = ClientContainer.Resolve<LocalClient>().Player;

            gameSession.Controllers.Add(ClientContainer.Resolve<PhysicsController>());
            ILevel level = ClientContainer.Resolve<ILevel>();
            gameSession.Controllers.Add(ClientContainer.Resolve<LevelController>(new TypedParameter(typeof(ILevel), level)));

            ICamera camera = ClientContainer.Resolve<ICamera>
                                            (
                                            new TypedParameter(typeof(IPlayer), localPlayer),
                                            new TypedParameter(typeof(Vector2), new Vector2(_screenWidth, _screenHeight))
                                            );

            PlayerView localPlayerView = ClientContainer.Resolve<PlayerView>(new TypedParameter(typeof(IPlayer), localPlayer), new TypedParameter(typeof(IPlayerSettings), ClientContainer.Resolve<LocalPlayerSettings>()));
            gameSession.Views.Add(localPlayerView);

            gameSession.Controllers.Add(ClientContainer.Resolve<KeyboardPlayerController>(new TypedParameter(typeof(IPlayer), localPlayer)));

            gameSession.Views.Add(ClientContainer.Resolve<VisibilityView>(new TypedParameter(typeof(IPlayer), localPlayer)));

            ITexture levelTexture = ClientContainer.Resolve<ITexture>(new TypedParameter(typeof(Texture2D), _contentManager.Load<Texture2D>("Textures/blank")));
            gameSession.Views.Add(ClientContainer.Resolve<LevelView>(new TypedParameter(typeof(ITexture), levelTexture)));

            ITexture crosshairTexture = ClientContainer.Resolve<ITexture>(new TypedParameter(typeof(Texture2D), _contentManager.Load<Texture2D>("Textures/cursor")));
            gameSession.Views.Add(ClientContainer.Resolve<CrosshairView>(new TypedParameter(typeof(ITexture), crosshairTexture)));

            // TEMP CODE:
            // *********************************************************************************
            //gameSession.Controllers.Add(ClientContainer.Resolve<DumbRayCasterTestController>());

            // DEBUG VIEW:  // TODO: Write a controller for this...
            // TODO: Instantiate with Autofac
            IPhysicsSimulator physicsSimulator = ClientContainer.Resolve<IPhysicsSimulator>();
            var debugView = new PhysicsSimulatorView(physicsSimulator.PhysicsSimulator, new SpriteBatch(_graphicsDevice), camera);
            physicsSimulator.PhysicsSimulator.EnableDiagnostics = true;
            debugView.LoadContent(_graphicsDevice, _contentManager);
            gameSession.Views.Add(debugView);
            // *********************************************************************************

            return localPlayer;
        }

        public void Dispose()
        {
            if (ClientContainer != null)
            {
                ClientContainer.Dispose();
            }
            if (ServerContainer != null)
            {
                ServerContainer.Dispose();
            }
        }

        #endregion

        GraphicsDevice _graphicsDevice;
        ContentManager _contentManager;
        IContainer _parentContainer;
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
