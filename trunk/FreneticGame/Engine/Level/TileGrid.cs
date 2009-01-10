using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frenetic
{
    public class TileGrid : IEnumerable<Tile>
    {
        private int rows = 20;
        private int columns = 28;

        private int tileWidth = 50;
        private int tileHeight = 50;

        private Vector2 min;
        private Vector2 max;

        private Texture2D tempBlank;

        public int Rows
        {
            get { return rows; }
            set { rows = value; }
        }
        public int Columns
        {
            get { return columns; }
            set { columns = value; }
        }
        public int TileWidth
        {
            get { return tileWidth; }
            set { tileWidth = value; }
        }
        public int TileHeight
        {
            get { return tileHeight; }
            set { tileHeight = value; }
        }

        private List<Tile>[] grid;

        public List<Tile> this[int i]
        {
            get { return grid[i]; }
        }

        public Vector2 MinimumPoint
        {
            get { return min; }
        }
        public Vector2 MaximumPoint
        {
            get { return max; }
        }

        public TileGrid(Texture2D tempTexture)
        { 
            grid = new List<Tile>[rows + 2];
            this.tempBlank = tempTexture;
        }

        #region IEnumerable<Tile> Members

        IEnumerator<Tile> IEnumerable<Tile>.GetEnumerator()
        {
            foreach (List<Tile> row in grid)
            {
                foreach(Tile tile in row)
                {
                    yield return tile;
                }
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        public void Initialize()
        {
            min = new Vector2(tileWidth, tileHeight);
            max = new Vector2(tileWidth + (tileWidth * columns), tileHeight + (tileHeight * rows));

            // Create padded grid of 'Cell's
            for (int row = 0; row < rows + 2; row++)
            {
                grid[row] = new List<Tile>();
                for (int column = 0; column < columns + 2; column++)
                {
                    grid[row].Add(new Tile(tileWidth, tileHeight, tempBlank, this));
                    grid[row][column].Position = new Vector2((column * tileWidth) + (tileWidth / 2),
                        row * tileHeight + (tileHeight / 2));
                }
            }

            // Setup border
            for (int row = 0; row < rows + 2; row++) // Vertical edges
            {
                grid[row][0].Type = TileType.Solid;
                grid[row][columns + 1].Type = TileType.Solid;
            }
            for (int column = 0; column < columns + 2; column++)
            {
                grid[0][column].Type = TileType.Solid;
                grid[rows + 1][column].Type = TileType.Solid;
            }
        }

        public Tile GetTile(Vector2 position)
        {
            int col = (int)Math.Floor(position.X / tileWidth);
            int row = (int)Math.Floor(position.Y / tileHeight);

            return this[row][col];
        }

        public void PlaceGameplayObject(GameplayObject gameplayObject)
        {
            //if (gameplayObject.Tile != null)
            {
                //gameplayObject.Tile.GameplayObjects.Remove(gameplayObject);
            }
            Tile tile = GetTile(gameplayObject.Position);
            tile.GameplayObjects.Add(gameplayObject);
        }

        public void TempLoadLevel()
        {
            /*
            for (int col = 1; col < 8; col++)
            {
                grid[3][col + 4].Type = TileType.Solid;
                grid[11][col + 11].Type = TileType.Solid;
                grid[6][col + 7].Type = TileType.Solid;
            }
            for (int col = 1; col < 6; col++)
            {
                grid[17][col + 2].Type = TileType.Solid;
                grid[15][col + 16].Type = TileType.Solid;
            }
             */
            Random rand = new Random();

            foreach (Tile tile in this)
            {
                if (tile.Type == TileType.Solid)
                    continue;

                if (rand.Next(10) <= 1)
                {
                    tile.Type = TileType.Solid;
                }
            }
        }
    }
}
