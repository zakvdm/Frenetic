using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.GamerServices;

namespace Frenetic
{
    public class MainMenuScreen : MenuScreen
    {
        #region State Data

        public enum MainMenuState
        {
            Empty,
            Network,
        }

        /// <summary>
        /// The current state of the main menu.
        /// </summary>
        MainMenuState state = MainMenuState.Empty;
        public MainMenuState State
        {
            get { return state; }
            set
            {
                // exit early from trivial sets
                if (state == value)
                {
                    return;
                }
                state = value;
                if (MenuEntries != null)
                {
                    switch (state)
                    {
                        case MainMenuState.Network:
                            {
                                MenuEntries.Clear();
                                MenuEntries.Add("Create Network Session");
                                MenuEntries.Add("Join Network Session");
                                MenuEntries.Add("Exit");
                                break;
                            }
                    }
                }
            }
        }
        #endregion

        private IGameSessionFactory _gameSessionFactory;
        private IScreenFactory _screenFactory;
        private Game _game;

        #region Initialization

        /// <summary>
        /// Constructs a new MainMenu object.
        /// </summary>
        public MainMenuScreen(Viewport viewport, SpriteBatch spriteBatch, SpriteFont font, IGameSessionFactory gameSessionFactory, IScreenFactory screenFactory, Game game)
            : base(viewport, spriteBatch, font)
        {
            // TODO: There must be a way to reduce the number of parameters here???
            _gameSessionFactory = gameSessionFactory;
            _screenFactory = screenFactory;
            _game = game;

            // set the transition times
            TransitionOnTime = TimeSpan.FromSeconds(1.0);
            TransitionOffTime = TimeSpan.FromSeconds(0.0);
        }


        #endregion

        #region Updating Methods


        /// <summary>
        /// Updates the screen. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
            bool coveredByOtherScreen)
        {
            State = MainMenuState.Network;

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.B))
                GC.Collect();
        }

        /// <summary>
        /// Responds to user menu selections.
        /// </summary>
        public override void OnSelectEntry(int entryIndex)
        {
            switch (state)
            {
                case MainMenuState.Network:
                    {
                        switch (entryIndex)
                        {
                            case 0: // Create System Link Session
                                CreateSession();
                                break;

                            case 1: // Join System Link Session
                                //FindSession(NetworkSessionType.SystemLink);
                                JoinSession();
                                break;

                            case 2: // Exit
                                OnCancel();
                                break;
                        }
                        break;
                    }
            }
        }


        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        public override void OnCancel()
        {
            const string message = "Exit Frenetic?";
            MessageBoxScreen messageBox = _screenFactory.MakeMessageBoxScreen(message);
            // TODO: Only using the reference to Game in this one method... this needs refactoring...
            messageBox.Accepted += (sender, e) => _game.Exit();
        }

        #endregion

        #region Networking Methods

        /// <summary>
        /// Start creating a session of the given type.
        /// </summary>
        /// <param name="sessionType">The type of session to create.</param>
        void CreateSession()
        {
            var serverGameSessionCandV = _gameSessionFactory.MakeServerGameSession();
            var clientGameSessionCandV = _gameSessionFactory.MakeClientGameSession();

            _screenFactory.MakeGameplayScreen(clientGameSessionCandV, serverGameSessionCandV);

            #region Old Network Code
            /*
            try
            {
                IAsyncResult asyncResult = NetworkSession.BeginCreate(sessionType, 1,
                    WorldManager.MaximumPlayers, null, null);

                // create the busy screen
                NetworkBusyScreen busyScreen = new NetworkBusyScreen(
                    "Creating a session...", asyncResult);
                busyScreen.OperationCompleted += SessionCreated;
                ScreenManager.AddScreen(busyScreen);
            }
            catch (NetworkException ne)
            {
                const string message = "Failed creating the session.";
                MessageBoxScreen messageBox = new MessageBoxScreen(message);
                messageBox.Accepted += FailedMessageBox;
                messageBox.Cancelled += FailedMessageBox;
                ScreenManager.AddScreen(messageBox);

                System.Console.WriteLine("Failed to create session:  " +
                    ne.Message);
            }
            catch (GamerPrivilegeException gpe)
            {
                const string message =
                    "You do not have permission to create a session.";
                MessageBoxScreen messageBox = new MessageBoxScreen(message);
                messageBox.Accepted += FailedMessageBox;
                messageBox.Cancelled += FailedMessageBox;
                ScreenManager.AddScreen(messageBox);

                System.Console.WriteLine(
                    "Insufficient privilege to create session:  " + gpe.Message);
            }
             * 
             * 
             * 
             * 
             * 
             * 
             * 
        /// <summary>
        /// Callback when a session is created.
        /// </summary>
        void SessionCreated(object sender, OperationCompletedEventArgs e)
        {
            NetworkSession networkSession = null;
            try
            {
                networkSession = NetworkSession.EndCreate(e.AsyncResult);
            }
            catch (NetworkException ne)
            {
                const string message = "Failed creating the session.";
                MessageBoxScreen messageBox = new MessageBoxScreen(message);
                messageBox.Accepted += FailedMessageBox;
                messageBox.Cancelled += FailedMessageBox;
                ScreenManager.AddScreen(messageBox);

                System.Console.WriteLine("Failed to create session:  " +
                    ne.Message);
            }
            catch (GamerPrivilegeException gpe)
            {
                const string message =
                    "You do not have permission to create a session.";
                MessageBoxScreen messageBox = new MessageBoxScreen(message);
                messageBox.Accepted += FailedMessageBox;
                messageBox.Cancelled += FailedMessageBox;
                ScreenManager.AddScreen(messageBox);

                System.Console.WriteLine(
                    "Insufficient privilege to create session:  " + gpe.Message);
            }
            if (networkSession != null)
            {
                networkSession.AllowHostMigration = true;
                networkSession.AllowJoinInProgress = false;
                LoadGameplayScreen(networkSession);
            }
        }
             * 
             * 
            */
            #endregion
        }

        void JoinSession()
        {
            var clientGameSessionCandV = _gameSessionFactory.MakeClientGameSession();

            _screenFactory.MakeGameplayScreen(clientGameSessionCandV, null);
        }

        /// <summary>
        /// Start searching for a session of the given type.
        /// </summary>
        /// <param name="sessionType">The type of session to look for.</param>
        void FindSession(NetworkSessionType sessionType)
        {
            throw new System.NotImplementedException(); // TODO: delete if not gonna use
            // create the new screen
            /*
            SearchResultsScreen searchResultsScreen =
               new SearchResultsScreen(sessionType);
            searchResultsScreen.ScreenManager = this.ScreenManager;
            ScreenManager.AddScreen(searchResultsScreen);

            // start the search
            try
            {
                IAsyncResult asyncResult = Microsoft.Xna.Framework.Net.NetworkSession.BeginFind(sessionType, 1, null,
                    null, null);

                // create the busy screen
                NetworkBusyScreen busyScreen = new NetworkBusyScreen(
                    "Searching for a session...", asyncResult);
                busyScreen.OperationCompleted += searchResultsScreen.SessionsFound;
                ScreenManager.AddScreen(busyScreen);
            }
            catch (NetworkException ne)
            {
                const string message = "Failed searching for the session.";
                MessageBoxScreen messageBox = new MessageBoxScreen(message);
                messageBox.Accepted += FailedMessageBox;
                messageBox.Cancelled += FailedMessageBox;
                ScreenManager.AddScreen(messageBox);

                System.Console.WriteLine("Failed to search for session:  " +
                    ne.Message);
            }
            catch (GamerPrivilegeException gpe)
            {
                const string message =
                    "You do not have permission to search for a session.";
                MessageBoxScreen messageBox = new MessageBoxScreen(message);
                messageBox.Accepted += FailedMessageBox;
                messageBox.Cancelled += FailedMessageBox;
                ScreenManager.AddScreen(messageBox);

                System.Console.WriteLine(
                    "Insufficient privilege to search for session:  " + gpe.Message);
            }
             */
        }
        /// <summary>
        /// Callback when a session is quick-matched.
        /// </summary>
        void QuickMatchSessionJoined(object sender, OperationCompletedEventArgs e)
        {
            throw new System.NotImplementedException();
            /*
            Microsoft.Xna.Framework.Net.NetworkSession networkSession = null;
            try
            {
                networkSession = Microsoft.Xna.Framework.Net.NetworkSession.EndJoin(e.AsyncResult);
            }
            catch (NetworkException ne)
            {
                const string message = "Failed joining the session.";
                MessageBoxScreen messageBox = new MessageBoxScreen(message);
                messageBox.Accepted += FailedMessageBox;
                messageBox.Cancelled += FailedMessageBox;
                ScreenManager.AddScreen(messageBox);

                System.Console.WriteLine("Failed to join session:  " +
                    ne.Message);
            }
            catch (GamerPrivilegeException gpe)
            {
                const string message =
                    "You do not have permission to join a session.";
                MessageBoxScreen messageBox = new MessageBoxScreen(message);
                messageBox.Accepted += FailedMessageBox;
                messageBox.Cancelled += FailedMessageBox;
                ScreenManager.AddScreen(messageBox);

                System.Console.WriteLine(
                    "Insufficient privilege to join session:  " + gpe.Message);
            }
            if (networkSession != null)
            {
                LoadGameplayScreen(networkSession);
            }
             * */
        }

        /// <summary>
        /// Event handler for when the user selects ok on the network-operation-failed
        /// message box.
        /// </summary>
        void FailedMessageBox(object sender, EventArgs e) { }
        #endregion
    }
}
