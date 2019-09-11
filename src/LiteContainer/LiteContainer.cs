using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace LiteContainer
{
    public sealed class LiteContainer
    {
        private readonly ConcurrentDictionary<Type, Func<object>> _factories  = new ConcurrentDictionary<Type, Func<object>>();
        
        public T Resolve<T>()
        {
            var key = typeof(T);
            if (!_factories.ContainsKey(key))
                throw new KeyNotFoundException($"No registration found for type {typeof(T).FullName}");
            return (T)_factories[key]();
        }
        
        public void Register<T>(Type type, Func<T> createInstance)
        {            
            if (_factories.ContainsKey(type))
                throw new InvalidOperationException($"Factory for type {typeof(T).FullName} was already registered.");
            _factories.AddOrUpdate(type, () => (object)createInstance(), (k, v) => () => (object)createInstance());
        }
    }

    public static class LiteContainerExtensions
    {
        public static void Register<T>(this LiteContainer c, Func<T> createInstance) => c.Register(typeof(T), createInstance);
        
        public static void RegisterSingleton<T>(this LiteContainer c, Func<T> createInstance)
        {
            var once = new Lazy<T>(createInstance);
            c.Register(typeof(T), () => once.Value);
        }
    }
}
