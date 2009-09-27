using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Frenetic.Graphics
{
    public class BubbleTextDrawer : IBubbleTextDrawer
    {
        public BubbleTextDrawer(IFont font)
        {
            this.Font = font;
        }

        public void AddText(string text, Vector2 position, Color color, float fadeOutTime)
        {
            this.BubbleTexts.Add(new BubbleText() { Text = text, Position = position, Color = color, TotalTime = fadeOutTime });
        }

        public void DrawText(ISpriteBatch spriteBatch, float elapsedSeconds)
        {
            foreach (var bubbleText in this.BubbleTexts)
            {
                float transitionOffset = 1 - (bubbleText.TimeRemaining / bubbleText.TotalTime);
                // Modulate size:
                float scale = MathHelper.SmoothStep(0f, 1f, transitionOffset) + 0.5f;
                // Modulate alpha:
                var color = new Color(bubbleText.Color.R, bubbleText.Color.G, bubbleText.Color.B, MathHelper.SmoothStep(1f, 0f, transitionOffset));

                spriteBatch.DrawText(this.Font, bubbleText.Text, bubbleText.Position, color, scale);

                bubbleText.TimeRemaining -= elapsedSeconds;
            }

            this.BubbleTexts.RemoveAll((bubbleTxt) => bubbleTxt.TimeRemaining < 0);
        }

        IFont Font;

        List<BubbleText> BubbleTexts = new List<BubbleText>();

        private class BubbleText
        {
            public string Text { get; set; }
            public Vector2 Position { get; set; }
            public Color Color { get; set; }
            public float TotalTime
            {
                get { return _totalTime; }
                set
                {
                    _totalTime = value;
                    this.TimeRemaining = value;
                }
            }
            public float TimeRemaining { get; set; }

            float _totalTime;
        }
    }
}
