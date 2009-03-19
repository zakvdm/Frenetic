using Microsoft.Xna.Framework.Input;

namespace Frenetic
{
    public class FreneticMouse : IMouse
    {
        #region IMouse Members

        public bool LeftButtonIsDown()
        {
            return Mouse.GetState().LeftButton == ButtonState.Pressed;
        }

        public bool RightButtonIsDown()
        {
            return Mouse.GetState().RightButton == ButtonState.Pressed;
        }

        #endregion
    }
}
