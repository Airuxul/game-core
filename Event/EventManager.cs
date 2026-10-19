using System;
using System.Collections.Generic;

namespace Air.GameCore
{
    /// <summary>
    /// 全局事件。按事件 ID 分发，节点由订阅方持有并调用 <see cref="ViEventNode.End"/> 注销。
    /// 框架事件占用 1..999，游戏事件从 1000 起。
    /// </summary>
    public sealed class EventManager : Singleton<EventManager>
    {
        sealed class Chain
        {
            public ViEventNode Head;
            public ViEventNode Tail;
        }

        readonly Dictionary<uint, Chain> _chains = new Dictionary<uint, Chain>();
        ViEventNode _queueHead;
        ViEventNode _queueTail;

        public void Attach(Enum eventId, ViTupleCallback node, Action<uint> callback)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));
            node.End();
            node.Bind(ToId(eventId), callback);
            Link(node);
        }

        public void Attach<T0>(Enum eventId, ViTupleCallback<T0> node, Action<uint, T0> callback)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));
            node.End();
            node.Bind(ToId(eventId), callback);
            Link(node);
        }

        public void Attach<T0, T1>(Enum eventId, ViTupleCallback<T0, T1> node, Action<uint, T0, T1> callback)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));
            node.End();
            node.Bind(ToId(eventId), callback);
            Link(node);
        }

        public void Attach(Enum eventId, ViAsynTupleCallback node, Action<uint> callback)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));
            node.End();
            node.Bind(ToId(eventId), callback);
            Link(node);
        }

        public void Attach<T0>(Enum eventId, ViAsynTupleCallback<T0> node, Action<uint, T0> callback)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));
            node.End();
            node.Bind(ToId(eventId), callback);
            Link(node);
        }

        public void Attach<T0, T1>(Enum eventId, ViAsynTupleCallback<T0, T1> node, Action<uint, T0, T1> callback)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));
            node.End();
            node.Bind(ToId(eventId), callback);
            Link(node);
        }

        public void Invoke(Enum eventId)
        {
            var id = ToId(eventId);
            Broadcast(id, node =>
            {
                if (node is IEventInvoke0 invoke)
                    invoke.Invoke(id);
            });
        }

        public void Invoke<T0>(Enum eventId, T0 arg)
        {
            var id = ToId(eventId);
            Broadcast(id, node =>
            {
                if (node is IEventInvoke<T0> invoke)
                    invoke.Invoke(id, arg);
            });
        }

        public void Invoke<T0, T1>(Enum eventId, T0 arg0, T1 arg1)
        {
            var id = ToId(eventId);
            Broadcast(id, node =>
            {
                if (node is IEventInvoke<T0, T1> invoke)
                    invoke.Invoke(id, arg0, arg1);
            });
        }

        /// <summary>
        /// 执行本帧入队的异步回调。回调里新入队的留到下一次。
        /// </summary>
        public void Flush()
        {
            if (_queueHead == null)
                return;

            var pending = ListPool<ViEventNode>.Get();
            try
            {
                for (var node = _queueHead; node != null; node = node.QueueNext)
                    pending.Add(node);
                _queueHead = null;
                _queueTail = null;
                for (var i = 0; i < pending.Count; i++)
                {
                    pending[i].QueuePrev = null;
                    pending[i].QueueNext = null;
                    pending[i].Queued = false;
                }

                for (var i = 0; i < pending.Count; i++)
                {
                    var node = pending[i];
                    try
                    {
                        if (node.Queued)
                            continue;
                        node.RunAsync();
                    }
                    catch
                    {
                        for (var j = i + 1; j < pending.Count; j++)
                        {
                            var rest = pending[j];
                            if (!rest.Queued && rest.Listed)
                                Enqueue(rest);
                        }

                        throw;
                    }
                }
            }
            finally
            {
                ListPool<ViEventNode>.Return(pending);
            }
        }

        internal void Enqueue(ViEventNode node)
        {
            if (node.Queued)
                Unqueue(node);
            node.QueuePrev = _queueTail;
            node.QueueNext = null;
            if (_queueTail != null)
                _queueTail.QueueNext = node;
            else
                _queueHead = node;
            _queueTail = node;
            node.Queued = true;
        }

        internal void Unqueue(ViEventNode node)
        {
            if (!node.Queued)
                return;
            if (node.QueuePrev != null)
                node.QueuePrev.QueueNext = node.QueueNext;
            else if (_queueHead == node)
                _queueHead = node.QueueNext;
            if (node.QueueNext != null)
                node.QueueNext.QueuePrev = node.QueuePrev;
            else if (_queueTail == node)
                _queueTail = node.QueuePrev;
            node.QueuePrev = null;
            node.QueueNext = null;
            node.Queued = false;
        }

        internal void Unlist(ViEventNode node)
        {
            if (!node.Listed)
                return;
            if (!_chains.TryGetValue(node.EventId, out var chain))
            {
                node.Listed = false;
                node.Prev = null;
                node.Next = null;
                return;
            }

            if (node.Prev != null)
                node.Prev.Next = node.Next;
            else if (chain.Head == node)
                chain.Head = node.Next;
            if (node.Next != null)
                node.Next.Prev = node.Prev;
            else if (chain.Tail == node)
                chain.Tail = node.Prev;
            node.Prev = null;
            node.Next = null;
            node.Listed = false;
            if (chain.Head == null)
                _chains.Remove(node.EventId);
        }

        internal void Reset()
        {
            var pending = new List<ViEventNode>();
            foreach (var chain in _chains.Values)
            {
                for (var node = chain.Head; node != null; node = node.Next)
                    pending.Add(node);
            }

            for (var node = _queueHead; node != null; node = node.QueueNext)
            {
                if (!node.Listed)
                    pending.Add(node);
            }

            for (var i = 0; i < pending.Count; i++)
                pending[i].End();
            _chains.Clear();
            _queueHead = null;
            _queueTail = null;
        }

        void Link(ViEventNode node)
        {
            if (!_chains.TryGetValue(node.EventId, out var chain))
            {
                chain = new Chain();
                _chains[node.EventId] = chain;
            }

            node.Prev = chain.Tail;
            node.Next = null;
            if (chain.Tail != null)
                chain.Tail.Next = node;
            else
                chain.Head = node;
            chain.Tail = node;
            node.Listed = true;
        }

        void Broadcast(uint id, Action<ViEventNode> call)
        {
            if (!Snapshot(id, out var pending))
                return;
            try
            {
                for (var i = 0; i < pending.Count; i++)
                {
                    var node = pending[i];
                    if (node.Listed && node.EventId == id)
                        call(node);
                }
            }
            finally
            {
                ListPool<ViEventNode>.Return(pending);
            }
        }

        bool Snapshot(uint id, out List<ViEventNode> pending)
        {
            pending = null;
            if (!_chains.TryGetValue(id, out var chain) || chain.Head == null)
                return false;
            pending = ListPool<ViEventNode>.Get();
            for (var node = chain.Head; node != null; node = node.Next)
            {
                pending.Add(node);
                if (node == chain.Tail)
                    break;
            }

            return true;
        }

        static uint ToId(Enum eventId)
        {
            if (eventId == null)
                throw new ArgumentNullException(nameof(eventId));
            return Convert.ToUInt32(eventId);
        }
    }
}
