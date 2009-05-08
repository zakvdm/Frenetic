using System;
using System.Collections.Generic;
using System.Reflection;
namespace Frenetic
{
    public interface IMediator
    {
        List<string> AvailableProperties { get; }
        void Set(string name, string value);
        string Get(string name);
        void Register(PropertyInfo property, object instance);
    }
}
