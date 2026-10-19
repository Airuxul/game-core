namespace Air.GameCore
{
    /// <summary>
    /// 真实时间每帧最多推进 0.5 秒，余量留到后续帧。
    /// </summary>
    public struct RealTimeBudget
    {
        public const float MaxStep = 0.5f;

        public float Pending;

        public float Consume(float deltaSeconds)
        {
            if (deltaSeconds < 0f)
                deltaSeconds = 0f;
            Pending += deltaSeconds;
            var step = Pending > MaxStep ? MaxStep : Pending;
            Pending -= step;
            return step;
        }
    }
}
