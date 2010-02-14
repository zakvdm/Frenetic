using Microsoft.Xna.Framework.Input;
using System;

namespace Frenetic.UserInput
{
    public class XnaMouse : IMouse
    {
        #region IMouse Members

        public bool IsKeyDown(MouseKeys mousekey)
        {
            switch (mousekey)
            {
                case MouseKeys.Left:
                    return Mouse.GetState().LeftButton == ButtonState.Pressed;
                case MouseKeys.Right:
                    return Mouse.GetState().RightButton == ButtonState.Pressed;
                default:
                    return false;
            }
        }

        #endregion
    }
}
