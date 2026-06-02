namespace Air.GameCore.Entity
{
    public interface IEntityLogic
    {
        EntityId EntityId { get; }
        EntityTypeId TypeId { get; }
        EntityGroupName Group { get; }
        EntityStatus Status { get; }
        object UserData { get; }

        void OnInit(EntityId entityId, EntityTypeId typeId, EntityGroupName group, object userData);
        void OnShow();
        void OnHide();
        void OnUpdate(float deltaTime);
        void OnAttach(EntityId childId);
        void OnDetach(EntityId childId);
        void OnRecycle();
    }
}
