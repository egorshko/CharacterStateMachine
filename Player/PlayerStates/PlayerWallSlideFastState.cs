using UnityEngine;

namespace Root
{
    internal class PlayerWallSlideFastState : PlayerState
    {
        private AnimationState _wallSlideFastState = AnimationState.WallSlideFast;

        private float _horizontalInput;
        private bool _isGrounded;
        private bool _hasRightContact;
        private bool _hasLeftContact;
        private bool _isFallingDown;
        private bool _isFallingDownSlow;

        private float _proxyHorizontalInput;

        public PlayerWallSlideFastState(PlayerStateMachine stateMachine, PlayerController playerController) : base(playerController, stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _playerController.SetAnimation(_wallSlideFastState, true);
        }

        public override void InputHandle(float deltatime)
        {
            base.InputHandle(deltatime);
            _horizontalInput = _playerController.InputController.HorizontalInput();
        }

        public override void Update(float deltatime)
        {
            _isFallingDownSlow = _playerController.PlayerView._rigidbody.velocity.y > -30;

            _isGrounded = _playerController.ContactPooler.isGrounded;

            _playerController.MoveDown(deltatime, true);

            _hasRightContact = _playerController.ContactPooler.HasRightContact;
            _hasLeftContact = _playerController.ContactPooler.HasLeftContact;
            
            if (!_hasRightContact && !_hasLeftContact)
            {
                _playerController.StateMachine.ChangeState(_playerController.FallingState);
            }

            if (_horizontalInput > 0 && _hasRightContact || _horizontalInput < 0 && _hasLeftContact)
            {
                _proxyHorizontalInput = 0;
            }
            else
            {
                _proxyHorizontalInput = _horizontalInput;
            }

            _playerController.Move(deltatime, _proxyHorizontalInput);

            if (_isGrounded)
            {
                _playerController.StateMachine.ChangeState(_playerController.GroundedState);
            }
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}
