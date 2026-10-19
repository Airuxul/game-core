using System;

namespace Air.GameCore
{
    public sealed class ViTupleCallback : ViEventNode, IEventInvoke0
    {
        Action<uint> _callback;

        internal void Bind(uint eventId, Action<uint> callback)
        {
            EventId = eventId;
            _callback = callback;
        }

        internal override void ClearCallback() => _callback = null;

        internal override void RunAsync()
        {
        }

        void IEventInvoke0.Invoke(uint eventId) => _callback?.Invoke(eventId);
    }

    public sealed class ViTupleCallback<T0> : ViEventNode, IEventInvoke<T0>
    {
        Action<uint, T0> _callback;

        internal void Bind(uint eventId, Action<uint, T0> callback)
        {
            EventId = eventId;
            _callback = callback;
        }

        internal override void ClearCallback() => _callback = null;

        internal override void RunAsync()
        {
        }

        void IEventInvoke<T0>.Invoke(uint eventId, T0 arg) => _callback?.Invoke(eventId, arg);
    }

    public sealed class ViTupleCallback<T0, T1> : ViEventNode, IEventInvoke<T0, T1>
    {
        Action<uint, T0, T1> _callback;

        internal void Bind(uint eventId, Action<uint, T0, T1> callback)
        {
            EventId = eventId;
            _callback = callback;
        }

        internal override void ClearCallback() => _callback = null;

        internal override void RunAsync()
        {
        }

        void IEventInvoke<T0, T1>.Invoke(uint eventId, T0 arg0, T1 arg1) =>
            _callback?.Invoke(eventId, arg0, arg1);
    }
}
