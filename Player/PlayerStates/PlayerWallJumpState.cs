
using UnityEngine;

namespace Root
{
    internal class PlayerWallJumpState : PlayerState
    {
        private AnimationState _wallJumpState = AnimationState.WallJump;

        private float _horizontalInput;
        private float _proxyHorizontalInput;
        private bool _isGrounded;
        private bool _hasRightContact;
        private bool _hasLeftContact;
        private bool _isFalling;
        private bool _isFallingDownSlow;

        public PlayerWallJumpState(PlayerStateMachine stateMachine, PlayerController playerController) : base(playerController, stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _playerController.SetAnimation(_wallJumpState, false);
            _playerController.WallJump();
            _playerController.WasWallJumpPressed = false;
            _playerController.CanDoLateWallJump = false;
        }

        public override void InputHandle(float deltatime)
        {
            _horizontalInput = _playerController.InputController.HorizontalInput();
            base.InputHandle(deltatime);
        }

        public override void Update(float deltatime)
        {

            _isGrounded = _playerController.ContactPooler.isGrounded;

            _playerController.AddGravity(deltatime);

            _hasRightContact = _playerController.ContactPooler.HasRightContact;
            _hasLeftContact = _playerController.ContactPooler.HasLeftContact;

            _isFalling = _playerController.PlayerView._rigidbody.velocity.y < 0;
            
            if(_horizontalInput > 0 && _hasRightContact || _horizontalInput<0 && _hasLeftContact)
            {
                _proxyHorizontalInput = 0;
            }
            else
            {
                _proxyHorizontalInput = _horizontalInput;
            }

            _playerController.Move(deltatime, _proxyHorizontalInput);

            if (_isFalling)
            {
                _playerController.StateMachine.ChangeState(_playerController.FallingState);
            }
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}
