using System;

namespace Air.GameCore
{
    public static class ViTimerInstance
    {
        internal static readonly ViTimer Timer = new ViTimer(300, 300);

        public static IViTimerHost Host => Timer;

        public static void SetTime(ViTimeNodeInterface node, float seconds, Action<ViTimeNodeInterface> callback) =>
            Timer.SetTime(node, seconds, callback);

        public static void ResetTime(ViTimeNodeInterface node, float seconds) => Timer.ResetTime(node, seconds);

        public static void Modify(ViTimeNodeInterface node, int deltaTicks) => Timer.Modify(node, deltaTicks);

        public static void Update(float deltaSeconds) => Timer.Update(deltaSeconds);

        internal static void Clear() => Timer.Clear();
    }

    public static class ViVisualTimerInstance
    {
        internal static readonly ViTimer Timer = new ViTimer(300, 300);

        public static IViTimerHost Host => Timer;

        public static void SetTime(ViTimeNodeInterface node, float seconds, Action<ViTimeNodeInterface> callback) =>
            Timer.SetTime(node, seconds, callback);

        public static void ResetTime(ViTimeNodeInterface node, float seconds) => Timer.ResetTime(node, seconds);

        public static void Modify(ViTimeNodeInterface node, int deltaTicks) => Timer.Modify(node, deltaTicks);

        public static void Update(float deltaSeconds) => Timer.Update(deltaSeconds);

        internal static void Clear() => Timer.Clear();
    }

    public sealed class ViVisualCustomTimerInstance : IViTimerHost
    {
        ViTimer _timer = new ViTimer(300, 300);
        float _scale = 1f;

        public void Start(long startTime, double accumulate, float scale, int rollSize0, int rollSize1)
        {
            var timer = new ViTimer(rollSize0, rollSize1);
            timer.SetClock(startTime, accumulate);
            _timer.Clear();
            _timer = timer;
            _scale = scale;
        }

        public void Update(float deltaSeconds)
        {
            if (_scale == 0f)
                return;
            _timer.Update(deltaSeconds * _scale);
        }

        public void End() => _timer.Clear();

        public void SetTime(ViTimeNodeInterface node, float seconds, Action<ViTimeNodeInterface> callback) =>
            _timer.SetTime(node, seconds, callback);

        public void ResetTime(ViTimeNodeInterface node, float seconds) => _timer.ResetTime(node, seconds);

        public void Modify(ViTimeNodeInterface node, int deltaTicks) => _timer.Modify(node, deltaTicks);
    }
}
