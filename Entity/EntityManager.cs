using System;
using System.Collections.Generic;

namespace Air.GameCore.Entity
{
    public sealed class EntityManager : IEntityManager
    {
        readonly Dictionary<EntityId, EntityInfo> _entities = new();
        readonly Dictionary<string, EntityGroup> _groups = new(StringComparer.Ordinal);
        readonly IEntityLogicFactory _factory;
        int _nextId = 1;

        public EntityManager(IEntityLogicFactory factory) =>
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));

        public int EntityCount => _entities.Count;

        public EntityId ShowEntity(EntityTypeId typeId, EntityGroupName group, object userData = null)
        {
            if (!typeId.IsValid)
                throw new ArgumentException("Entity type id is invalid.", nameof(typeId));
            if (!group.IsValid)
                throw new ArgumentException("Entity group is invalid.", nameof(group));

            var entityGroup = GetOrCreateGroup(group);
            if (entityGroup.Paused)
                throw new InvalidOperationException($"Entity group '{group}' is paused.");

            var countInGroup = 0;
            foreach (var pair in _entities)
            {
                if (pair.Value.Group == group && pair.Value.Status == EntityStatus.Showed)
                    countInGroup++;
            }

            if (countInGroup >= entityGroup.InstanceCapacity)
                throw new InvalidOperationException($"Entity group '{group}' is at capacity.");

            var id = new EntityId(_nextId++);
            var logic = _factory.Create(typeId);
            var info = new EntityInfo
            {
                Id = id,
                TypeId = typeId,
                Group = group,
                Status = EntityStatus.WillShow,
                Logic = logic,
                UserData = userData,
                ParentId = EntityId.Invalid,
            };
            _entities[id] = info;

            logic.OnInit(id, typeId, group, userData);
            info.Status = EntityStatus.Inited;
            logic.OnShow();
            info.Status = EntityStatus.Showed;
            return id;
        }

        public bool HideEntity(EntityId id)
        {
            if (!TryGet(id, out var info))
                return false;

            if (info.Status is EntityStatus.Hidden or EntityStatus.WillRecycle)
                return false;

            info.Status = EntityStatus.WillHide;
            info.Logic?.OnHide();
            info.Status = EntityStatus.Hidden;
            info.Logic?.OnRecycle();
            info.Status = EntityStatus.WillRecycle;
            _entities.Remove(id);
            return true;
        }

        public void HideAllLoadedEntities(EntityGroupName group)
        {
            var toHide = new List<EntityId>();
            foreach (var pair in _entities)
            {
                if (pair.Value.Group == group)
                    toHide.Add(pair.Key);
            }

            for (var i = toHide.Count - 1; i >= 0; i--)
                HideEntity(toHide[i]);
        }

        public bool HasEntity(EntityId id) => id.IsValid && _entities.ContainsKey(id);

        public EntityInfo GetEntity(EntityId id) =>
            TryGet(id, out var info) ? info : null;

        public bool AttachEntity(EntityId childId, EntityId parentId)
        {
            if (!TryGet(childId, out var child) || !TryGet(parentId, out var parent))
                return false;

            child.ParentId = parentId;
            parent.Logic?.OnAttach(childId);
            return true;
        }

        public bool DetachEntity(EntityId childId)
        {
            if (!TryGet(childId, out var child) || !child.ParentId.IsValid)
                return false;

            if (TryGet(child.ParentId, out var parent))
                parent.Logic?.OnDetach(childId);

            child.Logic?.OnDetach(child.ParentId);
            child.ParentId = EntityId.Invalid;
            return true;
        }

        public void Tick(float deltaTime)
        {
            foreach (var pair in _entities)
            {
                var info = pair.Value;
                if (info.Status != EntityStatus.Showed)
                    continue;

                var group = GetOrCreateGroup(info.Group);
                if (group.Paused)
                    continue;

                info.Logic?.OnUpdate(deltaTime);
            }
        }

        public IReadOnlyList<EntityId> GetEntitiesInGroup(EntityGroupName group)
        {
            var list = new List<EntityId>();
            foreach (var pair in _entities)
            {
                if (pair.Value.Group == group)
                    list.Add(pair.Key);
            }

            return list;
        }

        public IReadOnlyList<EntityId> GetAllEntityIds() => new List<EntityId>(_entities.Keys);

        public void HideAllEntities()
        {
            var toHide = new List<EntityId>(_entities.Keys);
            for (var i = toHide.Count - 1; i >= 0; i--)
                HideEntity(toHide[i]);
        }

        EntityGroup GetOrCreateGroup(EntityGroupName group)
        {
            if (!_groups.TryGetValue(group.Value, out var entityGroup))
            {
                entityGroup = new EntityGroup(group);
                _groups[group.Value] = entityGroup;
            }

            return entityGroup;
        }

        bool TryGet(EntityId id, out EntityInfo info)
        {
            info = null;
            if (!id.IsValid)
                return false;
            return _entities.TryGetValue(id, out info);
        }
    }
}
