using UnityEngine;

namespace Root
{
    internal class PlayerFactory 
    {
        private readonly PlayerData _playerData;
        
        public PlayerFactory(PlayerData playerData)
        {
            _playerData = playerData;
        }
        
        public Transform CreatePlayer()
        {
            GameObject playerPrefab = Object.Instantiate(_playerData.PlayerPrefab);

            return playerPrefab.GetComponent<Transform>();
        }
    }
}