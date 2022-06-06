using System.Collections.Generic;
using UnityEngine;

public class InnerPower : Ability
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

        Unit target;

        target = GameController.Instance.Grid.GetUnitOnNode(aoe[0].node.Coords);
        if (target && target.TeamId == user.TeamId)
        {
            base.UseAbility(user, aoe);
            user.ChangeEnergy(-abilityData.epCost);
            user.ChangeTime(-abilityData.tpCost);

            AbilityEffect aEffect;
            aEffect = GameController.Instance.ObjectPooler.SpawnFromPool(abilityEffect.EffectTag, 
                aoe[0].node.transform.position, abilityEffect.transform.rotation).GetComponent<AbilityEffect>();
            aEffect = GameController.Instance.ObjectPooler.SpawnFromPool(abilityEffect.EffectTag, 
                GameController.Instance.Grid.nodeList[user.Coords.x, user.Coords.y].transform.position, abilityEffect.transform.rotation).GetComponent<AbilityEffect>();

            var value1 = (int)((abilityData.values[0] * (1 + user.UnitStats.AspectDedications[2].Value / 100f) + user.UnitStats.Power) / 5f) * 5;
            var value2 = (int)((abilityData.values[1] * (1 + user.UnitStats.AspectDedications[0].Value / 100f) + user.UnitStats.Power) / 5f) * 5;
            
            target.ChangeHealth(value1);
            target.ChangeEnergy(value2);
            
            if (!target || !(target is MasterUnit masterUser)) { return; }
            
            for (var i = 0; i < abilityData.values[2]; i++)
            {
                masterUser.DeckManager.DrawCard();
            }
        }
        
    }
}
