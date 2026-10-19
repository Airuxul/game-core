using System;

namespace Air.GameCore
{
    public sealed class ViAsynTupleCallback : ViEventNode, IEventInvoke0
    {
        Action<uint> _callback;
        bool _pending;

        internal void Bind(uint eventId, Action<uint> callback)
        {
            EventId = eventId;
            _callback = callback;
        }

        internal override void ClearCallback()
        {
            _callback = null;
            _pending = false;
        }

        internal override void RunAsync()
        {
            if (!_pending)
                return;
            _pending = false;
            _callback?.Invoke(EventId);
        }

        void IEventInvoke0.Invoke(uint eventId)
        {
            _pending = true;
            EventManager.Instance.Enqueue(this);
        }
    }

    public sealed class ViAsynTupleCallback<T0> : ViEventNode, IEventInvoke<T0>
    {
        Action<uint, T0> _callback;
        T0 _arg;
        bool _pending;

        internal void Bind(uint eventId, Action<uint, T0> callback)
        {
            EventId = eventId;
            _callback = callback;
        }

        internal override void ClearCallback()
        {
            _callback = null;
            _arg = default;
            _pending = false;
        }

        internal override void RunAsync()
        {
            if (!_pending)
                return;
            _pending = false;
            _callback?.Invoke(EventId, _arg);
        }

        void IEventInvoke<T0>.Invoke(uint eventId, T0 arg)
        {
            _arg = arg;
            _pending = true;
            EventManager.Instance.Enqueue(this);
        }
    }

    public sealed class ViAsynTupleCallback<T0, T1> : ViEventNode, IEventInvoke<T0, T1>
    {
        Action<uint, T0, T1> _callback;
        T0 _arg0;
        T1 _arg1;
        bool _pending;

        internal void Bind(uint eventId, Action<uint, T0, T1> callback)
        {
            EventId = eventId;
            _callback = callback;
        }

        internal override void ClearCallback()
        {
            _callback = null;
            _arg0 = default;
            _arg1 = default;
            _pending = false;
        }

        internal override void RunAsync()
        {
            if (!_pending)
                return;
            _pending = false;
            _callback?.Invoke(EventId, _arg0, _arg1);
        }

        void IEventInvoke<T0, T1>.Invoke(uint eventId, T0 arg0, T1 arg1)
        {
            _arg0 = arg0;
            _arg1 = arg1;
            _pending = true;
            EventManager.Instance.Enqueue(this);
        }
    }
}
