using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Frenetic.Level;

namespace Frenetic.Graphics
{
    class TestVisibilityView : IView, IController
    {

        public TestVisibilityView(GraphicsDevice device, ContentManager contentManager, Frenetic.Level.Level level, IPlayer player, ICamera camera)
        {
            _device = device;

            _renderTarget = new RenderTarget2D(device, 800, 600, 1, device.DisplayMode.Format);
            _spriteBatch = new SpriteBatch(device);
            _contentManager = contentManager;
            _level = level;
            _player = player;
            _camera = camera;

            _contentManager.Load<Texture2D>("Textures/ball");

            _drawingEffect = new BasicEffect(_device, null);
            _drawingEffect.TextureEnabled = false;
            _drawingEffect.VertexColorEnabled = true;
            _drawingEffect.LightingEnabled = false;
        }

        GraphicsDevice _device;
        RenderTarget2D _renderTarget;
        SpriteBatch _spriteBatch;
        ContentManager _contentManager;
        Frenetic.Level.Level _level;
        IPlayer _player;
        ICamera _camera;

        Texture2D _texture;
        BasicEffect _drawingEffect;

        #region IController Members

        public void Process(float elapsedTime)
        {
            Setup();
        }

        #endregion

        void Setup()
        {
            _device.SetRenderTarget(0, _renderTarget);
            _device.Clear(ClearOptions.Target, Color.Gray, 0, 0);


            //_spriteBatch.Begin();
            //_spriteBatch.Draw(_contentManager.Load<Texture2D>("Textures/ball"), new Rectangle(200, 100, 400, 400), Color.DarkGray);
            //_spriteBatch.End();

            DrawShadows();

            _device.SetRenderTarget(0, null);
            _texture = _renderTarget.GetTexture();
        }

        #region IView Members

        public void Generate()
        {
            DrawRenderTarget();
        }

        #endregion

        void DrawRenderTarget()
        {
            if (_texture == null)
                return;
            _spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
            // sourcecolor * sourceblend + destcolor * destblend => sourcecolor * destcolor + destcolor * sourcecolor => 2 * sourcecolor * destcolor
            _device.RenderState.SourceBlend = Blend.DestinationColor;
            _device.RenderState.DestinationBlend = Blend.SourceColor;
            _spriteBatch.Draw(_texture, Vector2.Zero, Color.White);
            _spriteBatch.End();
        }

        void DrawShadows()
        {
            Matrix worldMatrix = _camera.TranslationMatrix;
            Matrix viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, 10f), Vector3.Zero, Vector3.Up);
            Matrix projectionMatrix = Matrix.CreateOrthographicOffCenter(0, 800f, 600f, 0, 1, 50);

            _drawingEffect.World = worldMatrix;
            _drawingEffect.Projection = projectionMatrix;
            _drawingEffect.View = viewMatrix;

            _drawingEffect.Begin();

            _drawingEffect.CurrentTechnique.Passes[0].Begin();

            foreach (LevelPiece piece in _level.Pieces)
            {
                DrawTopAndBottomShadow(piece);

                DrawLeftAndRightShadow(piece);
            }
            
            

            _drawingEffect.CurrentTechnique.Passes[0].End();
            _drawingEffect.End();
        }


        void DrawLeftAndRightShadow(LevelPiece piece)
        {
            VertexPositionColor[] shadowVertices = new VertexPositionColor[4];

            Color shadowColor = Color.Black;

            Vector2 smallX = Vector2.Zero;
            Vector2 bigX = Vector2.Zero;

            for (int pass = 1; pass <= 2; pass++)
            {
                if (pass == 1)
                {
                    // Left:
                    smallX = new Vector2(piece.Position.X - (piece.Size.X / 2), piece.Position.Y - (piece.Size.Y / 2)); // top
                    bigX = new Vector2(piece.Position.X - (piece.Size.X / 2), piece.Position.Y + (piece.Size.Y / 2));   // bottom
                }
                if (pass == 2)
                {
                    // Right:
                    smallX = new Vector2(piece.Position.X + (piece.Size.X / 2), piece.Position.Y - (piece.Size.Y / 2));
                    bigX = new Vector2(piece.Position.X + (piece.Size.X / 2), piece.Position.Y + (piece.Size.Y / 2));
                }

                Vector3 pos = new Vector3(_player.Position, 0);

                if (smallX.X > pos.X)
                {
                    /*
                    right of us:
                            2FL
                        1L  |
                     *  |   |
                        3R  |
                            4FR
                    */
                    shadowVertices[0] = new VertexPositionColor(new Vector3(smallX.X, smallX.Y, 0), shadowColor);
                    shadowVertices[2] = new VertexPositionColor(new Vector3(bigX.X, bigX.Y, 0), shadowColor);
                }
                else
                {
                    /*
                    left of us:
                    4FR
                    |    3R  
                    |    |   *
                    |    1L  
                    2FL      
                    */
                    shadowVertices[0] = new VertexPositionColor(new Vector3(bigX.X, bigX.Y, 0), shadowColor);
                    shadowVertices[2] = new VertexPositionColor(new Vector3(smallX.X, smallX.Y, 0), shadowColor);
                }

                DrawLine(shadowVertices);
            }
        }


        void DrawTopAndBottomShadow(LevelPiece piece)
        {
            VertexPositionColor[] shadowVertices = new VertexPositionColor[4];

            Color shadowColor = Color.Black;

            Vector2 smallX = Vector2.Zero;
            Vector2 bigX = Vector2.Zero;

            for (int pass = 1; pass <= 2; pass++)
            {
                if (pass == 1)
                {
                    // Bottom:
                    smallX = new Vector2(piece.Position.X - (piece.Size.X / 2), piece.Position.Y + (piece.Size.Y / 2));
                    bigX = new Vector2(piece.Position.X + (piece.Size.X / 2), piece.Position.Y + (piece.Size.Y / 2));
                }
                if (pass == 2)
                {
                    // Top:
                    smallX = new Vector2(piece.Position.X - (piece.Size.X / 2), piece.Position.Y - (piece.Size.Y / 2));
                    bigX = new Vector2(piece.Position.X + (piece.Size.X / 2), piece.Position.Y - (piece.Size.Y / 2));
                }

                Vector3 pos = new Vector3(_player.Position, 0);

                if (smallX.Y > pos.Y)
                {
                    /*
                    below us:
                     * 
                    R3---1L
                
                  FR4-----2FL
                    */
                    shadowVertices[0] = new VertexPositionColor(new Vector3(bigX.X, bigX.Y, 0), shadowColor);
                    shadowVertices[2] = new VertexPositionColor(new Vector3(smallX.X, smallX.Y, 0), shadowColor);
                }
                else
                {
                    /*
                    above us:
                  FL2-----4FR
                 
                    L1---3R
                      * 
                    */
                    shadowVertices[0] = new VertexPositionColor(new Vector3(smallX.X, smallX.Y, 0), shadowColor);
                    shadowVertices[2] = new VertexPositionColor(new Vector3(bigX.X, bigX.Y, 0), shadowColor);
                }

                DrawLine(shadowVertices);
            }
        }

        void DrawLine(VertexPositionColor[] shadowVertices)
        {
            Vector3 pos = new Vector3(_player.Position, 0);
            Color shadowColor = Color.Black;

            Vector3 angleLeft, angleRight;
            angleLeft = shadowVertices[0].Position - pos;
            angleRight = shadowVertices[2].Position - pos;

            shadowVertices[1] = new VertexPositionColor(shadowVertices[0].Position + angleLeft * 1000, shadowColor);
            shadowVertices[3] = new VertexPositionColor(shadowVertices[2].Position + angleRight * 1000, shadowColor);

            _device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, shadowVertices, 0, 2);
        }

        
    }

    struct Shadow
    {
        public VertexPositionColor NearLeft;
        public VertexPositionColor NearRight;
        public VertexPositionColor FarLeft;
        public VertexPositionColor FarRight;
    }
}
