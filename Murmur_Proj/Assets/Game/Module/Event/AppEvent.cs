using System;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class AppEvent : UnityEvent<Component, EventArgs>
{
}
