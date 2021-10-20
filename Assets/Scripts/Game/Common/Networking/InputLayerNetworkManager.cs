using System;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using Game.Common.Gameplay.Commands;
using Game.Common.Instances;
using Game.Common.Networking.Commands;
using Game.Common.Settings;
using UnityEngine;

namespace Game.Common.Networking
{
    public partial class InputLayerNetworkManager : InputLayerBehavior, IGamePersistantInstance
    {

        public MasterSettings masterSettings;

        private CommandHandlerNetworkManager _commandHandlerNetworkManager;
        
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        protected override void NetworkStart()
        {
            base.NetworkStart();

            MainThreadManager.Run(()=>{ 
                MainPersistantInstances.TryAdd(this);
            });
            
            if (networkObject.IsServer)
            {
                ServerStart();
            }
            else
            {
                ClientStart();
            }
            
            _commandHandlerNetworkManager = new CommandHandlerNetworkManager(networkObject, RPC_COMMAND_UPDATE, masterSettings);
        }
        


        /// <summary>
        /// Whenever the client wants to activate a command, it sends it to the server
        /// </summary>
        /// <param name="args"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override void CommandUpdate(RpcArgs args)
        {
            if (!networkObject.IsServer && !args.Info.SendingPlayer.IsHost)
            {
                Debug.LogError("Received invalid RPC from other client. Clients should only receive server data");
                return;
            }
            
            _commandHandlerNetworkManager.HandleRPC(args);
        }

       
        /// <summary>
        /// Server receives movement updates, so it can update the ships
        /// </summary>
        /// <param name="args"></param>
        public override void MovementUpdate(RpcArgs args)
        {
            if (networkObject.IsServer)
            {
                
                ServerMovementUpdate(args);
            }
            else
            {
                Debug.LogError("Client received movement update through input layer, this should not be the case");
                return;
            }
        }

        private void OnDestroy()
        {
            MainPersistantInstances.Remove<InputLayerNetworkManager>();
        }

        partial void ServerMovementUpdate(RpcArgs args);

        partial void ServerStart();
        partial void ClientStart();
    }
}
