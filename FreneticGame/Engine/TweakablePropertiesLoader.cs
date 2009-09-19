using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;

namespace Frenetic
{
    [AttributeUsage(AttributeTargets.Property)]
    class TweakableAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    class Command : Attribute
    {
        public Command(string commandName)
        {
            Name = commandName;
        }

        public string Name { get; set; }
    }

    class TweakablePropertiesLoader
    {
        public TweakablePropertiesLoader(IMediator mediator)
        {
            _mediator = mediator;
        }

        public void LoadTweakableProperties(object instance)
        {
            Type type = instance.GetType();
            var properties = type.GetProperties();
            foreach (PropertyInfo propinfo in properties)
            {
                RegisterPropertyWithMediator(propinfo, instance, IsAReadWriteProperty);
            }
        }

        internal void LoadCommands(object instance)
        {
            Type type = instance.GetType();
            var methods = type.GetMethods();
            foreach (var method in methods)
            {
                RegisterMethodWithMediator(method, instance);
            }
        }

        void RegisterPropertyWithMediator(PropertyInfo propertyInfo, object instance, Predicate<PropertyInfo> isAValidProperty)
        {
            var tweakableAttribute = propertyInfo.GetCustomAttributes(typeof(TweakableAttribute), true);
            if (tweakableAttribute != null && tweakableAttribute.Length > 0)
            {
                if (isAValidProperty(propertyInfo))
                {
                    _mediator.Register(propertyInfo, instance);
                }
            }
        }

        void RegisterMethodWithMediator(MethodInfo methodInfo, object instance)
        {
            var commandAttributes = (Command[])methodInfo.GetCustomAttributes(typeof(Command), true);
            if (commandAttributes != null && commandAttributes.Length > 0)
            {
                _mediator.Register(methodInfo, commandAttributes.First().Name, instance);
            }
        }

        bool IsAReadWriteProperty(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.GetSetMethod(true).IsPublic || !propertyInfo.GetGetMethod(true).IsPublic)
                throw new InvalidOperationException("Tweakable property {" + propertyInfo.ReflectedType + "." + propertyInfo.Name + "} is of the wrong type (should be public read/write)");

            return true;
        }

        IMediator _mediator;
    }
}
