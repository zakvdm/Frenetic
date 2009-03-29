#region Using Statements


using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


#endregion

namespace Frenetic.MyConsole.Components
{
    public class FPS : DrawableGameComponent
    {
        private ContentManager content;
        private SpriteBatch    batch;
        private SpriteFont     font;

        private float elapsedTime, totalFrames, fps;


        public FPS(Game game)
            : base(game)
        {
            content = new ContentManager(game.Services, "Content");
        }


        public override void Initialize()
        {
            base.Initialize();
        }


        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            if (loadAllContent)
            {
                batch = new SpriteBatch(GraphicsDevice);
                font  = content.Load<SpriteFont>("Fonts/detailsFont");
            }

            base.LoadGraphicsContent(loadAllContent);
        }


        public override void Draw(GameTime gameTime)
        {
            elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            totalFrames++;

            if (elapsedTime >= 1.0f)
            {
                fps = totalFrames;
                totalFrames = 0;
                elapsedTime = elapsedTime - 1.0f;
            }

            batch.Begin();
            batch.DrawString(font, fps.ToString(), new Vector2(40.0f, GraphicsDevice.Viewport.Height - 40.0f - font.MeasureString(fps.ToString()).Y), Color.White);
            batch.End();


            base.Draw(gameTime);
        }
    }
}