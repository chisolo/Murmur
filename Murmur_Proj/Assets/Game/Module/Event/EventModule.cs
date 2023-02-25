using System;
using System.Collections.Generic;
using Lemegeton;
using UnityEngine;
using UnityEngine.Events;

public partial class EventModule : Singleton<EventModule>
{
    protected EventModule() { }

    public Dictionary<string, AppEvent> _events;


    public void Init()
    {
        _events = new Dictionary<string, AppEvent>();
    }
    public void FireEvent(string name)
    {
        this.FireEvent(name, null, EventArgs.Empty);
    }
    public void FireEvent(string name, EventArgs e)
    {
        this.FireEvent(name, null, e);
    }
    public void FireEvent(string name, Component sender)
    {
        this.FireEvent(name, sender, EventArgs.Empty);
    }
    public void FireEvent(string name, Component sender, EventArgs e)
    {
        if(_events.TryGetValue(name, out var eventHandler)) {
            eventHandler.Invoke(sender, e);
        }
    }
    public void Register(string name, UnityAction<Component, EventArgs> handler)
    {
        // TODO: record the registered target to enable auto release when gameobject is been deleted
        // handler.Target
        if (!_events.ContainsKey(name)) {
            _events.Add(name, new AppEvent());
        }

        _events[name].AddListener(handler);
    }

    public void UnRegister(string name, UnityAction<Component, EventArgs> handler)
    {
        if (!_events.ContainsKey(name)) {
            Debug.LogErrorFormat("Event {0} is not Registered", name);
            return;
        }

        _events[name].RemoveListener(handler);
    }
}


