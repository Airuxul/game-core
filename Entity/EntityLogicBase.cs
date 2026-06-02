namespace Air.GameCore.Entity
{
    public abstract class EntityLogicBase : IEntityLogic
    {
        public EntityId EntityId { get; private set; }
        public EntityTypeId TypeId { get; private set; }
        public EntityGroupName Group { get; private set; }
        public EntityStatus Status { get; private set; } = EntityStatus.Unknown;
        public object UserData { get; private set; }

        public void OnInit(EntityId entityId, EntityTypeId typeId, EntityGroupName group, object userData)
        {
            EntityId = entityId;
            TypeId = typeId;
            Group = group;
            UserData = userData;
            Status = EntityStatus.Inited;
            OnInit();
        }

        public void OnShow()
        {
            Status = EntityStatus.Showed;
            OnShowInternal();
        }

        public void OnHide()
        {
            OnHideInternal();
            Status = EntityStatus.Hidden;
        }

        public void OnUpdate(float deltaTime) => OnUpdateInternal(deltaTime);

        public void OnAttach(EntityId childId) => OnAttachInternal(childId);

        public void OnDetach(EntityId childId) => OnDetachInternal(childId);

        public void OnRecycle()
        {
            OnRecycleInternal();
            Status = EntityStatus.WillRecycle;
        }

        protected virtual void OnInit() { }
        protected virtual void OnShowInternal() { }
        protected virtual void OnHideInternal() { }
        protected virtual void OnUpdateInternal(float deltaTime) { }
        protected virtual void OnAttachInternal(EntityId childId) { }
        protected virtual void OnDetachInternal(EntityId childId) { }
        protected virtual void OnRecycleInternal() { }
    }
}
