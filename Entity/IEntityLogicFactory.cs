namespace Air.GameCore.Entity
{
    public interface IEntityLogicFactory
    {
        IEntityLogic Create(EntityTypeId typeId);
    }
}
