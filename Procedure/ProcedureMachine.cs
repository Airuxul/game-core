using Air.GameCore.StateMachine;

namespace Air.GameCore.Procedure
{
    public sealed class ProcedureMachine : StateMachine.StateMachine
    {
        public ProcedureBase Current => CurrentState as ProcedureBase;

        public void Begin(ProcedureBase initial)
        {
            Initialize(initial);
            Start();
        }

        public void Go(ProcedureBase next)
        {
            if (next == null)
                throw new System.ArgumentNullException(nameof(next));

            if (CurrentState == null)
            {
                Begin(next);
                return;
            }

            ChangeState(next);
        }
    }
}
