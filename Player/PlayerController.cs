using UnityEngine;

namespace Root
{
    internal class PlayerController : IExecute
    {
        public PlayerStateMachine StateMachine { get; private set; }
        public PlayerGroundedState GroundedState { get; private set; }
        public PlayerMoveState MovingState { get; private set; }
        public PlayerJumpState JumpingState { get; private set; }
        public PlayerFallState FallingState { get; private set; }
        public PlayerWallRunning WallRunningState { get; private set; }
        public PlayerWallSlideState WallSlideState { get; private set; }
        public PlayerWallLeaningState WallLeaningState { get; private set; }
        public PlayerWallJumpState WallJumpState { get; private set; }
        public PlayerWallSlideFastState WallSlideFastState { get; private set; }

        private readonly SpriteAnimation _animator;
        private readonly PlayerData _playerData;
        private readonly PlayerView _playerView;
        private readonly ContactPooler _contactPooler;
        private readonly NewInputController _inputController;
        
        public ContactPooler ContactPooler => _contactPooler;
        public NewInputController InputController => _inputController;
        public PlayerView PlayerView => _playerView;
        public PlayerData PlayerData => _playerData;

        private Transform _player;
        private Vector3 _jump;
        private Vector3 _move;
        private Vector3 _moveUp;
        private Vector3 _moveDown;

        private bool _wasJumpReleased = true;
        private bool _isTryingToJump;
        private float _lateJumpTimer = 0.4f;
        private float _totalJumpTime;
        private float _maxTimeTimer = 1f;
        private bool _canDoLateWallJump;
        private bool _wasGounded;
        private bool _wasWalled;

        public bool WasGrounded { get { return _wasGounded; } set { _wasGounded = value; } }
        public bool WasWalled { get { return _wasWalled; } set { _wasWalled = value; } }

        public bool CanDoLateWallJump { get { return _canDoLateWallJump; } set { _canDoLateWallJump = value; } }
        public bool WasJumpReleased   {  get { return _wasJumpReleased; }  set { _wasJumpReleased = value; }  }
        public float LateJumpTimer { get { return _lateJumpTimer; } set { _lateJumpTimer = value; } }
        public bool WasWallJumpPressed { get; set; }
        public float WallRunTimer { get; set; }
        
        public PlayerController(Transform player, PlayerData playerData, PlayerView playerView, NewInputController inputController)
        {
            _playerData = playerData;
            _playerView = playerView;
            _player = player;

            _animator = new SpriteAnimation(_playerData);
            _contactPooler = new ContactPooler(_playerView._collider);

            _inputController = inputController;

            StateMachine = new PlayerStateMachine();

            GroundedState = new PlayerGroundedState(StateMachine, this);
            JumpingState = new PlayerJumpState(StateMachine, this);
            MovingState = new PlayerMoveState(StateMachine, this);
            FallingState = new PlayerFallState(StateMachine, this);
            WallRunningState = new PlayerWallRunning(StateMachine, this);
            WallSlideState = new PlayerWallSlideState(StateMachine, this);
            WallSlideFastState = new PlayerWallSlideFastState(StateMachine, this);
            WallLeaningState = new PlayerWallLeaningState(StateMachine, this);
            WallJumpState = new PlayerWallJumpState(StateMachine, this);

            StateMachine.Init(GroundedState);
        }

        public void Execute(float deltatime)
        {
            StateMachine.CurrentState.Update(deltatime);
            StateMachine.CurrentState.InputHandle(deltatime);

            _animator.Execute(deltatime);

            _contactPooler.CheckContact();
        }

        public void CheckJumpRelease()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _wasJumpReleased = false;
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                _wasJumpReleased = true;
            }
        }

        public void Move(float deltatime, float horizontalInput)
        {
            var speed = deltatime * _playerData.WalkSpeed;
            
            _move.Set(horizontalInput * speed, 0.0f, 0.0f);
            
            _player.localPosition += _move;
        }

        public void MoveUp(float deltatime)
        {
            var speed = deltatime * _playerData.WallRunSpeed;

            _moveUp.Set(0.0f, speed*0.5f , 0.0f);

            _player.localPosition += _moveUp;
        }

        public void ResetPlayerVelocity()
        {
            _playerView._rigidbody.velocity = Vector3.zero;
        }

        public void MoveDown(float deltatime, bool isGrounded)
        {
            Vector2 gravity = Vector2.up * Physics2D.gravity.y;

            _playerView._rigidbody.velocity += (gravity * _playerData.WallSlideSpeedMultiplier * deltatime);
        }

        public void Jump()
        {
            _playerView._rigidbody.velocity = new Vector2(_playerView._rigidbody.velocity.x, _playerData.JumpForce);
        }

        public void WallJump()
        {
            var diff = _playerData.WallJumpForce;

            if (_contactPooler.HasRightContact)
            {
                diff = diff * -1;
            }
            else
            {
                diff = _playerData.WallJumpForce;
            }

            _playerView._rigidbody.velocity =
                new Vector2(0, _playerData.JumpForce);
        }

        public void CheckJumpInputAndRelease(bool buttonDown, bool buttonUp)
        {
            if (buttonDown)
            {
                _wasJumpReleased = false;
            }
            if (buttonUp)
            {
                _wasJumpReleased = true;
            }
        }

        public void TakeJumpTimer(float deltatime)
        {
            _lateJumpTimer -= deltatime;
        }

        public void ResetJumpTimer()
        {
            _lateJumpTimer = _playerData.TimeForLateJump;
        }

        public void EquateJumpTimerToZero()
        {
            _lateJumpTimer = 0;
        }

        public void ResetWallRunTimer()
        {
            WallRunTimer = _maxTimeTimer;
        }

        public void TakeWallRunTimer(float deltatime)
        {
            WallRunTimer -= deltatime;
        }

        public void EquateWallRunTimerToZero()
        {
            WallRunTimer = 0;
        }

        public void AddGravity(float deltatime)
        {
            Vector2 gravity = Vector2.up * Physics2D.gravity.y;

            if (_playerView._rigidbody.velocity.y <= 0)
            {
                _playerView._rigidbody.velocity += gravity * deltatime * _playerData.FallMultiplier;
            }
            else if (_playerView._rigidbody.velocity.y > 0)
            {
                _playerView._rigidbody.velocity += gravity * _playerData.GravityMultiplier * deltatime;
            }
        }

        public void WallRunMultiply(float deltatime, bool isGrounded)
        {
            Vector2 gravity = Vector2.up * Physics2D.gravity.y;

            if (_playerView._rigidbody.velocity.y < 0)
            {
                _playerView._rigidbody.velocity += gravity * deltatime * _playerData.WallRunMultiplier;
            }
            else if (_playerView._rigidbody.velocity.y > 0 && !isGrounded)
            {
                _playerView._rigidbody.velocity += gravity * _playerData.WallFallingMultiplier * deltatime;
            }
        }

        public void SetAnimation(AnimationState state, bool isLoop)
        {
            _animator.StartAnimation(_playerView._spriteRenderer, state, isLoop, _playerData.AnimationSpeed);
        }
        
        public void SetState(PlayerState state)
        {
            StateMachine.ChangeState(state);
        }
    }
}
