using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventBusManager : SingleTon<EventBusManager>
{
    private Dictionary<Type, Delegate> delegateDictionary = new Dictionary<Type, Delegate>();

    private Dictionary<Type, BaseEvent> eventInstanceDictionary = new Dictionary<Type, BaseEvent>();

    public T GetEventInstance<T>() where T : BaseEvent, new()
    {
        if(!eventInstanceDictionary.ContainsKey(typeof(T)))
        {
            eventInstanceDictionary.Add(typeof(T), new T());
        }
        return (T)eventInstanceDictionary[typeof(T)];
    }
    public void Publish<T>(T e)
    {
        if (delegateDictionary.TryGetValue(typeof(T), out var subscribers))
        {
            var subscriberList = subscribers as Action<T>;
            subscriberList?.Invoke(e);
        }
    }
    public void Subscribe<T>(Action<T> subscriber)
    {
        if (delegateDictionary.ContainsKey(typeof(T)))
        {
            delegateDictionary[typeof(T)] = Delegate.Combine(delegateDictionary[typeof(T)], subscriber);
        }
        else
        {
            delegateDictionary[typeof(T)] = subscriber;
        }
    }
    public void Unsubscribe<T>(Action<T> subscriber)
    {
        if (delegateDictionary.ContainsKey(typeof(T)))
        {
            var currentDelegate = delegateDictionary[typeof(T)];
            var newDelegate = Delegate.Remove(currentDelegate, subscriber);
            if (newDelegate == null)
            {
                delegateDictionary.Remove(typeof(T));
            }
            else
            {
                delegateDictionary[typeof(T)] = newDelegate;
            }
        }
    }
}
public class BaseEvent
{

}
