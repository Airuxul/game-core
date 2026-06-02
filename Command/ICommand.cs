namespace Air.GameCore.Command
{
    /// <summary>Canonical command: Execute (and optional Undo via <see cref="IUndoable"/>).</summary>
    public interface ICommand
    {
        void Execute();
    }
}
