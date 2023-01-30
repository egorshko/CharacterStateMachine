using UnityEngine;

namespace Root
{
    internal class PlayerGroundedState : PlayerState
    {        
        private AnimationState _idleState = AnimationState.Idle;

        private bool _isJump;
        private bool _isMoving;
        private bool _isFalling;

        private bool _isLeftMove;
        private bool _isRightMove;

        private bool _hasRightContact;
        private bool _hasLeftContact;
        private bool _isGrounded;

        public PlayerGroundedState(PlayerStateMachine playerStateMachine, 
            PlayerController playerController) : base(playerController, playerStateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _playerController.SetAnimation(_idleState, true);
            _playerController.WasWallJumpPressed = true;
            _playerController.ResetPlayerVelocity();
            _playerController.CanDoLateWallJump = true;
        }

        public override void InputHandle(float deltatime)
        {
            base.InputHandle(deltatime);

            _isJump = _playerController.InputController.WasJumpPressed();

            _isMoving = _playerController.InputController.HorizontalInput() != 0;
        }

        public override void Update(float deltatime)
        {
            base.Update(deltatime);

            _isFalling = _playerController.PlayerView._rigidbody.velocity.y < 0;

            _isGrounded = _playerController.ContactPooler.isGrounded;

            if (_isGrounded)
            {
                if (_isJump && !_playerController.WasJumpReleased)
                {
                    SetState(_playerController.JumpingState);
                }
                else
                {
                    if (_isMoving)
                    {
                        SetState(_playerController.MovingState);
                    }
                }
            }
            else
            {
                SetState(_playerController.FallingState);
            }
        }

        private void SetState(PlayerState state)
        {
            _playerController.StateMachine.ChangeState(state);
        }

        public override void Exit()
        {
            base.Exit();
            _playerController.ResetWallRunTimer();
            _playerController.ResetJumpTimer();
            _playerController.WasWalled = false;
            _playerController.WasGrounded = true;
        }

    }


}