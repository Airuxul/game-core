using System;

namespace Air.GameCore.Entity
{
  public readonly struct EntityId : IEquatable<EntityId>
  {
    public static readonly EntityId Invalid = default;

    public int Value { get; }

    public bool IsValid => Value > 0;

    public EntityId(int value) => Value = value;

    public bool Equals(EntityId other) => Value == other.Value;

    public override bool Equals(object obj) => obj is EntityId other && Equals(other);

    public override int GetHashCode() => Value;

    public override string ToString() => IsValid ? $"Entity({Value})" : "Entity(Invalid)";

    public static bool operator ==(EntityId left, EntityId right) => left.Equals(right);

    public static bool operator !=(EntityId left, EntityId right) => !left.Equals(right);
  }
}
