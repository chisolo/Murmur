using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Lemegeton
{
    public class TaskModule : Singleton<TaskModule>
    {
        public void NextFrame(Action action)
        {
            Process(NextFrameCo(action));
        }

        private IEnumerator NextFrameCo(Action action)
        {
            yield return null;

            action?.Invoke();
        }

        public void Process(IEnumerator action)
        {
            StartCoroutine(action);
        }

        public void Delay(int ms, Action action)
        {
            DelayAsync(ms, action);
        }

        public async void DelayAsync(int ms, Action action)
        {
            await Task.Delay(ms);
            action?.Invoke();
        }


        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            mainThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }

        private static int mainThreadId;
        private static readonly Queue<Action> mainThreadActions = new Queue<Action>();
        private static bool hasMainThreadActions;

        private static bool OnMainThread => mainThreadId == System.Threading.Thread.CurrentThread.ManagedThreadId;

        public void RunOnMainThread(Action action)
        {
            if (OnMainThread) {
                action();
            } else {
                lock (mainThreadActions)
                {
                    mainThreadActions.Enqueue(action);
                    hasMainThreadActions = true;
                }
            }
        }

        void Update()
        {
            if (!hasMainThreadActions)
            {
                return;
            }

            int count;
            lock (mainThreadActions)
            {
                count = mainThreadActions.Count;
                hasMainThreadActions = false;
            }

            for (; 0 < count; --count)
            {
                Action action;
                lock (mainThreadActions)
                {
                    action = mainThreadActions.Dequeue();
                }
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
    }
}