using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Frenetic
{
    public class OldPlayer : GameplayObject
    {
        #region Constants
        const string playerTexturePath = "Textures/ball";
        const int radius = 10;
        #endregion


        private Vector2 lastPosition = new Vector2(100, 100);
        private Color color = Color.White;

        private Weapon weapon;

        private Tile tile;
        
        #region Input Data
        /// <summary>
        /// The current player input.
        /// </summary>
        private PlayerInput playerInput;
        public PlayerInput PlayerInput
        {
            get { return playerInput; }
            set { playerInput = value; }
        }
        #endregion

        private static Texture2D playerTexture;


        #region Properties
        public Vector2 LastPosition
        {
            get { return lastPosition; }
            set { lastPosition = value; }
        }

        public override Vector2 Position
        {
            get { return position; }
            set
            {
                if (tile != null)
                {
                    // Move player into appropriate tile grid
                    tile.GameplayObjects.Remove(this);
                    tile.Grid.GetTile(value).GameplayObjects.Add(this);
                    tile = tile.Grid.GetTile(value);
                }
                position = value;
            }
        }

        public Weapon Weapon
        {
            get { return weapon; }
            //set { weapon = value; }
        }

        
        public int Radius
        {
            get { return radius; }
        }
        
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        private int life;
        public int Life
        {
            get { return life; }
            set { life = value; }
        }
        private GameplayObject lastDamagedBy;
        public GameplayObject LastDamagedBy
        {
            get { return lastDamagedBy; }
        }
        #endregion

        #region Static Graphics Methods
        public static void LoadContent(ContentManager pContentManager)
        {
            // safety-check the parameters
            if (pContentManager == null)
            {
                throw new ArgumentNullException("contentManager");
            }

            playerTexture = pContentManager.Load<Texture2D>(playerTexturePath);
        }
        public static void UnloadContent()
        {
            playerTexture = null;
        }
        #endregion

        public OldPlayer()
            : base()
        {
            base.Position = new Vector2(100, 100);

            width = 20;
            height = 20;

            weapon = new RailGun();
        }

        public void HookUpPhysics(TileGrid grid)
        {
            tile = grid.GetTile(position);
            tile.GameplayObjects.Add(this);
        }

        public void Update(PhysicsManager physicsManager)
        {
            // ALL RUN BY THE SERVER!
            TickPhysics(physicsManager);

            // TODO:  Proper movement validation on the server...
            if (color == Color.Black)
                position += playerInput.movementVector;
            else
            {
                playerInput.movementVector.Y = 0.0f;
                position += playerInput.movementVector / 5.0f;
            }

            if (playerInput.fired)
            {
                weapon.Fire(Position, playerInput.mousePosition, physicsManager);
                //weapon.Fired = true;
                //weapon.Position = CurrentPosition;
                //weapon.Fire(playerInput.mousePosition - this.CurrentPosition, physicsManager);
            }
        }

        private void TickPhysics(PhysicsManager physicsManager)
        {
            physicsManager.IntegrateVerlet(this);

            physicsManager.CollidePlayerWithTileGrid(this);

            Color = Color.White;

            if (physicsManager.IsNearFloor(this))
                Color = Color.Black;

            physicsManager.CollideRectangleVsWorldBounds(this);

            weapon.Update();
        }


        internal void Draw(SpriteBatch spriteBatch)
        {
            //pSpriteBatch.Draw(mPlayerTexture, new Rectangle((int)position.X - HalfWidth, (int)position.Y - HalfHeight, width, height), color);
            base.Draw(spriteBatch, playerTexture, null, color);

            weapon.Draw(spriteBatch);
        }
    }
}
