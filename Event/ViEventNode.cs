namespace Air.GameCore
{
    /// <summary>
    /// 事件链表上的一个回调节点。注销只走 <see cref="End"/>。
    /// </summary>
    public abstract class ViEventNode
    {
        internal ViEventNode Prev;
        internal ViEventNode Next;
        internal ViEventNode QueuePrev;
        internal ViEventNode QueueNext;
        internal uint EventId;
        internal bool Listed;
        internal bool Queued;

        public bool Active => Listed || Queued;

        public void End()
        {
            if (Listed)
                EventManager.Instance.Unlist(this);
            if (Queued)
                EventManager.Instance.Unqueue(this);
            ClearCallback();
        }

        internal abstract void ClearCallback();

        internal abstract void RunAsync();
    }

    internal interface IEventInvoke0
    {
        void Invoke(uint eventId);
    }

    internal interface IEventInvoke<T0>
    {
        void Invoke(uint eventId, T0 arg);
    }

    internal interface IEventInvoke<T0, T1>
    {
        void Invoke(uint eventId, T0 arg0, T1 arg1);
    }
}
