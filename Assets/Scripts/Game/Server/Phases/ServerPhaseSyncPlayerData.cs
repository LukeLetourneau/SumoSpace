using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using Game.Common.Networking;
using Game.Common.Phases;
using Game.Common.Phases.PhaseData;
using Game.Common.Registry;
using UnityEngine;

namespace Game.Server.Phases
{
    public class ServerPhaseSyncPlayerData : IGamePhase
    {
        private GamePhaseNetworkManager _gamePhaseNetworkManager;

        private Dictionary<uint, PlayerID> finishedSynced = new Dictionary<uint, PlayerID>();
        public ServerPhaseSyncPlayerData(GamePhaseNetworkManager gamePhaseNetworkManager)
        {
            _gamePhaseNetworkManager = gamePhaseNetworkManager;
        }
        
        public void PhaseStart()
        {
            Debug.Log("Sending clients player data");
            PhaseSyncPlayerData.Data data = new PhaseSyncPlayerData.Data();
            
            data.playerIDs = _gamePhaseNetworkManager.masterSettings.GetPlayerIDs();

            List<PlayerStaticData> staticDataList = new List<PlayerStaticData>();
            foreach (var playerID in data.playerIDs)
            {
                _gamePhaseNetworkManager.masterSettings.playerStaticDataRegistry.TryGet(playerID, out var staticData);
                staticDataList.Add(staticData);
            }

            data.staticData = staticDataList.ToArray();
            
            data.serverUpdateInterval = _gamePhaseNetworkManager.masterSettings.network.updateInterval;
            data.friendlyFire = _gamePhaseNetworkManager.masterSettings.matchSettings.FriendlyFire;
            _gamePhaseNetworkManager.SendPhaseUpdate(Phase.MATCH_SYNC_PLAYER_DATA, PhaseSyncPlayerData.Serialized(data));
        }

        public void PhaseUpdate()
        {
            
        }

        public void PhaseCleanUp()
        {
            
        }

        public void OnUpdateReceived(RPCInfo info, byte[] data)
        {
            if (finishedSynced.ContainsKey(info.SendingPlayer.NetworkId))
            {
                Debug.LogWarning("Double update received for player sync, what happened?");
                return;
            }
            
            finishedSynced.Add(info.SendingPlayer.NetworkId, 
                _gamePhaseNetworkManager.masterSettings.playerIDRegistry.Get(info.SendingPlayer.NetworkId));
            
            Debug.Log($"Checking - {finishedSynced.Count} == {_gamePhaseNetworkManager.masterSettings.matchSettings.MaxPlayerCount}");
            
            if (finishedSynced.Count == _gamePhaseNetworkManager.masterSettings.matchSettings.MaxPlayerCount)
            {
                _gamePhaseNetworkManager.ServerNextPhase();
            }
            
        }
    }
}