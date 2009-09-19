using System;
using NUnit.Framework;
using Rhino.Mocks;
using Frenetic.Gameplay.Level;
using Frenetic.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Frenetic.Network;
using Frenetic;
using Frenetic.Physics;
using Microsoft.Xna.Framework.Graphics;
using Frenetic.Player;

namespace UnitTestLibrary
{
    [TestFixture]
    public class VisibilityViewTests
    {
        ILevel stubLevel;
        IPlayer stubPlayer;
        IPrimitiveDrawer stubPrimitiveDrawer;
        VisibilityView visibilityView;

        VertexPositionColor nearLeftVertex, nearRightVertex, farLeftVertex, farRightVertex;

        [SetUp]
        public void SetUp()
        {
            stubLevel = MockRepository.GenerateStub<ILevel>();
            stubPlayer = MockRepository.GenerateStub<IPlayer>();
            stubPrimitiveDrawer = MockRepository.GenerateStub<IPrimitiveDrawer>();
            visibilityView = new VisibilityView(stubLevel, stubPlayer, stubPrimitiveDrawer);

            List<LevelPiece> pieces = new List<LevelPiece>();
            stubLevel.Stub(me => me.Pieces).Return(pieces);
        }

        [Test]
        public void DoesntDoAnythingWithAnEmptyLevel()
        {
            visibilityView.Generate();

            stubPrimitiveDrawer.AssertWasNotCalled(me => me.DrawTriangleFan(Arg<VertexPositionColor[]>.Is.Anything));
        }

        [Test]
        public void DrawsAShadowForTheBottomOfALevelPiece()
        {
            // the block is top left (relative to the player)
            LevelPiece piece = LevelPieceHelper.MakeLevelPiece(new Vector2(100, 100), new Vector2(40, 20));
            stubLevel.Pieces.Add(piece);
            stubPlayer.Position = new Vector2(200, 200);
            // The expected vertex positions:
            nearLeftVertex = new VertexPositionColor(new Vector3(80, 110, 0), VisibilityView.ShadowColor);
            nearRightVertex = new VertexPositionColor(new Vector3(120, 110, 0), VisibilityView.ShadowColor);
            farLeftVertex = new VertexPositionColor(nearLeftVertex.Position + new Vector3(-120 * VisibilityView.ShadowLength, -90 * VisibilityView.ShadowLength, 0), VisibilityView.ShadowColor);
            farRightVertex = new VertexPositionColor(nearRightVertex.Position + new Vector3(-80 * VisibilityView.ShadowLength, -90 * VisibilityView.ShadowLength, 0), VisibilityView.ShadowColor);

            visibilityView.Generate();

            stubPrimitiveDrawer.AssertWasCalled(me => me.DrawTriangleFan(Arg<VertexPositionColor[]>.Is.Equal(new VertexPositionColor[4] { nearLeftVertex, farLeftVertex, farRightVertex, nearRightVertex })));
        }

        [Test]
        public void DrawsAShadowForTheTopOfALevelPiece()
        {
            // the block is bottom right (relative to the player)
            LevelPiece piece = LevelPieceHelper.MakeLevelPiece(new Vector2(300, 400), new Vector2(20, 20));
            stubLevel.Pieces.Add(piece);
            stubPlayer.Position = new Vector2(100, 100);
            // The expected vertex positions:
            nearLeftVertex = new VertexPositionColor(new Vector3(310, 390, 0), VisibilityView.ShadowColor);
            nearRightVertex = new VertexPositionColor(new Vector3(290, 390, 0), VisibilityView.ShadowColor);
            farLeftVertex = new VertexPositionColor(nearLeftVertex.Position + new Vector3(210 * VisibilityView.ShadowLength, 290 * VisibilityView.ShadowLength, 0), VisibilityView.ShadowColor);
            farRightVertex = new VertexPositionColor(nearRightVertex.Position + new Vector3(190 * VisibilityView.ShadowLength, 290 * VisibilityView.ShadowLength, 0), VisibilityView.ShadowColor);

            visibilityView.Generate();

            stubPrimitiveDrawer.AssertWasCalled(me => me.DrawTriangleFan(Arg<VertexPositionColor[]>.Is.Equal(new VertexPositionColor[4] { nearLeftVertex, farLeftVertex, farRightVertex, nearRightVertex })));
        }

        [Test]
        public void DrawsAShadowForTheLeftOfALevelPiece()
        {
            // the block is bottom right (relative to the player)
            LevelPiece piece = LevelPieceHelper.MakeLevelPiece(new Vector2(300, 400), new Vector2(20, 20));
            stubLevel.Pieces.Add(piece);
            stubPlayer.Position = new Vector2(100, 100);
            // The expected vertex positions:
            nearLeftVertex = new VertexPositionColor(new Vector3(290, 390, 0), VisibilityView.ShadowColor);
            nearRightVertex = new VertexPositionColor(new Vector3(290, 410, 0), VisibilityView.ShadowColor);
            farLeftVertex = new VertexPositionColor(nearLeftVertex.Position + new Vector3(190 * VisibilityView.ShadowLength, 290 * VisibilityView.ShadowLength, 0), VisibilityView.ShadowColor);
            farRightVertex = new VertexPositionColor(nearRightVertex.Position + new Vector3(190 * VisibilityView.ShadowLength, 310 * VisibilityView.ShadowLength, 0), VisibilityView.ShadowColor);

            visibilityView.Generate();

            stubPrimitiveDrawer.AssertWasCalled(me => me.DrawTriangleFan(Arg<VertexPositionColor[]>.Is.Equal(new VertexPositionColor[4] { nearLeftVertex, farLeftVertex, farRightVertex, nearRightVertex })));
        }

        [Test]
        public void DrawsAShadowForTheRightOfALevelPiece()
        {
            // the block is bottom right (relative to the player)
            LevelPiece piece = LevelPieceHelper.MakeLevelPiece(new Vector2(300, 400), new Vector2(20, 20));
            stubLevel.Pieces.Add(piece);
            stubPlayer.Position = new Vector2(500, 400);
            // The expected vertex positions:
            nearLeftVertex = new VertexPositionColor(new Vector3(310, 410, 0), VisibilityView.ShadowColor);
            nearRightVertex = new VertexPositionColor(new Vector3(310, 390, 0), VisibilityView.ShadowColor);
            farLeftVertex = new VertexPositionColor(nearLeftVertex.Position + new Vector3(-190 * VisibilityView.ShadowLength, 10 * VisibilityView.ShadowLength, 0), VisibilityView.ShadowColor);
            farRightVertex = new VertexPositionColor(nearRightVertex.Position + new Vector3(-190 * VisibilityView.ShadowLength, -10 * VisibilityView.ShadowLength, 0), VisibilityView.ShadowColor);

            visibilityView.Generate();

            stubPrimitiveDrawer.AssertWasCalled(me => me.DrawTriangleFan(Arg<VertexPositionColor[]>.Is.Equal(new VertexPositionColor[4] { nearLeftVertex, farLeftVertex, farRightVertex, nearRightVertex })));
        }

        [Test]
        public void OnlyDrawsShadowsForVisibleEdges()
        {
            // By placing the player inside the level piece, we make sure that none of the edges are "visible" to the player, so no shadows should get drawn...
            LevelPiece piece = LevelPieceHelper.MakeLevelPiece(new Vector2(300, 400), new Vector2(20, 20));
            stubLevel.Pieces.Add(piece);
            stubPlayer.Position = new Vector2(305, 397);

            visibilityView.Generate();

            stubPrimitiveDrawer.AssertWasNotCalled(me => me.DrawTriangleFan(Arg<VertexPositionColor[]>.Is.Anything));
        }
    }
}
