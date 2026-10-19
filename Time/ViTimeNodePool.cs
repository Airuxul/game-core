namespace Air.GameCore
{
    /// <summary>
    /// 只池化节点对象。取出后由调用方持有，归还时摘掉回调。
    /// </summary>
    public static class ViTimeNodePool
    {
        public static ViTimeNode1 Get() => ObjectPool<ViTimeNode1>.Get();

        public static void Return(ViTimeNode1 node) => ObjectPool<ViTimeNode1>.Return(node);
    }
}
