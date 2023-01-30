
using UnityEngine;

namespace Root
{
    internal class PlayerWallLeaningState : PlayerState
    {
        private AnimationState _wallLeaningState = AnimationState.WallLeaning;

        private float _horizontalInput;
        private bool _isJump;
        private bool _isGrounded;
        private bool _hasRightContact;
        private bool _hasLeftContact;
        private bool _isFallingDown;
        private bool _isFallingDownFast;

        public PlayerWallLeaningState(PlayerStateMachine stateMachine, PlayerController playerController) : base(playerController, stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _playerController.SetAnimation(_wallLeaningState, true);
        }

        public override void InputHandle(float deltatime)
        {
            base.InputHandle(deltatime);
            _horizontalInput = _playerController.InputController.HorizontalInput();
            _isJump = _playerController.InputController.WasJumpPressed();
        }

        public override void Update(float deltatime)
        {
            _hasRightContact = _playerController.ContactPooler.HasRightContact;
            _hasLeftContact = _playerController.ContactPooler.HasLeftContact;

            _playerController.WallRunMultiply(deltatime, false);

            _isGrounded = _playerController.ContactPooler.isGrounded;

            _isFallingDown = _playerController.PlayerView._rigidbody.velocity.y < 0;

            _isFallingDownFast = _playerController.PlayerView._rigidbody.velocity.y < -40;

            if (_isJump)
            {
                _playerController.StateMachine.ChangeState(_playerController.JumpingState);
            }

            if (_isGrounded)
            {
                if(!_hasRightContact && !_hasLeftContact || _horizontalInput == 0)
                {
                    _playerController.StateMachine.ChangeState(_playerController.GroundedState);
                }
            }
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}
