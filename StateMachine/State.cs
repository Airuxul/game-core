namespace Air.GameCore.StateMachine
{
    public abstract class State
    {
        protected StateMachine Machine;
        protected State ActiveState => this;

        protected State()
        {
        }

        protected State(StateMachine machine)
        {
            Bind(machine);
        }

        internal void Bind(StateMachine machine)
        {
            Machine = machine ?? throw new System.ArgumentNullException(nameof(machine));
        }

        internal virtual void Enter()
        {
            OnEnter();
        }

        internal virtual void Exit()
        {
            OnExit();
        }

        internal virtual void Update(float deltaTime)
        {
            OnUpdate(deltaTime);
        }

        protected virtual void OnEnter() { }

        protected virtual void OnUpdate(float deltaTime) { }

        protected virtual void OnExit() { }
    }
}
