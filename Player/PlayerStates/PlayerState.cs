using UnityEngine;

namespace Root
{
    internal abstract class PlayerState
    {
        protected PlayerController _playerController;
        protected PlayerStateMachine _stateMachine;

        private float _horizontalInput;
        private bool _isGrounded;
        

        public PlayerState(PlayerController playerController, PlayerStateMachine stateMachine)
        {
            _playerController = playerController;
            _stateMachine = stateMachine;
        }
        public virtual void Enter()
        {

        }

        public virtual void InputHandle(float deltatime)
        {
            _horizontalInput = _playerController.InputController.HorizontalInput();
        }

        public virtual void Update(float deltatime)
        {
            var contactPooler = _playerController.ContactPooler;
            
            _isGrounded = contactPooler.isGrounded;

            _playerController.Move(deltatime, _horizontalInput);

            _playerController.CheckJumpInputAndRelease(Input.GetKeyDown(KeyCode.Space), Input.GetKeyUp(KeyCode.Space));

            FlipRenderer(_horizontalInput < 0);
        }

        public virtual void Exit()
        {

        }

        private void FlipRenderer(bool isFlip)
        {
            if (_horizontalInput != 0)
                _playerController.PlayerView._spriteRenderer.flipX = isFlip;
        }
    }
}
