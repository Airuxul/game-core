using System.Collections.Generic;

namespace Air.GameCore.Entity
{
    public interface IEntityManager
    {
        int EntityCount { get; }

        EntityId ShowEntity(EntityTypeId typeId, EntityGroupName group, object userData = null);
        bool HideEntity(EntityId id);
        void HideAllLoadedEntities(EntityGroupName group);
        bool HasEntity(EntityId id);
        EntityInfo GetEntity(EntityId id);
        bool AttachEntity(EntityId childId, EntityId parentId);
        bool DetachEntity(EntityId childId);
        void Tick(float deltaTime);
        IReadOnlyList<EntityId> GetEntitiesInGroup(EntityGroupName group);
        IReadOnlyList<EntityId> GetAllEntityIds();
        void HideAllEntities();
    }
}
