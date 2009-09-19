using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using log4net;
using Frenetic.Engine;

namespace Frenetic
{
    public class Mediator : IMediator
    {
        public Mediator(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.GetLogger(this.GetType());
            _properties = new Dictionary<string, List<Delegate>>();
            _actions = new Dictionary<string, List<Action<object[]>>>();
        }

        public List<string> AvailableProperties
        {
            get
            {
                return _properties.Keys.ToList();
            }
        }

        public List<string> AvailableActions
        {
            get
            {
                return _actions.Keys.ToList();
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

        public void Register(MethodInfo method, string command, object instance)
        {
            if (!_actions.ContainsKey(command))
            {
                _actions.Add(command, new List<Action<object[]>>());
            }

            _actions[command].Add(new Action<object[]>((parameters) => method.Invoke(instance, parameters)));
        }

        public string Process(string name, params object[] args)
        {
            if (IsProperty(name))
            {
                if (args.Length == 0)
                {
                    return Get(name);
                }
                else
                {
                    Set(name, args);
                    return null;
                }
            }
            else if (IsAction(name))
            {
                Execute(name, args);
                return null;
            }
            else
            {
                _logger.Info("Unknown command: " + name);
                return null;
            }
        }

        private void Set(string name, params object[] args)
        {
            if (!IsProperty(name))
            {
                return;
            }

            string value = String.Join(" ", args.Select(a => a.ToString()).ToArray());

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

        private string Get(string name)
        {
            Func<object> getter = (Func<object>)_properties[name][1];

            Type type = GetTypeOfProperty(name);
            string value = "";

            if (type == typeof(Vector2))
            {
                Vector2 tmp = (Vector2)getter();
                value = tmp.X + " " + tmp.Y;
            }
            else if (type == typeof(Color))
            {
                Color tmp = (Color)getter();
                value = tmp.R + " " + tmp.G + " " + tmp.B;
            }
            else
            {
                value = getter().ToString();
            }

            _logger.Info(name + " is : " + value);
            return value;
        }

        public void Execute(string method, params object[] parameters)
        {
            var action = (Action<object[]>)_actions[method].First();

            action(parameters);
        }
        
        private void RegisterSetterAndGetter<PropertyType>(PropertyInfo property, object instance)
        {
            Action<PropertyType> setter = (value) => property.SetValue(instance, value, null);
            _properties[GetNameOfProperty(property)].Add(setter);

            Func<object> getter = () => property.GetValue(instance, null);
            _properties[GetNameOfProperty(property)].Add(getter);
        }

        private void Set<PropertyType>(string name, string value)
        {
            Action<PropertyType> setter = (Action<PropertyType>)_properties[name][0];

            try
            {
                PropertyType convertedValue = ConvertTo<PropertyType>(value);
                setter(convertedValue);
                _logger.Info(name + " set to : " + value);
            }
            catch (FormatException)
            {
                _logger.Info(value + " is not valid for : " + name);
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
        private bool IsProperty(string name)
        {
            if (_properties.ContainsKey(name))
            {
                return true;
            }

            _logger.Info("Unknown command: " + name);
            return false;
        }

        private bool IsAction(string name)
        {
            if (_actions.ContainsKey(name))
            {
                return true;
            }

            _logger.Info("Unknown action: " + name);
            return false;
        }

        private string GetNameOfProperty(PropertyInfo property)
        {
            // Get a name for the property in the form: {last piece of namespace} . {property name}
            string fullNamespace = property.ReflectedType.Namespace;
            string endOfNamespace = fullNamespace.Substring(fullNamespace.LastIndexOf('.') + 1);
        
            return endOfNamespace + "." + property.Name;
        }

        ILog _logger;
        private Dictionary<string, List<Delegate>> _properties;
        private Dictionary<string, List<Action<object[]>>> _actions;
    }
}
