using UnityEngine;

namespace Root {
    internal class PlayerJumpState : PlayerState
    {
        private AnimationState _jumpState = AnimationState.Jump;

        private float _horizontalInput;

        private bool _isGrounded;

        private bool _hasRightContact;
        private bool _hasLeftContact;

        private bool _isFalling;
        private bool _isJump;

        public PlayerJumpState(PlayerStateMachine stateMachine, PlayerController playerController)
            : base (playerController, stateMachine)
        {

        }

        public override void Enter()
        {
            base.Enter();
            _playerController.SetAnimation(_jumpState, false);
            _playerController.Jump();
            _playerController.WasJumpReleased = true;
            _playerController.EquateJumpTimerToZero();
        }

        public override void InputHandle(float deltatime)
        {
            base.InputHandle(deltatime);
            _horizontalInput = _playerController.InputController.HorizontalInput();
            _isJump = _playerController.InputController.WasJumpPressed();
        }

        public override void Update(float deltatime)
        {
            base.Update(deltatime);

            _isGrounded = _playerController.ContactPooler.isGrounded;

            _playerController.AddGravity(deltatime);

            _hasRightContact = _playerController.ContactPooler.HasRightContact;
            _hasLeftContact = _playerController.ContactPooler.HasLeftContact;

            _isFalling = _playerController.PlayerView._rigidbody.velocity.y < 0;

            if(_horizontalInput != 0)
            {
                if (_hasRightContact && !_isGrounded || _hasLeftContact && !_isGrounded)
                {
                    if (_playerController.WallRunTimer > 0)
                    {
                        _playerController.StateMachine.ChangeState(_playerController.WallRunningState);
                    }
                }
            }

            if(_isJump && !_playerController.WasJumpReleased)
            {
                if(_playerController.ContactPooler.WallHitLeft && _horizontalInput!=0 || _playerController.ContactPooler.WallHitRight && _horizontalInput !=0)
                {
                    _playerController.StateMachine.ChangeState(_playerController.WallJumpState);
                }
            }

            if (_isFalling)
            {
                _playerController.StateMachine.ChangeState(_playerController.FallingState);
            }
        }

        private void SetState(PlayerState state)
        {
            _playerController.StateMachine.ChangeState(state);
        }

        public override void Exit()
        {
            base.Exit();
        }

    }
}
