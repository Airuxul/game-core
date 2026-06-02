namespace Air.GameCore.Entity
{
    public sealed class EntityGroup
    {
        public EntityGroupName Name { get; }
        public int InstanceCapacity { get; set; } = int.MaxValue;
        public float InstanceAutoReleaseInterval { get; set; }
        public bool Paused { get; set; }

        public EntityGroup(EntityGroupName name) => Name = name;
    }
}
