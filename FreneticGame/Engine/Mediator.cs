using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frenetic
{
    public class Mediator : IMediator
    {
        public Mediator()
        {
            _properties = new Dictionary<string, List<Delegate>>();
        }

        public List<string> AvailableProperties
        {
            get
            {
                return _properties.Keys.ToList();
            }
        }
        
        public void Register(PropertyInfo property, object instance)
        {
            _properties.Add(GetNameOfProperty(property), new List<Delegate>());

            if (property.PropertyType == typeof(int))
            {
                RegisterSetterAndGetter<int>(property, instance);
            }
            else if (property.PropertyType == typeof(string))
            {
                RegisterSetterAndGetter<string>(property, instance);
            }
            else if (property.PropertyType == typeof(float))
            {
                RegisterSetterAndGetter<float>(property, instance);
            }
            else if (property.PropertyType == typeof(Vector2))
            {
                RegisterSetterAndGetter<Vector2>(property, instance);
            }
            else if (property.PropertyType == typeof(Color))
            {
                RegisterSetterAndGetter<Color>(property, instance);
            }
            else
            {
                // Not a supported type...
                throw new ArgumentException("This implementation of IMediator does not support Properties of type " + property.PropertyType);
            }
        }

        public void Set(string name, string value)
        {
            if (!IsPropertyRegistered(name))
            {
                return;
            }

            Type type = GetTypeOfProperty(name);

            if (type == typeof(int))
            {
                Set<int>(name, value);
            }
            if (type == typeof(string))
            {
                Set<string>(name, value);
            }
            if (type == typeof(float))
            {
                Set<float>(name, value);
            }
            if (type == typeof(Vector2))
            {
                Set<Vector2>(name, value);
            }
            if (type == typeof(Color))
            {
                Set<Color>(name, value);
            }
        }

        public string Get(string name)
        {
            if (!IsPropertyRegistered(name))
            {
                return null;
            }
            Func<string> getter = (Func<string>)_properties[name][1];

            return getter();
        }
        
        private void RegisterSetterAndGetter<PropertyType>(PropertyInfo property, object instance)
        {
            Action<PropertyType> setter = (value) => property.SetValue(instance, value, null);
            _properties[GetNameOfProperty(property)].Add(setter);
            
            Func<string> getter = () => property.GetValue(instance, null).ToString();
            _properties[GetNameOfProperty(property)].Add(getter);
        }

        private void Set<PropertyType>(string name, string value)
        {
            Action<PropertyType> setter = (Action<PropertyType>)_properties[name][0];

            try
            {
                PropertyType convertedValue = ConvertTo<PropertyType>(value);
                setter(convertedValue);
            }
            catch (FormatException invalidConversionException)
            {
                // NOTE: If the conversion from string to the property type fails, we just do nothing...
            }
        }

        PropertyType ConvertTo<PropertyType>(string value)
        {
            if (typeof(PropertyType) == typeof(string))
            {
                return DoGenericConvert<PropertyType>(value);
            }
            if (typeof(PropertyType) == typeof(int))
            {
                return DoGenericConvert<PropertyType>(value);
            }
            if (typeof(PropertyType) == typeof(float))
            {
                return DoGenericConvert<PropertyType>(float.Parse(value));
            }
            if (typeof(PropertyType) == typeof(Vector2))
            {
                Vector2 tmpVector;
                string[] args = value.Split(new char[] { ' ' }, 2);
                tmpVector.X = float.Parse(args[0]);
                tmpVector.Y = float.Parse(args[1]);
                return DoGenericConvert<PropertyType>(tmpVector);
            }
            if (typeof(PropertyType) == typeof(Color))
            {
                Color tmpColor = Color.White;
                string[] args = value.Split(new char[] { ' ' }, 3);
                tmpColor.R = (byte)(int.Parse(args[0]) % 255);
                tmpColor.G = (byte)(int.Parse(args[1]) % 255);
                tmpColor.B = (byte)(int.Parse(args[2]) % 255);
                return DoGenericConvert<PropertyType>(tmpColor);
            }

            return default(PropertyType);
        }
        PropertyType DoGenericConvert<PropertyType>(object value)
        {
            return (PropertyType)Convert.ChangeType(value, typeof(PropertyType));
        }

        private Type GetTypeOfProperty(string name)
        {
            return _properties[name][0].Method.GetParameters()[0].ParameterType;
        }
        private bool IsPropertyRegistered(string name)
        {
            if (!_properties.ContainsKey(name))
                return false;

            return true;
        }
        private string GetNameOfProperty(PropertyInfo property)
        {
            // Get a name for the property in the form: {last piece of namespace} . {property name}
            string fullNamespace = property.ReflectedType.Namespace;
            string endOfNamespace = fullNamespace.Substring(fullNamespace.LastIndexOf('.') + 1);
        
            return endOfNamespace + "." + property.Name;
        }

        private Dictionary<string, List<Delegate>> _properties;
    }
}
