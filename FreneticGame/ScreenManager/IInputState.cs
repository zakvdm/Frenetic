using System;
namespace Frenetic
{
    public interface IInputState
    {
        bool MarkReady { get; }
        bool MenuCancel { get; }
        bool MenuDown { get; }
        bool MenuSelect { get; }
        bool MenuUp { get; }
        bool PauseGame { get; }


    }
}
