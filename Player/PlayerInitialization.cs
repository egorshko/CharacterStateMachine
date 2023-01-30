using UnityEngine;

namespace Root
{
    internal class PlayerInitialization : IInitialization
    {
        private readonly PlayerFactory _playerFactory;
        private Transform _player;
        private PlayerView _view;

        public PlayerInitialization(PlayerFactory playerFactory, Vector3 positionPlayer)
        {
            _playerFactory = playerFactory;
            _player = _playerFactory.CreatePlayer();
            _player.position = positionPlayer;
            
        }
        public void Initialization()
        {
            
        }

        public Transform GetPlayer()
        {
            return _player;
        }

        public PlayerView GetView()
        {
            return _player.gameObject.GetComponent<PlayerView>();
        }

    }
}