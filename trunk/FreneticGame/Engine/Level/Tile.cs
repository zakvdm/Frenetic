using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frenetic
{
    public enum TileType
    {
        Empty,
        Solid,
    }

    public class Tile : GameplayObject
    {
        private TileType type;
        private Texture2D texture;
        private TileGrid grid;

        private List<GameplayObject> gameplayObjects;

        #region Properties
        public TileType Type
        {
            get { return type; }
            set { type = value; }
        }

        public List<GameplayObject> GameplayObjects
        {
            get { return gameplayObjects; }
        }

        public Tile Left
        {
            get
            {
                return grid.GetTile(new Vector2(position.X - width, position.Y));
            }
        }
        public Tile Right
        {
            get
            {
                return grid.GetTile(new Vector2(position.X + width, position.Y));
            }
        }
        public Tile Up
        {
            get
            {
                return grid.GetTile(new Vector2(position.X, position.Y - height));
            }
        }
        public Tile Down
        {
            get
            {
                return grid.GetTile(new Vector2(position.X, position.Y + height));
            }
        }

        public TileGrid Grid
        {
            get { return grid; }
        }
        #endregion

        public Tile(int tileWidth, int tileHeight, Texture2D texture, TileGrid grid)
        {
            width = tileWidth;
            height = tileHeight;
            type = TileType.Empty;

            this.texture = texture;
            this.grid = grid;

            gameplayObjects = new List<GameplayObject>();
        }

        public void Draw(SpriteBatch spriteBatch)//, Rectangle renderRectangle)
        {
            if (type == TileType.Solid)
            {
                //spriteBatch.Draw(texture, renderRectangle, Color.Gray);
                //spriteBatch.Draw(texture, rectangle, Color.Gray);
                base.Draw(spriteBatch, texture, null, Color.Gray);
            }
        }
    }
}

