using UnityEngine;

namespace Root
{
    internal class PlayerWallRunning : PlayerState
    {
        private AnimationState _wallRunState = AnimationState.WallRun;

        private float _horizontalInput;
        private bool _isJump;
        private bool _isGrounded;
        private bool _hasRightContact;
        private bool _hasLeftContact;
        private bool _isFallingDown;
        public PlayerWallRunning(PlayerStateMachine stateMachine, PlayerController playerController) : base(playerController, stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _playerController.SetAnimation(_wallRunState, true);
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
            //base.Update(deltatime);

            _playerController.MoveUp(deltatime);
            _playerController.TakeWallRunTimer(deltatime);

            _hasRightContact = _playerController.ContactPooler.HasRightContact;

            _hasLeftContact = _playerController.ContactPooler.HasLeftContact;

            _isFallingDown = _playerController.PlayerView._rigidbody.velocity.y < 0;

            _isGrounded = _playerController.ContactPooler.isGrounded;

            if (_horizontalInput == 0 || _playerController.WallRunTimer <= 0 || !_hasRightContact && !_hasLeftContact)
            {
                _playerController.StateMachine.ChangeState(_playerController.WallSlideState);
            }

            if (_isJump && _playerController.WasWallJumpPressed && !_playerController.WasJumpReleased)
            {
                _playerController.StateMachine.ChangeState(_playerController.WallJumpState);
            }
            
            _playerController.CheckJumpRelease();
        }

        public override void Exit()
        {
            base.Exit();
            _playerController.EquateWallRunTimerToZero();
            _playerController.WasWalled = true;
            _playerController.WasGrounded = false;
        }

    }
}
