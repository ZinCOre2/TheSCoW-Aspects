using System.Collections.Generic;
using UnityEngine;

public class Educate : Ability
{
    public override void UseAbility(Unit user, List<PathNode> aoe)
    {
        if (abilityData.epCost > user.UnitStats.Energy)
        {
            GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.NotEnoughEnergy,
                user.transform.position, "Недостаточно энергии!");
            return;
        }
        
        if (abilityData.tpCost > user.UnitStats.Time)
        {
            GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.NotEnoughTime,
                user.transform.position, "Недостаточно времени!");
            return;
        }
        
        var target = GameController.Instance.Grid.GetUnitOnNode(aoe[0].node.Coords);
        
        if (!target || !(target is MasterUnit)) { return; }
        
        if (target.TeamId == user.TeamId)
        {
            base.UseAbility(user, aoe);
            user.ChangeEnergy(-abilityData.epCost);
            user.ChangeTime(-abilityData.tpCost);

            AbilityEffect aEffect;
            aEffect = GameController.Instance.ObjectPooler.SpawnFromPool(abilityEffect.EffectTag, aoe[0].node.transform.position, 
                abilityEffect.transform.rotation).GetComponent<AbilityEffect>();

            for (var i = 0; i < abilityData.values[0]; i++)
            {
                ((MasterUnit)target).DeckManager.DrawCard();
            }
        }
    }
}
