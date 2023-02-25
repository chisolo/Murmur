using System;
using UnityEngine;
using UnityEngine.Pool;

public abstract class BaseEventArgs : EventArgs, IDisposable
{
    protected bool disposed = false;

    public BaseEventArgs()
    {
        //Debug.Log("BaseEventArgs");
    }

    ~BaseEventArgs()
    {
        this.Dispose(false);
    }

    protected abstract void Return();

    public void Dispose()
    {
        Dispose(true);
    }

    private void Dispose(bool isDisposing)
    {
        if (disposed) return;
        if (isDisposing) Return();
        disposed = true;
    }
}

public class BaseEventArgs<T> : BaseEventArgs where T: BaseEventArgs<T>, new()
{
    public static T Get()
    {
        var obj = GenericPool<T>.Get();
        obj.disposed = false;
        return obj;
    }

    protected override void Return()
    {
        GenericPool<T>.Release((T)this);
    }
}
