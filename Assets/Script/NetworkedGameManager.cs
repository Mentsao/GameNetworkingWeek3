using UnityEngine;
using Fusion;
using System.Collections.Generic;

namespace Network {

    public class NetworkedGameManager : NetworkBehaviour
    {
        private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = null;
        private NetworkSessionManager networkSessionManager;
        private int maxPlayers = 2;
        private int timerBeforeStart = 3;
        [SerializeField] private NetworkPrefabRef playerPrefab;

        public override void Spawned()
        {
            base.Spawned();
            NetworkSessionManager.Instance.OnPlayerJoinedEvent += OnPlayerJoined;
            NetworkSessionManager.Instance.OnPlayerLeftEvent += OnPlayerLeft;
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            NetworkSessionManager.Instance.OnPlayerJoinedEvent += OnPlayerJoined;
            NetworkSessionManager.Instance.OnPlayerLeftEvent += OnPlayerLeft;
        }

        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();
        }

        public override void Render()
        {
            base.Render();
        }

        private void OnPlayerJoined(PlayerRef player)
        {
            if (!HasStateAuthority) return;
            if (NetworkSessionManager.Instance.JoinedPlayers.Count >= maxPlayers)
            {
                OnGameStarted();
            }
            Debug.Log($"Player{player.PlayerId} joined");
        }

        private void OnPlayerLeft(PlayerRef player)
        {
            if (!HasStateAuthority) return;
            if (!_spawnedCharacters.TryGetValue(player, out var playerObject)) return;
            Object.Runner.Despawn(playerObject);
            _spawnedCharacters.Remove(player);
        }

        private void OnGameStarted()
        {
            foreach (var player in networkSessionManager.JoinedPlayers)
            {
                var networkObj = Object.Runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, player);
                _spawnedCharacters.Add(player, networkObj);
            }

        }
    }
}
