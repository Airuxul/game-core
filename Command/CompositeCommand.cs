using System;
using System.Collections.Generic;

namespace Air.GameCore.Command
{
    /// <summary>Macro command: Execute/Undo children in order (Undo reverses).</summary>
    public sealed class CompositeCommand : IUndoable
    {
        readonly IReadOnlyList<IUndoable> _children;
        readonly string _name;

        public CompositeCommand(string name, IReadOnlyList<IUndoable> children)
        {
            _name = name ?? "composite";
            _children = children ?? throw new ArgumentNullException(nameof(children));
        }

        public void Execute()
        {
            foreach (var child in _children)
                child.Execute();
        }

        public void Undo()
        {
            for (var i = _children.Count - 1; i >= 0; i--)
                _children[i].Undo();
        }

        public override string ToString() => _name;
    }
}
