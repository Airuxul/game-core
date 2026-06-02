namespace Air.GameCore.Command
{
    public interface IUndoable : ICommand
    {
        void Undo();
    }
}
