using System;

namespace Air.GameCore.Entity
{
    public readonly struct EntityGroupName : IEquatable<EntityGroupName>
    {
        public string Value { get; }

        public bool IsValid => !string.IsNullOrEmpty(Value);

        public EntityGroupName(string value) => Value = value ?? "";

        public bool Equals(EntityGroupName other) =>
            string.Equals(Value, other.Value, StringComparison.Ordinal);

        public override bool Equals(object obj) => obj is EntityGroupName other && Equals(other);

        public override int GetHashCode() =>
            Value != null ? StringComparer.Ordinal.GetHashCode(Value) : 0;

        public override string ToString() => Value ?? "";

        public static bool operator ==(EntityGroupName left, EntityGroupName right) => left.Equals(right);

        public static bool operator !=(EntityGroupName left, EntityGroupName right) => !left.Equals(right);
    }
}
