using System;

namespace Air.GameCore
{
    /// <summary>
    /// 两级时间轮。刻度 10ms。超过两级覆盖范围的节点放在有序预约链表里。
    /// </summary>
    public sealed class ViTimer : IViTimerHost
    {
        public const double TickSeconds = 0.01;

        readonly int _roll0;
        readonly int _roll1;
        readonly long _span1;
        readonly ViTimeNodeBase[] _level0;
        readonly ViTimeNodeBase[] _level1;
        ViTimeNodeBase _reserve;
        ViTimeNodeBase _pending;
        long _now;
        double _carry;
        bool _updating;

        public ViTimer(int rollSize0, int rollSize1)
        {
            if (rollSize0 < 1)
                throw new ArgumentOutOfRangeException(nameof(rollSize0));
            if (rollSize1 < 1)
                throw new ArgumentOutOfRangeException(nameof(rollSize1));
            _roll0 = rollSize0;
            _roll1 = rollSize1;
            _span1 = (long)rollSize0 * rollSize1;
            _level0 = new ViTimeNodeBase[rollSize0];
            _level1 = new ViTimeNodeBase[rollSize1];
        }

        internal void SetClock(long startTick, double accumulateSeconds)
        {
            _now = startTick;
            _carry = accumulateSeconds * 100.0;
            if (_carry < 0)
                _carry = 0;
        }

        public void Update(double deltaSeconds)
        {
            if (deltaSeconds <= 0)
                return;
            _carry += deltaSeconds * 100.0;
            if (_updating)
                return;
            var ticks = (long)_carry;
            if (ticks <= 0)
                return;
            _carry -= ticks;
            _updating = true;
            try
            {
                for (var i = 0L; i < ticks; i++)
                    Step();
                // One callback may have called Update. Apply that time once; further
                // nested calls stay in _carry for the next Update.
                ticks = (long)_carry;
                if (ticks <= 0)
                    return;
                _carry -= ticks;
                for (var i = 0L; i < ticks; i++)
                    Step();
            }
            finally
            {
                _updating = false;
            }
        }

        public void SetTime(ViTimeNodeInterface node, float seconds, Action<ViTimeNodeInterface> callback)
        {
            var timeNode = AsNode(node);
            timeNode.Detach();
            timeNode.Callback = callback;
            timeNode.Host = this;
            timeNode.TriggerTick = _now + DelayTicks(seconds);
            Insert(timeNode);
        }

        public void ResetTime(ViTimeNodeInterface node, float seconds)
        {
            var timeNode = AsNode(node);
            if (timeNode.Host != this)
                return;
            timeNode.TriggerTick = _now + DelayTicks(seconds);
            Unlink(timeNode);
            Insert(timeNode);
        }

        public void Modify(ViTimeNodeInterface node, int deltaTicks)
        {
            var timeNode = AsNode(node);
            if (timeNode.Host != this)
                return;
            var trigger = timeNode.TriggerTick + deltaTicks;
            if (trigger <= _now)
                trigger = _now + 1;
            timeNode.TriggerTick = trigger;
            Unlink(timeNode);
            Insert(timeNode);
        }

        public void Clear()
        {
            while (_pending != null)
            {
                var node = _pending;
                node.Detach();
            }

            for (var i = 0; i < _level0.Length; i++)
            {
                while (_level0[i] != null)
                    _level0[i].Detach();
            }

            for (var i = 0; i < _level1.Length; i++)
            {
                while (_level1[i] != null)
                    _level1[i].Detach();
            }

            while (_reserve != null)
                _reserve.Detach();
        }

        internal void Unlink(ViTimeNodeBase node)
        {
            if (node.Level < 0)
                return;

            if (node.Prev != null)
                node.Prev.Next = node.Next;
            else if (node.Level == 0)
                _level0[node.Slot] = node.Next;
            else if (node.Level == 1)
                _level1[node.Slot] = node.Next;
            else if (node.Level == 2)
                _reserve = node.Next;
            else if (node.Level == 3)
                _pending = node.Next;

            if (node.Next != null)
                node.Next.Prev = node.Prev;
            node.Prev = null;
            node.Next = null;
            node.Level = -1;
            node.Slot = -1;
        }

        void Step()
        {
            _now++;
            if (_now % _roll0 == 0)
            {
                var slot = (int)((_now / _roll0) % _roll1);
                var cascaded = _level1[slot];
                _level1[slot] = null;
                Cascade(cascaded);
            }

            if (_now % _span1 == 0)
                DrainReserve();

            var index = (int)(_now % _roll0);
            var head = _level0[index];
            _level0[index] = null;
            Fire(head);
        }

        void Cascade(ViTimeNodeBase head)
        {
            while (head != null)
            {
                var node = head;
                head = node.Next;
                node.Prev = null;
                node.Next = null;
                node.Level = -1;
                node.Slot = -1;
                Insert(node);
            }
        }

        void DrainReserve()
        {
            while (_reserve != null && _reserve.TriggerTick - _now < _span1)
            {
                var node = _reserve;
                Unlink(node);
                Insert(node);
            }
        }

        void Fire(ViTimeNodeBase head)
        {
            var node = head;
            while (node != null)
            {
                node.Level = 3;
                node = node.Next;
            }

            _pending = head;
            while (_pending != null)
            {
                node = _pending;
                Unlink(node);
                node.Host = null;
                var callback = node.Callback;
                node.Callback = null;
                if (callback == null)
                    continue;
                try
                {
                    callback(node);
                }
                catch (Exception ex)
                {
                    ViTimerLog.Exception?.Invoke(ex);
                }
            }
        }

        void Insert(ViTimeNodeBase node)
        {
            var delta = node.TriggerTick - _now;
            if (delta < _roll0)
            {
                var slot = (int)(node.TriggerTick % _roll0);
                if (slot < 0)
                    slot += _roll0;
                Link(node, 0, slot, ref _level0[slot]);
                return;
            }

            if (delta < _span1)
            {
                var slot = (int)((node.TriggerTick / _roll0) % _roll1);
                if (slot < 0)
                    slot += _roll1;
                Link(node, 1, slot, ref _level1[slot]);
                return;
            }

            InsertReserve(node);
        }

        static void Link(ViTimeNodeBase node, int level, int slot, ref ViTimeNodeBase head)
        {
            node.Level = level;
            node.Slot = slot;
            node.Prev = null;
            node.Next = head;
            if (head != null)
                head.Prev = node;
            head = node;
        }

        void InsertReserve(ViTimeNodeBase node)
        {
            ViTimeNodeBase prev = null;
            var current = _reserve;
            while (current != null && current.TriggerTick <= node.TriggerTick)
            {
                prev = current;
                current = current.Next;
            }

            node.Level = 2;
            node.Slot = -1;
            node.Prev = prev;
            node.Next = current;
            if (prev != null)
                prev.Next = node;
            else
                _reserve = node;
            if (current != null)
                current.Prev = node;
        }

        static ViTimeNodeBase AsNode(ViTimeNodeInterface node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));
            if (node is ViTimeNodeBase timeNode)
                return timeNode;
            throw new ArgumentException("Node does not belong to this timer.", nameof(node));
        }

        static long DelayTicks(float seconds)
        {
            if (seconds <= 0f)
                return 1;
            var ticks = (long)Math.Round(seconds * 100.0, MidpointRounding.AwayFromZero);
            return ticks < 1 ? 1 : ticks;
        }
    }
}
