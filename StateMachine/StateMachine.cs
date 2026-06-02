using System;

namespace Air.GameCore.StateMachine
{
    public class StateMachine
    {
        protected IStateTransition Transition;

        private State _currentState;

        protected State CurrentState => _currentState;

        private bool _isStarted;

        protected StateMachine()
        {
            Transition = new StateTransition();
        }

        protected StateMachine(State currentState) : this()
        {
            Initialize(currentState);
        }

        /// <summary>
        /// Sets the initial state and binds it to this machine. Call from subclass constructors.
        /// </summary>
        protected void Initialize(State initialState)
        {
            if (initialState == null)
            {
                throw new ArgumentNullException(nameof(initialState));
            }

            initialState.Bind(this);
            _currentState = initialState;
        }

        public void Start()
        {
            if (_currentState == null)
            {
                throw new Exception("StateMachine has no initial state set.");
            }

            if (_isStarted) return;
            _isStarted = true;
            _currentState.Enter();
        }

        public void Tick(float deltaTime)
        {
            if (!_isStarted) Start();
            _currentState.Update(deltaTime);
        }

        public void ChangeState(State from, State to)
        {
            if (to == null)
            {
                throw new ArgumentNullException(nameof(to));
            }

            to.Bind(this);
            Transition.ChangeState(from, to);
            _currentState = to;
        }

        public void ChangeState(State to) => ChangeState(_currentState, to);
    }
}
