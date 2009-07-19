using System;
using System.Linq;
using Frenetic.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Frenetic.Engine.Overlay;
using System.Collections.Generic;
using Frenetic.Player;

namespace Frenetic.Gameplay.HUD
{
    public class ScoreOverlayView : IOverlayView
    {
        const string PLAYER = "PLAYER";
        const string SCORE = "SCORE";
        const string DEATHS = "DEATHS";

        Color HEADING_COLOR = Color.Black;
        Color VALUES_COLOR = Color.Blue;

        const int MAX_NAME_LENGTH = 20;

        public ScoreOverlayView(List<IPlayer> playerList, Rectangle scoreWindow, IFont font)
        {
            _players = playerList;
            this.Window = scoreWindow;
            _font = font;

            this.BackgroundColor = OverlaySetView.BACKGROUND_COLOR;

            SCORE_OFFSET = new Vector2(scoreWindow.Width / 2, 0);
            DEATHS_OFFSET = new Vector2(scoreWindow.Width * ((float)2 / 3), 0);
        }

        public bool Visible { get; set; }
        public Rectangle Window { get; set; }
        public Color BackgroundColor { get; set; }

        public void Draw(ISpriteBatch spritebatch)
        {
            Vector2 currentTextPosition = new Vector2(this.Window.Left + OverlaySetView.TEXT_OFFSET.X, this.Window.Top + OverlaySetView.TEXT_OFFSET.Y);
            spritebatch.DrawText(_font, ScoreOverlayView.PLAYER, currentTextPosition, HEADING_COLOR);
            spritebatch.DrawText(_font, ScoreOverlayView.SCORE, currentTextPosition + SCORE_OFFSET, HEADING_COLOR);
            spritebatch.DrawText(_font, ScoreOverlayView.DEATHS, currentTextPosition + DEATHS_OFFSET, HEADING_COLOR);
            currentTextPosition.Y += _font.LineSpacing;

            List<IPlayer> playersSortedByScore = new List<IPlayer>();
            //foreach (IPlayer player in _players)
            //{
                //int index = FindIndex((player_in_list) => player.PlayerScore > player_in_list.PlayerScore);
                //playersSortedByScore.Insert((index == -1) ? 0: index, player);  
            //}
            foreach (IPlayer player in _players.OrderByDescending((p) => p.PlayerScore))
            {
                string name = player.PlayerSettings.Name;
                spritebatch.DrawText(_font, name.Substring(0, name.Length > MAX_NAME_LENGTH ? MAX_NAME_LENGTH : name.Length), currentTextPosition, VALUES_COLOR);
                spritebatch.DrawText(_font, player.PlayerScore.Kills.ToString(), currentTextPosition + SCORE_OFFSET, VALUES_COLOR);
                spritebatch.DrawText(_font, player.PlayerScore.Deaths.ToString(), currentTextPosition + DEATHS_OFFSET, VALUES_COLOR);
                currentTextPosition.Y += _font.LineSpacing;
            }
        }

        List<IPlayer> _players;
        IFont _font;

        Vector2 SCORE_OFFSET;
        Vector2 DEATHS_OFFSET;
    }
}
