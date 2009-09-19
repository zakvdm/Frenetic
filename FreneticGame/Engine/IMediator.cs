using System;
using System.Collections.Generic;
using System.Reflection;
namespace Frenetic
{
    public interface IMediator
    {
        List<string> AvailableProperties { get; }
        List<string> AvailableActions { get; }
        string Process(string name, params object[] args);
        void Register(PropertyInfo property, object instance);
        void Register(MethodInfo method, string command, object instance);
    }
}
