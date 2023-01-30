
using UnityEngine;

namespace Root
{
    internal class PlayerFallState : PlayerState
    {
        private AnimationState _fallState = AnimationState.Fall;

        private float _horizontalInput;

        private bool _isGrounded;
        private bool _isJump;
        private bool _isFallingDown;
        private bool _hasRightContact;
        private bool _hasLeftContact;

        public PlayerFallState(PlayerStateMachine stateMachine, PlayerController playerController)
            : base(playerController, stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _playerController.SetAnimation(_fallState, true);
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
            _hasRightContact = _playerController.ContactPooler.HasRightContact;
            _hasLeftContact = _playerController.ContactPooler.HasLeftContact;
            _isFallingDown = _playerController.PlayerView._rigidbody.velocity.y < -40;

            _playerController.TakeJumpTimer(deltatime);
            _playerController.AddGravity(deltatime);

            if (_isGrounded)
            {
                _playerController.StateMachine.ChangeState(_playerController.GroundedState);
            }

            if (_isJump && _playerController.LateJumpTimer > 0)
            {
                if (_playerController.WasGrounded)
                {
                    _playerController.StateMachine.ChangeState(_playerController.JumpingState);
                }
                if (_playerController.WasWalled && _playerController.CanDoLateWallJump)
                {
                    _playerController.SetState(_playerController.WallJumpState);
                }
            }
            else
            {
                if (_hasRightContact && _horizontalInput > 0 || _hasLeftContact && _horizontalInput < 0)
                {
                    if (_isFallingDown)
                    {
                        _playerController.StateMachine.ChangeState(_playerController.WallSlideFastState);
                    }
                    else
                    {
                        _playerController.StateMachine.ChangeState(_playerController.WallSlideState);
                    }
                    
                }
            }

            if (_isJump && !_playerController.WasJumpReleased && _playerController.CanDoLateWallJump)
            {
                if (_playerController.ContactPooler.WallHitLeft && _horizontalInput != 0 || _playerController.ContactPooler.WallHitRight && _horizontalInput != 0)
                {
                    _playerController.StateMachine.ChangeState(_playerController.WallJumpState);
                }
            }
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}
