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
            _methods = new Dictionary<string, CommandContainer>();
            _properties = new Dictionary<string, PropertyContainer>();
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
                return _methods.Keys.ToList();
            }
        }
        
        public void Register(PropertyInfo property, object instance)
        {
            if (!(property.PropertyType == typeof(int) ||
                  property.PropertyType == typeof(string) ||
                  property.PropertyType == typeof(float) ||
                  property.PropertyType == typeof(Vector2) ||
                  property.PropertyType == typeof(Color)))
            {
                // Not a supported type...
                throw new ArgumentException("This implementation of IMediator does not support Properties of type " + property.PropertyType);
            }
            
            _properties.Add(GetNameOfProperty(property), new PropertyContainer() { PropertyInfo = property, Instance = instance });
        }
        public void Register(MethodInfo method, string command, object instance)
        {
            _methods.Add(command, new CommandContainer() { MethodInfo = method, Instance = instance });
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
            else if (IsMethod(name))
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

            object parameter;
            try
            {
                parameter = ConvertTo(type, value);
            }
            catch (FormatException)
            {
                _logger.Info(value + " is not valid for : " + name);
                // NOTE: If the conversion from string to the parameter type fails, we just do nothing...
                return;
            }

            _properties[name].PropertyInfo.SetValue(_properties[name].Instance, parameter, null);
            _logger.Info(name + " set to : " + value);
        }
        private string Get(string name)
        {
            object unconverted_value = _properties[name].PropertyInfo.GetValue(_properties[name].Instance, null);
            //Func<object> getter = (Func<object>)_properties[name][1];

            Type type = GetTypeOfProperty(name);
            string value = "";

            if (type == typeof(Vector2))
            {
                Vector2 tmp = (Vector2)unconverted_value;
                value = tmp.X + " " + tmp.Y;
            }
            else if (type == typeof(Color))
            {
                Color tmp = (Color)unconverted_value;
                value = tmp.R + " " + tmp.G + " " + tmp.B;
            }
            else
            {
                value = unconverted_value.ToString();
            }

            _logger.Info(name + " is : " + value);
            return value;
        }

        private void Execute(string method, params object[] parameters)
        {
            MethodInfo methodInfo = _methods[method].MethodInfo;

            object[] typedParams;
            try
            {
                typedParams = GetTypedParameterArray(method, parameters);
            }
            catch (FormatException e)
            {
                _logger.Info("Invalid parameter values...");
                // NOTE: If the conversion from string to the parameter type fails, we just do nothing...
                return;
            }

            methodInfo.Invoke(_methods[method].Instance, typedParams);
        }

        private object[] GetTypedParameterArray(string method, object[] parameters)
        {
            var methodInfo = _methods[method].MethodInfo;
            List<object> typedParams = new List<object>();
            int parameterIndex = 0;
            foreach (ParameterInfo paramInfo in methodInfo.GetParameters())
            {
                typedParams.Add(ConvertTo(paramInfo.ParameterType, parameters[parameterIndex].ToString()));
                parameterIndex++;
            }

            return typedParams.ToArray();
        }

        object ConvertTo(Type type, string value)
        {
            if (type == typeof(string))
            {
                return DoGenericConvert<string>(value);
            }
            if (type == typeof(int))
            {
                return DoGenericConvert<int>(value);
            }
            if (type == typeof(float))
            {
                return DoGenericConvert<float>(float.Parse(value));
            }
            if (type == typeof(Vector2))
            {
                Vector2 tmpVector;
                string[] args = value.Split(new char[] { ' ' }, 2);
                tmpVector.X = float.Parse(args[0]);
                tmpVector.Y = float.Parse(args[1]);
                return DoGenericConvert<Vector2>(tmpVector);
            }
            if (type == typeof(Color))
            {
                Color tmpColor = Color.White;
                string[] args = value.Split(new char[] { ' ' }, 3);
                tmpColor.R = (byte)(int.Parse(args[0]) % 255);
                tmpColor.G = (byte)(int.Parse(args[1]) % 255);
                tmpColor.B = (byte)(int.Parse(args[2]) % 255);
                return DoGenericConvert<Color>(tmpColor);
            }

            // Not a supported type...
            _logger.Info("Cannot do a conversion for " + value + " to Type: " + type);
            throw new ArgumentException("This implementation of IMediator does not support Methods with Parameters of type " + type);
        }
        PropertyType DoGenericConvert<PropertyType>(object value)
        {
            return (PropertyType)Convert.ChangeType(value, typeof(PropertyType));
        }

        private Type GetTypeOfProperty(string name)
        {
            return _properties[name].PropertyInfo.PropertyType;
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

        private bool IsMethod(string name)
        {
            if (_methods.ContainsKey(name))
            {
                return true;
            }

            _logger.Info("Unknown method: " + name);
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
        private Dictionary<string, CommandContainer> _methods;
        private Dictionary<string, PropertyContainer> _properties;
        
        private class CommandContainer
        {
            public MethodInfo MethodInfo { get; set; }
            public object Instance { get; set; }
        }
        private class PropertyContainer
        {
            public PropertyInfo PropertyInfo { get; set; }
            public object Instance { get; set; }
        }
    }
}
