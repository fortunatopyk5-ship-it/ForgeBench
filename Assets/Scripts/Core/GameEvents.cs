using System;
using System.Collections.Generic;

namespace ForgeBench
{
    public sealed class GameEvents
    {
        private readonly Dictionary<string, Action<object>> routes = new Dictionary<string, Action<object>>();

        public void Subscribe(string key, Action<object> handler)
        {
            if (!routes.ContainsKey(key)) routes[key] = delegate { };
            routes[key] += handler;
        }

        public void Unsubscribe(string key, Action<object> handler)
        {
            if (routes.ContainsKey(key)) routes[key] -= handler;
        }

        public void Publish(string key, object payload = null)
        {
            Action<object> route;
            if (routes.TryGetValue(key, out route)) route(payload);
        }
    }
}
