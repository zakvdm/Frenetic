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

        bool IsAReadWriteProperty(PropertyInfo propertyInfo)
        {
            if (!propertyInfo.GetSetMethod(true).IsPublic || !propertyInfo.GetGetMethod(true).IsPublic)
                throw new InvalidOperationException("Tweakable property {" + propertyInfo.ReflectedType + "." + propertyInfo.Name + "} is of the wrong type (should be public read/write)");

            return true;
        }

        IMediator _mediator;
    }
}
