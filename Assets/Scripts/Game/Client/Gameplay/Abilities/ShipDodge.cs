using UnityEngine;

namespace Game.Client.Gameplay.Abilities
{
    [CreateAssetMenu(menuName = "Ship Abilities/Ship Dodge")]
    public class ShipDodge : ClientShipAbility {

        /*public override void Perform() {
        
        base.Perform();
        
        //Server v
        //controls.ShipMovement.SetLocked(true);
        //controls.Ship.invulnerable = true;
    }*/

        private static readonly int DODGE = Animator.StringToHash("Dodge");

        protected override void Execute() {
            throw new System.NotImplementedException();
        }

        protected override void Render() {
            shipManager.animator.SetTrigger(DODGE);
        }
    }
}
