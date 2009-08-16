using System;
namespace Frenetic.Engine
{
    public interface IDiffable<T>
    {
        void Clean();
        T GetDiff();
        bool IsDirty { get; }
    }
}
