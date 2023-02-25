using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace Lemegeton
{
    public class TimerModule : Singleton<TimerModule>
    {
        private class TimerContext
        {
            public long timerId;
            public Action onComplete;
            public Action<float> onStep;
            public bool realtime;
            public float duration;
            public float timer;
            public bool frame;
            public int loop;
            public float second;
            public float lastTime;
        }
        protected TimerModule() { }
        private Dictionary<long, TimerContext> _timers = new Dictionary<long, TimerContext>();
        private TimerContext[] _timerContexts;
        private List<long> _expiredTimers = new List<long>();
        private long serial = 1;

        public void Init()
        {
            CleanAll();
            serial = 0;
        }

        public long CreateFrameTimer(float duration, Action onComplete, bool realtime = true, Action<float> onStep = null,  int loop = 0)
        {
            ++serial;
            var context = GenericPool<TimerContext>.Get();
            context.timerId = serial;
            context.onComplete = onComplete;
            context.onStep = onStep;
            context.realtime = realtime;
            context.duration = duration;
            context.timer = duration;
            context.loop = loop;
            context.frame = true;
            context.second = 0;
            context.lastTime = Time.realtimeSinceStartup;
            _timers.Add(serial, context);
            _timerContexts = _timers.Values.ToArray();
            return serial;
        }
        public long CreateTimer(float duration, Action onComplete,  bool realtime = true, Action<float> onStep = null, int loop = 0)
        {
            ++serial;
            var context = GenericPool<TimerContext>.Get();
            context.timerId = serial;
            context.onComplete = onComplete;
            context.onStep = onStep;
            context.realtime = realtime;
            context.duration = duration;
            context.timer = duration;
            context.loop = loop;
            context.frame = false;
            context.second = 0;
            context.lastTime = Time.realtimeSinceStartup;
            _timers.Add(serial, context);
            _timerContexts = _timers.Values.ToArray();
            return serial;
        }
        public float GetTimer(long timerId)
        {
            if(_timers.TryGetValue(timerId, out var timer)) {
                return timer.timer;
            }
            return 0;
        }
        public void UpdateTimer(long timerId, float duration)
        {
            if(_timers.TryGetValue(timerId, out var timer)) {
                timer.timer = duration;
            }
        }
        public void CancelTimer(long timerId)
        {
            if(_timers.TryGetValue(timerId, out var timer)) {
                timer.onStep = null;
                timer.onComplete = null;
            }
            _expiredTimers.Add(timerId);
        }
        private void CleanAll()
        {
            foreach(var timer in _timers.Values) {
                GenericPool<TimerContext>.Release(timer);
            }
            _timers.Clear();
            _expiredTimers.Clear();
            _timerContexts = null;
        }

        void Update()
        {
            if(_timerContexts == null) return;

            float delta = Time.deltaTime;
            foreach(var context in _timerContexts) {
                float realDelta = Time.realtimeSinceStartup - context.lastTime;
                context.lastTime = Time.realtimeSinceStartup;
                var passTime = context.realtime ? realDelta : delta;
                context.second += passTime;
                if(context.timer > passTime) context.timer -= passTime;
                else context.timer = 0;

                if(context.frame) {
                    context.onStep?.Invoke(context.timer);
                } else {
                    var accSec = Mathf.FloorToInt(context.second);
                    if(accSec >= 1) {
                        context.second -= accSec;
                        context.onStep?.Invoke(context.timer);
                    }
                }
                if(context.timer > 0) continue;
                context.onComplete?.Invoke();
                if(context.loop > 0) {
                    context.loop--;
                    context.timer = context.duration;
                    context.second = 0;
                } else if(context.loop < 0) {
                    context.timer = context.duration;
                    context.second = 0;
                } else {
                    CancelTimer(context.timerId);
                }
            }
            if(_expiredTimers.Count > 0)
            {
                foreach (var timerId in _expiredTimers)
                {
                    _timers.Remove(timerId);
                }
                _expiredTimers.Clear();
                _timerContexts = _timers.Values.ToArray();
            }
        }
    }
}

