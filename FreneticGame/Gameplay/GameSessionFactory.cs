using System;
using Lidgren.Network;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Autofac.Builder;
using Autofac;
using Frenetic.Physics;
using Frenetic.Gameplay.Level;
using FarseerGames.FarseerPhysics;
using FarseerGames.FarseerPhysics.Factories;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Collisions;
using Frenetic.Graphics;
using Microsoft.Xna.Framework;
using FarseerGames.AdvancedSamplesXNA;
using Frenetic.Network.Lidgren;
using Frenetic.Network;
using Frenetic.Autofac;
using Frenetic.UserInput;
using Frenetic.Player;
using Frenetic.Gameplay.Weapons;
using Frenetic.Engine;
using Frenetic.Gameplay.HUD;
using log4net;

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
            ServerContainer.Resolve<IClientStateTracker>(new TypedParameter(typeof(INetworkSession), serverNetworkSession),
                                                         new TypedParameter(typeof(IClientFactory), ServerContainer.Resolve<ServerSideClientFactory>()));

            IIncomingMessageQueue incomingMessageQueue = ServerContainer.Resolve<IIncomingMessageQueue>(
                    new TypedParameter(typeof(INetworkSession), serverNetworkSession));
            IOutgoingMessageQueue outgoingMessageQueue = ServerContainer.Resolve<IOutgoingMessageQueue>(
                                        new TypedParameter(typeof(INetworkSession), serverNetworkSession));
            serverNetworkSession.Create(14242);
            // *****************************************

            // Server needs a new PhysicsSimulator (can't use the client's... -- NOTE: the parameter value is irrelevant, all that matters is that it exists...)
            var simulator = ServerContainer.Resolve<IPhysicsSimulator>(new NamedParameter("isServer", true));

            IGameSession gameSession = ServerContainer.Resolve<IGameSession>();

            GameSessionController gameSessionController = ServerContainer.Resolve<GameSessionController>();
            GameSessionView gameSessionView = ServerContainer.Resolve<GameSessionView>();

            gameSession.Controllers.Add(ServerContainer.Resolve<PlayerUpdater>());

            SnapCounter snapCounter = (SnapCounter)ServerContainer.Resolve<ISnapCounter>();
            gameSession.Controllers.Add(snapCounter);
            gameSession.Controllers.Add(ServerContainer.Resolve<TimerController>());

            Log<ChatMessage> serverLog = ServerContainer.Resolve<Log<ChatMessage>>();

            // THINGS TO SYNC OVER NETWORK:
            gameSession.Views.Add(ServerContainer.Resolve<GameStateSender>(new TypedParameter(typeof(Log<ChatMessage>), serverLog)));
            gameSession.Controllers.Add(ServerContainer.Resolve<ClientInputProcessor>(new TypedParameter(typeof(Log<ChatMessage>), serverLog)));
            // ****************************

            return new GameSessionControllerAndView(gameSession, gameSessionController, gameSessionView);
        }

        
        public GameSessionControllerAndView MakeClientGameSession(string address, int port)
        {
            ClientContainer = _parentContainer.CreateInnerContainer();
            
            // TODO: Move this somewhere more appropriate
            // Make queues for a CLIENT network session:
            // *****************************************
            IClientNetworkSession clientNetworkSession = ClientContainer.Resolve<IClientNetworkSession>();
            // *****************************************

            IGameSession gameSession = ClientContainer.Resolve<IGameSession>();

            SnapCounter snapCounter = (SnapCounter)ClientContainer.Resolve<ISnapCounter>();
            gameSession.Controllers.Add(snapCounter);

            IPlayer localPlayer = CreateClientComponents(gameSession);


            ClientContainer.Resolve<IClientStateTracker>(new TypedParameter(typeof(INetworkSession), clientNetworkSession),
                                                         new TypedParameter(typeof(IClientFactory), ClientContainer.Resolve<ClientSideClientFactory>()));
            IIncomingMessageQueue incomingMessageQueue = ClientContainer.Resolve<IIncomingMessageQueue>(
                    new TypedParameter(typeof(INetworkSession), clientNetworkSession));
            IOutgoingMessageQueue outgoingMessageQueue = ClientContainer.Resolve<IOutgoingMessageQueue>(
                    new TypedParameter(typeof(INetworkSession), clientNetworkSession));
            if (address != "")
            {
                clientNetworkSession.Join(address, port);
            }
            else
            {
                clientNetworkSession.Join(port);
            }

            gameSession.Controllers.Add(ClientContainer.Resolve<PhysicsController>());
            
            ILevel level = ClientContainer.Resolve<ILevel>();
            gameSession.Controllers.Add(ClientContainer.Resolve<LevelController>(new TypedParameter(typeof(ILevel), level)));

            gameSession.Views.Add(ClientContainer.Resolve<HudOverlaySetView>());
            gameSession.Controllers.Add(ClientContainer.Resolve<HudController>());

            GameSessionController gameSessionController = ClientContainer.Resolve<GameSessionController>(
                new TypedParameter(typeof(bool), false));
            GameSessionView gameSessionView = ClientContainer.Resolve<GameSessionView>();

            gameSession.Controllers.Add(ClientContainer.Resolve<TimerController>());

            // THINGS TO SYNC OVER NETWORK:
            Log<ChatMessage> chatLog = _parentContainer.Resolve<IMessageConsole>().Log;
            Log<ChatMessage> pendingLog = _parentContainer.Resolve<IMessageConsole>().PendingLog;
            gameSession.Controllers.Add(ClientContainer.Resolve<GameStateProcessor>(new TypedParameter(typeof(Log<ChatMessage>), chatLog), new TypedParameter(typeof(IIncomingMessageQueue), incomingMessageQueue)));
            gameSession.Views.Add(ClientContainer.Resolve<ClientInputSender>(new TypedParameter(typeof(Log<ChatMessage>), pendingLog)));
            // ****************************

            return new GameSessionControllerAndView(gameSession, gameSessionController, gameSessionView);
        }


        private IPlayer CreateClientComponents(IGameSession gameSession)
        {
            // Make local player:
            IPlayer localPlayer = ClientContainer.Resolve<LocalPlayer>();
            var localClient = ClientContainer.Resolve<LocalClient>(new TypedParameter(typeof(IPlayer), localPlayer));

            gameSession.Controllers.Add(ClientContainer.Resolve<PhysicsController>());
            ILevel level = ClientContainer.Resolve<ILevel>();
            gameSession.Controllers.Add(ClientContainer.Resolve<LevelController>(new TypedParameter(typeof(ILevel), level)));

            ICamera camera = ClientContainer.Resolve<ICamera>
                                            (
                                            new TypedParameter(typeof(IPlayer), localPlayer),
                                            new TypedParameter(typeof(Vector2), new Vector2(_screenWidth, _screenHeight))
                                            );

            gameSession.Controllers.Add(ClientContainer.Resolve<IPlayerController>(new TypedParameter(typeof(IPlayer), localPlayer)));

            gameSession.Controllers.Add(ClientContainer.Resolve<EffectUpdater>());

            gameSession.Views.Add(ClientContainer.Resolve<PlayerView>()); // PlayerView must draw before VisibilityView (so players are underneath blackness!)
            gameSession.Views.Add(ClientContainer.Resolve<VisibilityView>(new TypedParameter(typeof(IPlayer), localPlayer)));

            ITexture levelTexture = ClientContainer.Resolve<ITexture>(new TypedParameter(typeof(Texture2D), _contentManager.Load<Texture2D>("Textures/blank")));
            gameSession.Views.Add(ClientContainer.Resolve<LevelView>(new TypedParameter(typeof(ITexture), levelTexture)));

            ITexture crosshairTexture = ClientContainer.Resolve<ITexture>(new TypedParameter(typeof(Texture2D), _contentManager.Load<Texture2D>("Textures/cursor")));
            gameSession.Views.Add(ClientContainer.Resolve<CrosshairView>(new TypedParameter(typeof(ITexture), crosshairTexture)));


            // TEMP CODE:
            // *********************************************************************************
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
