using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Frenetic.Graphics
{
    public class XnaPrimitiveDrawer : IPrimitiveDrawer
    {
        public XnaPrimitiveDrawer(GraphicsDevice device, ICamera camera)
        {
            _device = device;
            _camera = camera;

            _effect = new BasicEffect(device, null);
            _effect.TextureEnabled = false;
            _effect.VertexColorEnabled = true;
            _effect.LightingEnabled = false;

            // NOTE: I'm not sure if these values are great... they shouldn't be hardcoded.
            _viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, 10f), Vector3.Zero, Vector3.Up);
            _projectionMatrix = Matrix.CreateOrthographicOffCenter(0, camera.ScreenWidth, camera.ScreenHeight, 0, 1, 50);

            _effect.View = _viewMatrix;
            _effect.Projection = _projectionMatrix;
        }
        #region IPrimitiveDrawer Members

        public void DrawTriangleFan(VertexPositionColor[] vertices)
        {
            _effect.World = _camera.TranslationMatrix;
            // NOTE: I'm guessing it's very ineffective to start a new pass for every primitive...
            _effect.Begin();
            _effect.CurrentTechnique.Passes[0].Begin();

            // Draw primitive:
            _device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleFan, vertices, 0, 2);

            _effect.CurrentTechnique.Passes[0].End();
            _effect.End();
        }

        #endregion

        GraphicsDevice _device;
        ICamera _camera;

        BasicEffect _effect;
        Matrix _viewMatrix;
        Matrix _projectionMatrix;
    }
}
