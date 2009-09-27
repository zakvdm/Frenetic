/*
 * Algorithm loosely copied from the article on "Dynamic 2D Soft Shadows" at http://www.gamedev.net/reference/programming/features/2dsoftshadow/
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frenetic.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Frenetic.Player;

namespace Frenetic.Gameplay.Level
{
    public class VisibilityView : IView
    {
        public const int ShadowLength = 1000;
        public static Color ShadowColor = Color.Black;

        public VisibilityView(ILevel level, IPlayer player, IPrimitiveDrawer primitiveDrawer)
        {
            _level = level;
            _player = player;
            _primitiveDrawer = primitiveDrawer;
        }

        #region IView Members

        public void Generate(float elapsedSeconds)
        {
            foreach (LevelPiece piece in _level.Pieces)
            {
                if (SetBottomOrTopEdgeVertices(piece))
                {
                    DrawEdgeShadow(_topBotVertices);
                }

                if (SetLeftOrRightVertices(piece))
                {
                    DrawEdgeShadow(_leftRightVertices);
                }
            }
        }

        #endregion

        void DrawEdgeShadow(VertexPositionColor[] vertices)
        {
            vertices[VertexPosition.FarLeft] = ProjectOutFromPlayerThroughVertex(vertices[VertexPosition.NearLeft]);
            vertices[VertexPosition.FarRight] = ProjectOutFromPlayerThroughVertex(vertices[VertexPosition.NearRight]);

            _primitiveDrawer.DrawTriangleFan(vertices);
        }

        bool SetBottomOrTopEdgeVertices(LevelPiece piece)
        {
            // Check if we need to draw the top, bottom, or neither edge:
            if (_player.Position.Y > piece.BottomEdge)
            {
                /*
                above us:
                FL-----FR
                 
                 NL---NR
                    * 
                */
                _topBotVertices[VertexPosition.NearLeft] = new VertexPositionColor(new Vector3(piece.LeftEdge, piece.BottomEdge, 0), VisibilityView.ShadowColor);
                _topBotVertices[VertexPosition.NearRight] = new VertexPositionColor(new Vector3(piece.RightEdge, piece.BottomEdge, 0), VisibilityView.ShadowColor);
                return true;
            }
            else if (_player.Position.Y < piece.TopEdge)
            {
                /*
                below us:
                   * 
                NR---NL
                    
               FR-----FL
                */
                _topBotVertices[VertexPosition.NearLeft] = new VertexPositionColor(new Vector3(piece.RightEdge, piece.TopEdge, 0), VisibilityView.ShadowColor);
                _topBotVertices[VertexPosition.NearRight] = new VertexPositionColor(new Vector3(piece.LeftEdge, piece.TopEdge, 0), VisibilityView.ShadowColor);
                return true;
            }
            return false;
        }
        bool SetLeftOrRightVertices(LevelPiece piece)
        {
            // Check if we need to draw the left, right, or neither edge:
            if (_player.Position.X > piece.RightEdge)
            {
                /*
                left of us:
                FR
                |    R  
                |    |   *
                |    L  
                FL      
                */
                _leftRightVertices[VertexPosition.NearLeft] = new VertexPositionColor(new Vector3(piece.RightEdge, piece.BottomEdge, 0), VisibilityView.ShadowColor);
                _leftRightVertices[VertexPosition.NearRight] = new VertexPositionColor(new Vector3(piece.RightEdge, piece.TopEdge, 0), VisibilityView.ShadowColor);
                return true;
            }
            else if (_player.Position.X < piece.LeftEdge)
            {
                /*
                right of us:
                        FL
                    NL  |
                 *  |   |
                    NR  |
                        FR
                */
                _leftRightVertices[VertexPosition.NearLeft] = new VertexPositionColor(new Vector3(piece.LeftEdge, piece.TopEdge, 0), VisibilityView.ShadowColor);
                _leftRightVertices[VertexPosition.NearRight] = new VertexPositionColor(new Vector3(piece.LeftEdge, piece.BottomEdge, 0), VisibilityView.ShadowColor);
                return true;
            }
            return false;
        }

        VertexPositionColor ProjectOutFromPlayerThroughVertex(VertexPositionColor vertex)
        {
            Vector3 angleVector = new Vector3(vertex.Position.X - _player.Position.X, vertex.Position.Y - _player.Position.Y, 0);
            return new VertexPositionColor(vertex.Position + (angleVector * VisibilityView.ShadowLength), VisibilityView.ShadowColor);
        }

        ILevel _level;
        IPlayer _player;
        IPrimitiveDrawer _primitiveDrawer;

        VertexPositionColor[] _topBotVertices = new VertexPositionColor[4];
        VertexPositionColor[] _leftRightVertices = new VertexPositionColor[4];
    }

    static class VertexPosition
    {
        // NOTE: This represents a clockwise winding for drawing triangle fans (left and right are taken from the player position looking at the level piece)
        public const int NearLeft = 0;
        public const int FarLeft = 1;
        public const int FarRight = 2;
        public const int NearRight = 3;
    }
}
