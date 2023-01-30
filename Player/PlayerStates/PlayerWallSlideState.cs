
using UnityEngine;

namespace Root
{
    internal class PlayerWallSlideState : PlayerState
    {
        private AnimationState _wallSlideState = AnimationState.WallSlide;

        private float _horizontalInput;
        private bool _isGrounded;
        private bool _isJump;
        private bool _hasRightContact;
        private bool _hasLeftContact;
        private bool _isFallingDown;
        private bool _isFallingDownFast;

        public PlayerWallSlideState(PlayerStateMachine stateMachine, PlayerController playerController) : base(playerController, stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _playerController.SetAnimation(_wallSlideState, true);
            _playerController.ResetPlayerVelocity();
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

            _isGrounded = _playerController.ContactPooler.isGrounded;

            _isFallingDown = _playerController.PlayerView._rigidbody.velocity.y < 0;
            
            _isFallingDownFast = _playerController.PlayerView._rigidbody.velocity.y < -40;
            
            _playerController.MoveDown(deltatime, _isGrounded);

            if (_isGrounded)
            {
                _playerController.SetState(_playerController.GroundedState);
            }
            else
            {
                if (!_hasRightContact && _horizontalInput > 0 || !_hasLeftContact && _horizontalInput < 0 || !_hasLeftContact && !_hasRightContact)
                {
                    _playerController.SetState(_playerController.FallingState);
                }
            }
            
            if(_playerController.WallRunTimer > 0)
            {
                if ((_horizontalInput > 0 && _hasRightContact) || (_horizontalInput < 0 && _hasLeftContact))
                {
                    _playerController.SetState(_playerController.WallRunningState);
                }
            }          

            if (_isFallingDownFast)
            {
                if (_hasRightContact || _hasLeftContact)
                {
                    _playerController.SetState(_playerController.WallSlideFastState);
                }
            }
            else
            {
                if (_isJump && !_playerController.WasJumpReleased && _playerController.WasWallJumpPressed)
                {
                    _playerController.SetState(_playerController.WallJumpState);
                }
            }
            
            _playerController.CheckJumpRelease();
        }

        public override void Exit()
        {
            base.Exit();
            _playerController.ResetJumpTimer();
        }
    }
}
