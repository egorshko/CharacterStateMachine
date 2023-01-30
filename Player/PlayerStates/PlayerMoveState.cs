
using UnityEngine;

namespace Root
{
    internal class PlayerMoveState : PlayerState
    {
        private AnimationState _runState = AnimationState.Run;

        private bool _isMoving;
        private float _horizontalMove;

        private bool _isJump;
        private bool _isGrounded;
        private bool _isFalling;

        private bool _hasRightContact;
        private bool _hasLeftContact;


        public PlayerMoveState(PlayerStateMachine stateMachine, PlayerController playerController) : base(playerController, stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _playerController.SetAnimation(_runState, true);
           
        }
        public override void InputHandle(float deltatime)
        {
            base.InputHandle(deltatime);

            _isJump = _playerController.InputController.WasJumpPressed();

            _horizontalMove = _playerController.InputController.HorizontalInput();
        }

        public override void Update(float deltatime)
        {
            base.Update(deltatime);

            _hasRightContact = _playerController.ContactPooler.HasRightContact;
            _hasLeftContact = _playerController.ContactPooler.HasLeftContact;

            _isGrounded = _playerController.ContactPooler.isGrounded;

            _isFalling = _playerController.PlayerView._rigidbody.velocity.y < 0;

            if (_horizontalMove == 0)
            {
                SetState(_playerController.GroundedState);
            }
            
            if (_isJump && !_playerController.WasJumpReleased)
            {
                _playerController.StateMachine.ChangeState(_playerController.JumpingState);
            }

            if (!_isGrounded)
            {
                _playerController.StateMachine.ChangeState(_playerController.FallingState);
            }

            if(_horizontalMove < 0 && _hasLeftContact || _horizontalMove > 0 && _hasRightContact)
            {
                _playerController.StateMachine.ChangeState(_playerController.WallRunningState);
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
