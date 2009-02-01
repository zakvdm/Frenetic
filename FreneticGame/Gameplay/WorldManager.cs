using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Input;

namespace Frenetic
{
    public class WorldManager : IDisposable
    {
        #region Public Constants
        /// <summary>
        /// The maximum number of players in the game.
        /// </summary>
        public const int MaximumPlayers = 16;

        /// <summary>
        /// The different types of packets sent in the game.
        /// </summary>
        /// <remarks>Frequently used in packets to identify their type.</remarks>
        public enum PacketTypes
        {
            PlayerInput,
            GameState,
        };

        #endregion

        #region Graphics Data
        private SpriteBatch spriteBatch;
        private GraphicsDevice graphicsDevice;
        private Microsoft.Xna.Framework.Net.NetworkSession networkSession;
        #endregion

        #region Game Data
        private TileGrid level;
        private Camera camera;
        private Cursor cursor;
        private PhysicsManager physicsManager;
        #endregion

        SpriteFont tempFont;

        #region Properties
        public TileGrid Level
        {
            get { return level; }
        }
        public Vector2 CursorPosition
        {
            get { return cursor.Position; }
        }

        #endregion

        #region Initialization
        public WorldManager(GraphicsDevice pGraphicsDevice, ContentManager pContentManager, Microsoft.Xna.Framework.Net.NetworkSession networkSession)
        {
            // safety-check the parameters, as they must be valid
            if (pGraphicsDevice == null)
            {
                throw new ArgumentNullException("graphicsDevice");
            }
            if (pContentManager == null)
            {
                throw new ArgumentNullException("contentManager");
            }

            spriteBatch = new SpriteBatch(pGraphicsDevice);
            graphicsDevice = pGraphicsDevice;
            this.networkSession = networkSession;

            OldPlayer.LoadContent(pContentManager);

            tempFont = pContentManager.Load<SpriteFont>("Fonts/MenuFont");

            level = new TileGrid(pContentManager.Load<Texture2D>("Textures/blank"));
            level.Initialize();
            level.TempLoadLevel();

            camera = new Camera(null, new Vector2(pGraphicsDevice.Viewport.Width, pGraphicsDevice.Viewport.Height));

            Line.LoadContent(pContentManager);
            cursor = new Cursor(pContentManager.Load<Texture2D>("Textures/cursor"));

            physicsManager = new PhysicsManager(level);
        }
        #endregion

        #region Update Methods
        public void Update()
        {
            if (networkSession.IsHost)
            {
                // Loop over all the players in the session, not just the local ones!
                foreach (NetworkGamer gamer in networkSession.AllGamers)
                {
                    // Look up what tank is associated with this player.
                    OldPlayer player = gamer.Tag as OldPlayer;

                    // Update the player.
                    player.Update(physicsManager);
                }
            }

            foreach (LocalNetworkGamer gamer in networkSession.LocalGamers)
            {
                // should be only one for now...
                //camera.Position = (gamer.Tag as OldPlayer).Position;
            }

            // Put cursor in World Coordinates
            //cursor.Position = Vector2.Transform(new Vector2(Mouse.GetState().X, Mouse.GetState().Y), Matrix.Invert(camera.TransformMatrix));

            foreach (LocalNetworkGamer gamer in networkSession.LocalGamers)
            {
                // Move cursor with player
                cursor.Position += (gamer.Tag as OldPlayer).Position - (gamer.Tag as OldPlayer).LastPosition;
            }
        }

        public PlayerInput GetInput()
        {
            PlayerInput input = new PlayerInput();
            input.GetButtonInput();
            input.mousePosition = cursor.Position;

            return input;
        }
        #endregion

        #region Drawing Methods
        public void Draw()
        {
            //spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred,
            //    SaveStateMode.None, camera.TransformMatrix);

            cursor.Draw(spriteBatch);

            // For each person in the session...
            foreach (NetworkGamer gamer in networkSession.AllGamers)
            {
                // Look up the tank object belonging to this network gamer.
                OldPlayer player = gamer.Tag as OldPlayer;

                player.Draw(spriteBatch);

                // Draw a gamertag label.
                string label = gamer.Gamertag;
                Color labelColor = Color.Black;
                Vector2 labelOffset = new Vector2(75, 75);

                if (gamer.IsHost)
                    label += " (server)";

                // Flash the gamertag to yellow when the player is talking.
                if (gamer.IsTalking)
                    labelColor = Color.Yellow;

                spriteBatch.DrawString(tempFont, label, player.Position, labelColor, 0,
                                       labelOffset, 0.6f, SpriteEffects.None, 0);
            }

            foreach (Tile tile in level)
            {
                tile.Draw(spriteBatch);
            }
            spriteBatch.End();
        }
        #endregion

        #region IDisposable Implementation
        ~WorldManager()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (this)
                {
                    if (spriteBatch != null)
                    {
                        spriteBatch.Dispose();
                        spriteBatch = null;
                    }

                    OldPlayer.UnloadContent();
                }
            }
        }
        #endregion
    }
}
