using System;

namespace Air.GameCore.Entity
{
    public readonly struct EntityTypeId : IEquatable<EntityTypeId>
    {
        public static readonly EntityTypeId Invalid = default;

        public int Value { get; }

        public bool IsValid => Value > 0;

        public EntityTypeId(int value) => Value = value;

        public bool Equals(EntityTypeId other) => Value == other.Value;

        public override bool Equals(object obj) => obj is EntityTypeId other && Equals(other);

        public override int GetHashCode() => Value;

        public override string ToString() => IsValid ? $"EntityType({Value})" : "EntityType(Invalid)";

        public static bool operator ==(EntityTypeId left, EntityTypeId right) => left.Equals(right);

        public static bool operator !=(EntityTypeId left, EntityTypeId right) => !left.Equals(right);
    }
}
