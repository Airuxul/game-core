using System;

namespace Air.GameCore
{
    public interface ViTimeNodeInterface
    {
        void Detach();
        bool IsAttach();
    }

    public abstract class ViTimeNodeBase : ViTimeNodeInterface
    {
        internal ViTimer Host;
        internal ViTimeNodeBase Prev;
        internal ViTimeNodeBase Next;
        internal long TriggerTick;
        internal int Level = -1;
        internal int Slot = -1;
        internal Action<ViTimeNodeInterface> Callback;

        public bool IsAttach() => Host != null;

        public void Detach()
        {
            if (Host != null)
            {
                var host = Host;
                Host = null;
                host.Unlink(this);
            }

            Callback = null;
        }
    }

    public sealed class ViTimeNode1 : ViTimeNodeBase, IPoolable
    {
        public void Reset() => Detach();
    }

    public sealed class ViTimeNodeEx1<T> : ViTimeNodeBase
    {
        public T Value;
    }
}
