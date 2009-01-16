using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Input;

namespace Frenetic
{
    public class GameplayScreen : GameScreen, IDisposable
    {
        
        #region Networking Data
        
        //private Server server;
        //private Client client;

        private NetworkManager networkManager;

        /// <summary>
        /// The network session for this game.
        /// </summary>
        private Microsoft.Xna.Framework.Net.NetworkSession networkSession;

        /// <summary>
        /// The packet writer used to send data from this screen.
        /// </summary>
        private PacketWriter mPacketWriter = new PacketWriter();

        /// <summary>
        /// Event handler for the session-ended event.
        /// </summary>
        EventHandler<NetworkSessionEndedEventArgs> sessionEndedHandler;

        /// <summary>
        /// Event handler for the game-ended event.
        /// </summary>
        EventHandler<GameStartedEventArgs> gameStartedHandler;

        /// <summary>
        /// Event handler for the gamer-left event.
        /// </summary>
        EventHandler<Microsoft.Xna.Framework.Net.GamerJoinedEventArgs> gamerJoinedHandler;

        /// <summary>
        /// Event handler for the game-ended event.
        /// </summary>
        EventHandler<GameEndedEventArgs> gameEndedHandler;

        /// <summary>
        /// Event handler for the gamer-left event.
        /// </summary>
        EventHandler<GamerLeftEventArgs> gamerLeftHandler;


        #endregion

        #region Gameplay Data
        WorldManager worldManager;
        #endregion

        public GameplayScreen(GameSessionControllerAndView clientGameSessionCandV)
        {
            _clientGameSessionController = clientGameSessionCandV.GameSessionController;
            _clientGameSessionView = clientGameSessionCandV.GameSessionView;

            // set the transition times
            TransitionOnTime = TimeSpan.FromSeconds(1.0);
            TransitionOffTime = TimeSpan.FromSeconds(1.0);
        }
        public GameplayScreen(GameSessionControllerAndView serverGameSessionCandV, GameSessionControllerAndView clientGameSessionCandV)
        {
            _serverGameSessionController = serverGameSessionCandV.GameSessionController;
            _clientGameSessionController = clientGameSessionCandV.GameSessionController;

            _serverGameSessionView = serverGameSessionCandV.GameSessionView;
            _clientGameSessionView = clientGameSessionCandV.GameSessionView;

            // set the transition times
            TransitionOnTime = TimeSpan.FromSeconds(1.0);
            TransitionOffTime = TimeSpan.FromSeconds(1.0);
        }

        public GameplayScreen(Microsoft.Xna.Framework.Net.NetworkSession pNetworkSession)
        {
            // TO BE DELETED:

            // safety-check the parameter
            if (pNetworkSession == null)
            {
                throw new ArgumentNullException("networkSession");
            }

            this.networkSession = pNetworkSession;

            // set the transition times
            TransitionOnTime = TimeSpan.FromSeconds(1.0);
            TransitionOffTime = TimeSpan.FromSeconds(1.0);

            // set up the network events
            gamerJoinedHandler = new EventHandler<Microsoft.Xna.Framework.Net.GamerJoinedEventArgs>(
                networkSession_GamerJoined);
            gameStartedHandler = new EventHandler<GameStartedEventArgs>(
                networkSession_GameStarted);
            sessionEndedHandler = new EventHandler<NetworkSessionEndedEventArgs>(
                networkSession_SessionEnded);

            gameEndedHandler = new EventHandler<GameEndedEventArgs>(
                networkSession_GameEnded);
            gamerLeftHandler = new EventHandler<GamerLeftEventArgs>(
                networkSession_GamerLeft);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            //server = new Server(networkSession);
            //client = new Client(networkSession);

            //networkManager = new NetworkManager(networkSession);

            //worldManager = new WorldManager(ScreenManager.GraphicsDevice, ScreenManager.Content, networkSession);
            //server.LoadWorld(worldManager);

            /*
            networkSession.GamerJoined += gamerJoinedHandler;
            networkSession.GameStarted += gameStartedHandler;
            networkSession.SessionEnded += sessionEndedHandler;

            networkSession.GameEnded += gameEndedHandler;
            networkSession.GamerLeft += gamerLeftHandler;
            */
        }

        public override void Update(GameTime pGameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(pGameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (_serverGameSessionController != null)
                _serverGameSessionController.Process(pGameTime.ElapsedGameTime.Ticks);

            if (_clientGameSessionController != null)
                _clientGameSessionController.Process(pGameTime.ElapsedGameTime.Ticks);
            /*
            // if something else has canceled our game, then exit
            if ((networkSession == null) || (worldManager == null))
            {
                if (!IsExiting)
                {
                    ExitScreen();
                }
                base.Update(pGameTime, otherScreenHasFocus, coveredByOtherScreen);
                return;
            }

            networkManager.ReadIncomingPackets();

            worldManager.Update();
            //client.ProcessInput();
            //server.UpdateServer(pGameTime);

            networkManager.SendOutgoingPackets(worldManager.GetInput());
            
            
            // update the network session
            try
            {
                networkSession.Update();
            }
            catch (NetworkException ne)
            {
                System.Console.WriteLine(
                    "Network failed to update:  " + ne.ToString());
                if (networkSession != null)
                {
                    networkSession.Dispose();
                    networkSession = null;
                }
            }

            //server.ReadInputFromClients();
            //client.ReadGameStateFromServer();
            */
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(IInputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (!IsExiting)
            {
                if (worldManager != null)
                {
                    if (input.PauseGame)
                    {
                        // If they pressed pause, bring up the pause menu screen.
                        const string message = "Exit the game?";
                        MessageBoxScreen messageBox = new MessageBoxScreen(message,
                            false);
                        messageBox.Accepted += ExitMessageBoxAccepted;
                        ScreenManager.AddScreen(messageBox);
                    }
                    /*
                    if (input.MenuSelect)
                    {
                        mMap = null;
                        if (!IsExiting)
                        {
                            ExitScreen();
                        }
                        mNetworkSession = null;
                    }
                     */
                }
            }

            base.HandleInput(input);
        }

        /// <summary>
        /// Event handler for when the user selects "yes" on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        private void ExitMessageBoxAccepted(object sender, EventArgs e)
        {
            worldManager = null;
        }

        /// <summary>
        /// Exit this screen.
        /// </summary>
        public override void ExitScreen()
        {
            if (!IsExiting && (networkSession != null))
            {
                networkSession.SessionEnded -= sessionEndedHandler;
                networkSession.GameEnded -= gameEndedHandler;
                networkSession.GamerLeft -= gamerLeftHandler;
            }
            base.ExitScreen();
        }

        #region Drawing Methods
        public override void Draw(GameTime gameTime)
        {
            if (_serverGameSessionView != null)
                _serverGameSessionView.Generate();

            _clientGameSessionView.Generate();

            /*
            if (networkSession != null)
            {
                if ((worldManager != null) && !IsExiting)
                {
                    worldManager.Draw();
                }
            }
            */
        }
        #endregion


        #region Networking Event Handlers



        /// <summary>
        /// Handle the end of the game session.
        /// </summary>
        void networkSession_GameEnded(object sender, GameEndedEventArgs e)
        {
            if (!IsExiting && (worldManager == null))
            {
                ExitScreen();
                networkSession = null;
            }
        }


        /// <summary>
        /// Handle the end of the network session.
        /// </summary>
        void networkSession_SessionEnded(object sender, NetworkSessionEndedEventArgs e)
        {
            if (!IsExiting)
            {
                ExitScreen();
            }
            if (worldManager != null)
            {
                worldManager.Dispose();
                worldManager = null;
            }
            if (networkSession != null)
            {
                networkSession.Dispose();
                networkSession = null;
            }
        }


        /// <summary>
        /// Handle the start of the game session.
        /// </summary>
        void networkSession_GameStarted(object sender, GameStartedEventArgs e)
        {
            // if we're the host, generate the data
            if ((networkSession != null) && networkSession.IsHost && (worldManager != null))
            {
                //world.GenerateWorld();
            }
        }


        /// <summary>
        /// Handle a new player joining the session.
        /// </summary>
        void networkSession_GamerJoined(object sender, Microsoft.Xna.Framework.Net.GamerJoinedEventArgs e)
        {
            int gamerIndex = networkSession.AllGamers.IndexOf(e.Gamer);

            e.Gamer.Tag = new OldPlayer();

            // make sure the data exists for the new gamer
            for (int i = 0; i < networkSession.AllGamers.Count; i++)
            {
                if (networkSession.AllGamers[i] == e.Gamer)
                {
                    /*
                    PlayerData playerData = new PlayerData();
                    e.Gamer.Tag = playerData;
                    playerData.ShipVariation = (byte)(i % 4);
                    playerData.ShipColor = (byte)i;
                     */
                }
            }

            // send my own data to the new gamer
            if ((networkSession.LocalGamers.Count > 0) && !e.Gamer.IsLocal)
            {
                /*
                PlayerData playerData = networkSession.LocalGamers[0].Tag as PlayerData;
                if (playerData != null)
                {
                    packetWriter.Write((int)World.PacketTypes.PlayerData);
                    playerData.Serialize(packetWriter);
                    networkSession.LocalGamers[0].SendData(packetWriter,
                        SendDataOptions.ReliableInOrder, e.Gamer);
                }
                 */
            }
        }

        /// <summary>
        /// Handle a player leaving the game.
        /// </summary>
        void networkSession_GamerLeft(object sender, GamerLeftEventArgs e)
        {
            /*
            PlayerData playerData = e.Gamer.Tag as PlayerData;
            if ((playerData != null) && (playerData.Ship != null))
            {
                playerData.Ship.Die(null, true);
            }
             */
        }


        #endregion


        
        #region IDisposable Implementation


        /// <summary>
        /// Finalizes the GameplayScreen object, calls Dispose(false)
        /// </summary>
        ~GameplayScreen()
        {
            Dispose(false);
        }


        /// <summary>
        /// Disposes the GameplayScreen object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// Disposes this object.
        /// </summary>
        /// <param name="disposing">
        /// True if this method was called as part of the Dispose method.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (this)
                {
                }
            }
        }


        #endregion

        private IController _clientGameSessionController;
        private IController _serverGameSessionController;
        private IView _clientGameSessionView;
        private IView _serverGameSessionView;
    }
}
