using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using CultOfJakito.UltraTelephone2.Events;
using UnityEngine;

namespace CultOfJakito.UltraTelephone2.Events
{
    public static class UKGameEventRegistry
    {
        private static Dictionary<Type, List<EventSubscriber>> _listeners = new Dictionary<Type, List<EventSubscriber>>();

        public static void RegisterListener(IEventListener listener)
        {
            Type type = listener.GetType();
            MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (MethodInfo method in methods)
            {
                EventListenerAttribute attribute = method.GetCustomAttribute<EventListenerAttribute>();
                if (attribute == null)
                    continue;

                ParameterInfo[] parameters = method.GetParameters();
                if (parameters.Length == 1)
                {
                    Type parameterType = parameters[0].ParameterType;
                    if (parameterType.IsSubclassOf(typeof(UKGameEvent)))
                    {
                        if (!_listeners.ContainsKey(parameterType))
                            _listeners[parameterType] = new List<EventSubscriber>();

                        _listeners[parameterType].Add(new EventSubscriber(method, listener));
                    }
                }
            }
        }

        public static void RaiseEvent<T>(T gameEvent) where T : UKGameEvent
        {
            Type type = typeof(T);

            if (!_listeners.ContainsKey(type))
                return;

            foreach (EventSubscriber subscriber in _listeners[type])
            {
                try
                {
                    subscriber?.Invoke(gameEvent);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error invoking event {type.Name} on {subscriber.TargetInstance.GetType().Name} - {e.Message}");
                }
            }
        }

        public static void RemoveListenerOfId(Guid id)
        {
            foreach (List<EventSubscriber> subscribers in _listeners.Values)
            {
                subscribers.RemoveAll(x => x.Id == id);
            }
        }
    }

}

public class EventSubscriber
{
    public MethodInfo Method { get; }
    public object TargetInstance { get; }
    public Guid Id { get; }

    public EventSubscriber(MethodInfo method, object targetInstance)
    {
        Method = method;
        TargetInstance = targetInstance;
        Id = Guid.NewGuid();
    }

    public void Invoke(object obj)
    {
        if(TargetInstance == null || Method == null)
        {
            UKGameEventRegistry.RemoveListenerOfId(Id);
            return;
        }

        Method.Invoke(TargetInstance, new object[] { obj });
    }
}
