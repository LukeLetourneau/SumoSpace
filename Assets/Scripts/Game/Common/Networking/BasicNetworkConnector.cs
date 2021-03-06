using System.Collections;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using Game.Common.Instances;
using Game.Common.Phases;
using Game.Common.Settings;
using Game.Common.Util;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Scene = UnityEngine.SceneManagement.Scene;

namespace Game.Common.Networking
{
    /// <summary>
    /// This is temporary class to be able to connect to a server, will be deprecated.
    /// </summary>
    public class BasicNetworkConnector : MonoBehaviour
    {
    
        public GameObject networkManagerPrefab;
        
        public GameMatchSettings gameMatchSettings;

        public delegate void FailedToConnectEvent();

        public FailedToConnectEvent OnFailedToConnect;
        
        private UDPClient _gameClient;
        private UDPServer _gameServer;
    
        // Start is called before the first frame update
        void Awake()
        {
            //DontDestroyOnLoad(this.gameObject);
        }

        // Update is called once per frame
        void Update()
        {
            return;
            /*
            if (NetworkManager.Instance != null && !NetworkManager.Instance.IsServer)
            {
                // ReSharper disable once ReplaceWithSingleAssignment.True
                bool allSystemsSet = true;
                
                allSystemsSet &= MainInstances.Get<GamePhaseNetworkManager>() == null;
                allSystemsSet &= MainInstances.Get<AgentManager>() == null;
                allSystemsSet &= MainInstances.Get<GameNetworkManager>() == null;
                
                if (allSystemsSet)
                {
                    SceneManager.LoadScene(1);
                }

            }*/
        }


        public float timeout = 30;
        public IEnumerator TimeoutWait()
        {
            yield return new WaitForSecondsRealtime(timeout);

            if (!_gameClient.IsConnected)
            {
                Disconnect();
            }
            timeoutWaitCor = null;
        }

        public Coroutine timeoutWaitCor;
    
        public void Connect(string address, ushort port)
        {
            if (NetworkManager.Instance != null)
            {
                Debug.Log("Stopping last connection");
                NetworkManager.Instance.Disconnect();
                if (timeoutWaitCor != null)
                {
                    StopCoroutine(timeoutWaitCor);
                    timeoutWaitCor = null;
                }
            }
            
            Debug.Log("Connecting to game server.. " + address + " " + port);
    
            gameMatchSettings.Reset();

            _gameClient = new UDPClient();

            _gameClient.serverAccepted += (sender) =>
            {
                Debug.Log("Player Connected!");
                MainThreadManager.Run(() =>
                {
                    if (timeoutWaitCor != null)
                    {
                        StopCoroutine(timeoutWaitCor);
                        timeoutWaitCor = null;
                    }
                });
                
                NetworkObject.Flush(_gameClient);
            };

            
            //_gameClient.connectAttemptFailed += (sender) => Debug.Log("Connection Failed!");
            
            _gameClient.Connect(address, port);
            timeoutWaitCor = StartCoroutine(TimeoutWait());

            //If network manager does not exist, make sure to spawn it in. 
            if (NetworkManager.Instance == null)
            {
                Instantiate(networkManagerPrefab);
            }
        
            Rpc.MainThreadRunner = MainThreadManager.Instance;
        
            NetworkManager.Instance.Initialize(_gameClient);
        }

        public void Disconnect(bool forced = true)
        {
            if (_gameClient == null) return;
            NetworkManager.Instance.Disconnect();
            Debug.LogError("Failed to connect to server");
            OnFailedToConnect?.Invoke();
        }
        
        public void Host(string address, ushort port)
        {
            Debug.Log("Starting game server.. " + address + " : " + port);
        
            _gameServer = new UDPServer(gameMatchSettings.MaxPlayerCount + 1);
            
            
            _gameServer.playerConnected += (player, sender) => Debug.Log("Player connected into server!");
            _gameServer.playerAccepted += (player, sender) => Debug.Log("Player accepted into server!");
            _gameServer.playerDisconnected += (player, sender) => Debug.Log("Player disconnected from server!");


            _gameServer.Connect(address, port);
            
            //If network manager does not exist, make sure to spawn it in. 
            if (NetworkManager.Instance == null)
            {
                Instantiate(networkManagerPrefab);
            }
        
            Rpc.MainThreadRunner = MainThreadManager.Instance;
        
            NetworkManager.Instance.Initialize(_gameServer);
            NetworkObject.Flush(_gameServer);
            Debug.Log("Loading new scene");
            //SceneManager.LoadScene(1);
            //SceneManager.sceneLoaded += NetworkInstantiate;
            //Debug.Log("Instantiating required objects");
            
            PersistantUtility.InstantiateNetworkPersistant();
        }

    }
}
