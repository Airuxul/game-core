using System;
using System.Collections.Generic;

namespace Air.GameCore.Command
{
    /// <summary>Undo / redo stacks for <see cref="IUndoable"/>.</summary>
    public sealed class CommandHistory
    {
        readonly Stack<IUndoable> _undo = new();
        readonly Stack<IUndoable> _redo = new();
        readonly int _maxDepth;
        readonly bool _executeOnDo;

        public CommandHistory(int maxDepth = 128, bool executeOnDo = true)
        {
            if (maxDepth < 1)
                throw new ArgumentOutOfRangeException(nameof(maxDepth));
            _maxDepth = maxDepth;
            _executeOnDo = executeOnDo;
        }

        public bool CanUndo => _undo.Count > 0;
        public bool CanRedo => _redo.Count > 0;

        /// <summary>Execute and push onto the undo stack.</summary>
        public void Do(IUndoable command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (_executeOnDo)
                command.Execute();

            _redo.Clear();
            _undo.Push(command);
            Trim(_undo);
        }

        /// <summary>Execute; undoable commands are recorded, others run once.</summary>
        public void Run(ICommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (command is IUndoable undoable)
            {
                Do(undoable);
                return;
            }

            command.Execute();
        }

        public void Undo()
        {
            if (!CanUndo)
                return;

            var command = _undo.Pop();
            command.Undo();
            _redo.Push(command);
            Trim(_redo);
        }

        public void Redo()
        {
            if (!CanRedo)
                return;

            var command = _redo.Pop();
            if (_executeOnDo)
                command.Execute();
            _undo.Push(command);
            Trim(_undo);
        }

        public void Clear()
        {
            _undo.Clear();
            _redo.Clear();
        }

        void Trim(Stack<IUndoable> stack)
        {
            if (stack.Count <= _maxDepth)
                return;

            var list = new List<IUndoable>(stack.Count);
            while (stack.Count > 0)
                list.Add(stack.Pop());
            list.Reverse();
            var skip = list.Count - _maxDepth;
            for (var i = skip; i < list.Count; i++)
                stack.Push(list[i]);
        }
    }
}
