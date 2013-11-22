using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;
using Loderpit.Components;

namespace Loderpit.Managers
{
    public class ResourceManager
    {
        private static Dictionary<string, object> _resources = new Dictionary<string, object>();

        public static void addResource(string name, object obj)
        {
            _resources.Add(name, obj);
        }

        public static void removeResource(string name)
        {
            _resources.Remove(name);
        }

        public static T getResource<T>(string name)
        {
            object value = null;

            _resources.TryGetValue(name, out value);

            return (T)value;
        }
    }
}
