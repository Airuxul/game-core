using System;
using System.Collections.Generic;

namespace Air.GameCore
{
    /// <summary>
    /// 玩法内托管延时。调用方不持有节点，结束时 <see cref="End"/> 一次收回。
    /// </summary>
    public sealed class ViTimePool
    {
        interface IPoolNode
        {
            string Key { get; }
            void ClearData();
            void ReturnToPool();
        }

        sealed class Node0 : ViTimeNodeBase, IPoolNode
        {
            public string Key { get; private set; }
            public Action UserCallback;
            public ViTimePool Owner;
            public readonly Action<ViTimeNodeInterface> Fire;

            public Node0() => Fire = OnFire;

            public void Arm(ViTimePool owner, string key, Action callback)
            {
                Owner = owner;
                Key = key;
                UserCallback = callback;
            }

            public void ClearData()
            {
                Key = null;
                UserCallback = null;
                Owner = null;
            }

            public void ReturnToPool() => ObjectPool<Node0>.Return(this);

            void OnFire(ViTimeNodeInterface _)
            {
                var callback = UserCallback;
                var owner = Owner;
                owner.Release(this);
                callback?.Invoke();
            }
        }

        sealed class Node1<T> : ViTimeNodeBase, IPoolNode
        {
            public string Key { get; private set; }
            public Action<T> UserCallback;
            public T Arg;
            public ViTimePool Owner;
            public readonly Action<ViTimeNodeInterface> Fire;

            public Node1() => Fire = OnFire;

            public void Arm(ViTimePool owner, string key, Action<T> callback, T arg)
            {
                Owner = owner;
                Key = key;
                UserCallback = callback;
                Arg = arg;
            }

            public void ClearData()
            {
                Key = null;
                UserCallback = null;
                Arg = default;
                Owner = null;
            }

            public void ReturnToPool() => ObjectPool<Node1<T>>.Return(this);

            void OnFire(ViTimeNodeInterface _)
            {
                var callback = UserCallback;
                var arg = Arg;
                var owner = Owner;
                owner.Release(this);
                callback?.Invoke(arg);
            }
        }

        sealed class Node2<T0, T1> : ViTimeNodeBase, IPoolNode
        {
            public string Key { get; private set; }
            public Action<T0, T1> UserCallback;
            public T0 Arg0;
            public T1 Arg1;
            public ViTimePool Owner;
            public readonly Action<ViTimeNodeInterface> Fire;

            public Node2() => Fire = OnFire;

            public void Arm(ViTimePool owner, string key, Action<T0, T1> callback, T0 arg0, T1 arg1)
            {
                Owner = owner;
                Key = key;
                UserCallback = callback;
                Arg0 = arg0;
                Arg1 = arg1;
            }

            public void ClearData()
            {
                Key = null;
                UserCallback = null;
                Arg0 = default;
                Arg1 = default;
                Owner = null;
            }

            public void ReturnToPool() => ObjectPool<Node2<T0, T1>>.Return(this);

            void OnFire(ViTimeNodeInterface _)
            {
                var callback = UserCallback;
                var arg0 = Arg0;
                var arg1 = Arg1;
                var owner = Owner;
                owner.Release(this);
                callback?.Invoke(arg0, arg1);
            }
        }

        readonly IViTimerHost _host;
        readonly Dictionary<string, ViTimeNodeBase> _keyed = new Dictionary<string, ViTimeNodeBase>();
        readonly HashSet<ViTimeNodeBase> _active = new HashSet<ViTimeNodeBase>();

        public ViTimePool(IViTimerHost host)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
        }

        public void Wait(float seconds, Action callback) =>
            Wait(null, seconds, callback);

        public void Wait<T>(float seconds, Action<T> callback, T arg) =>
            Wait(null, seconds, callback, arg);

        public void Wait<T0, T1>(float seconds, Action<T0, T1> callback, T0 arg0, T1 arg1) =>
            Wait(null, seconds, callback, arg0, arg1);

        public void Wait(string key, float seconds, Action callback)
        {
            Reserve(key);
            var node = ObjectPool<Node0>.Get();
            node.Arm(this, key, callback);
            Track(key, node);
            _host.SetTime(node, seconds, node.Fire);
        }

        public void Wait<T>(string key, float seconds, Action<T> callback, T arg)
        {
            Reserve(key);
            var node = ObjectPool<Node1<T>>.Get();
            node.Arm(this, key, callback, arg);
            Track(key, node);
            _host.SetTime(node, seconds, node.Fire);
        }

        public void Wait<T0, T1>(string key, float seconds, Action<T0, T1> callback, T0 arg0, T1 arg1)
        {
            Reserve(key);
            var node = ObjectPool<Node2<T0, T1>>.Get();
            node.Arm(this, key, callback, arg0, arg1);
            Track(key, node);
            _host.SetTime(node, seconds, node.Fire);
        }

        public void CancelWait(string key)
        {
            if (string.IsNullOrEmpty(key))
                return;
            if (_keyed.TryGetValue(key, out var node))
                Release(node);
        }

        public bool IsComplete(string key) => string.IsNullOrEmpty(key) || !_keyed.ContainsKey(key);

        public void End()
        {
            var pending = ListPool<ViTimeNodeBase>.Get();
            foreach (var node in _active)
                pending.Add(node);
            for (var i = 0; i < pending.Count; i++)
                Release(pending[i]);
            ListPool<ViTimeNodeBase>.Return(pending);
        }

        void Reserve(string key)
        {
            if (string.IsNullOrEmpty(key))
                return;
            if (_keyed.ContainsKey(key))
                throw new InvalidOperationException("Wait key is already pending: " + key);
        }

        void Track(string key, ViTimeNodeBase node)
        {
            _active.Add(node);
            if (!string.IsNullOrEmpty(key))
                _keyed.Add(key, node);
        }

        void Release(ViTimeNodeBase node)
        {
            if (!_active.Remove(node))
                return;
            if (node is IPoolNode pooled)
            {
                if (!string.IsNullOrEmpty(pooled.Key))
                    _keyed.Remove(pooled.Key);
                node.Detach();
                pooled.ClearData();
                pooled.ReturnToPool();
            }
        }
    }
}
