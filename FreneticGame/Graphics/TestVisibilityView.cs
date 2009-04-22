using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Frenetic.Level;

namespace Frenetic.Graphics
{
    class TestVisibilityView : IView
    {

        public TestVisibilityView(GraphicsDevice device, ILevel level, IPlayer player, ICamera camera)
        {
            _device = device;

            _level = level;
            _player = player;
            _camera = camera;

            _drawingEffect = new BasicEffect(_device, null);
            _drawingEffect.TextureEnabled = false;
            _drawingEffect.VertexColorEnabled = true;
            _drawingEffect.LightingEnabled = false;
        }

        GraphicsDevice _device;
        ILevel _level;
        IPlayer _player;
        ICamera _camera;

        BasicEffect _drawingEffect;

        #region IView Members

        public void Generate()
        {
            DrawShadows();
        }

        #endregion

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

                Vector2 pos = new Vector2(_player.Position.X, _player.Position.Y);

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

                DrawShadowForLine(shadowVertices);
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

                Vector2 pos = new Vector2(_player.Position.X, _player.Position.Y);

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

                DrawShadowForLine(shadowVertices);
            }
        }

        void DrawShadowForLine(VertexPositionColor[] shadowVertices)
        {
            Vector3 pos = new Vector3(_player.Position.X, _player.Position.Y, 0);
            Color shadowColor = Color.Black;

            Vector3 angleLeft, angleRight;
            angleLeft = shadowVertices[0].Position - pos;
            angleRight = shadowVertices[2].Position - pos;

            shadowVertices[1] = new VertexPositionColor(shadowVertices[0].Position + angleLeft * 1000, shadowColor);
            shadowVertices[3] = new VertexPositionColor(shadowVertices[2].Position + angleRight * 1000, shadowColor);

            VertexPositionColor[] tmp = new VertexPositionColor[4];
            tmp[0] = shadowVertices[0];
            tmp[1] = shadowVertices[1];
            tmp[2] = shadowVertices[3];
            tmp[3] = shadowVertices[2];


            _device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleFan, tmp, 0, 2);
        }

        
    }
}
