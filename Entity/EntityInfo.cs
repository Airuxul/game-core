namespace Air.GameCore.Entity
{
    public sealed class EntityInfo
    {
        public EntityId Id { get; internal set; }
        public EntityTypeId TypeId { get; internal set; }
        public EntityGroupName Group { get; internal set; }
        public EntityStatus Status { get; internal set; }
        public EntityId ParentId { get; internal set; }
        public IEntityLogic Logic { get; internal set; }
        public object UserData { get; internal set; }
    }
}
