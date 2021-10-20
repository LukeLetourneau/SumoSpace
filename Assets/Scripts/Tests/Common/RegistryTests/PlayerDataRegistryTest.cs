using System.Collections;
using Game.Common.Registry;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.RegistryTests
{
    public class PlayerDataRegistryTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void PlayerDataRegistryTestSimplePasses()
        {
            //Initialization
            var instance = ScriptableObject.CreateInstance<PlayerStaticDataRegistry>();
            var idRegistry = ScriptableObject.CreateInstance<PlayerIDRegistry>();

            idRegistry.RegisterPlayer(0);
            idRegistry.RegisterPlayer(1);
            idRegistry.RegisterPlayer(2);

            var playerOne = idRegistry.Get(0);
            var playerTwo = idRegistry.Get(1);
            var playerThree = idRegistry.Get(2);
            
            //Testing
            
            PlayerStaticData staticData;
            bool success;
            
            Assert.IsFalse(instance.TryGet(playerOne, out staticData));
            Assert.IsFalse(instance.TryGet(playerTwo, out staticData));
            Assert.IsFalse(instance.TryGet(playerThree, out staticData));
            
            success = instance.Add(playerOne, new PlayerStaticData()
            {
                PlayerMatchID = 100
            });
            
            Assert.IsTrue(success);
            
            instance.TryGet(playerOne, out staticData);
            Assert.IsTrue(staticData.PlayerMatchID == 100);
            Assert.IsFalse(instance.TryGet(playerTwo, out staticData));
            Assert.IsFalse(instance.TryGet(playerThree, out staticData));

            
            success = instance.Add(playerTwo, new PlayerStaticData()
            {
                PlayerMatchID = 200
            });
            
            Assert.IsTrue(success);
            
            Assert.IsTrue(instance.TryGet(playerOne, out staticData));
            instance.TryGet(playerTwo, out staticData);
            Assert.IsTrue(staticData.PlayerMatchID == 200);
            Assert.IsFalse(instance.TryGet(playerThree, out staticData));
            
            
            success = instance.Add(playerThree, new PlayerStaticData()
            {
                PlayerMatchID = 300
            });
            
            Assert.IsTrue(success);
            
            Assert.IsTrue(instance.TryGet(playerOne, out staticData));
            Assert.IsTrue(instance.TryGet(playerTwo, out staticData));
            instance.TryGet(playerThree, out staticData);
            Assert.IsTrue(staticData.PlayerMatchID == 300);
            
            
            Assert.IsFalse(instance.Add(playerOne, new PlayerStaticData()));
            Assert.IsFalse(instance.Add(playerTwo, new PlayerStaticData()));
            Assert.IsFalse(instance.Add(playerThree, new PlayerStaticData()));
        }

    }
}
