using System;

namespace Air.GameCore
{
    public interface IViTimerHost
    {
        void SetTime(ViTimeNodeInterface node, float seconds, Action<ViTimeNodeInterface> callback);
        void ResetTime(ViTimeNodeInterface node, float seconds);
        void Modify(ViTimeNodeInterface node, int deltaTicks);
    }
}
