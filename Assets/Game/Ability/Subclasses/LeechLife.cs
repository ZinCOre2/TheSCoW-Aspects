using System.Collections.Generic;
using UnityEngine;

public class LeechLife : Ability
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

        foreach (var pathNode in aoe)
        {
            var target = GameController.Instance.Grid.GetUnitOnNode(pathNode.node.Coords);
            
            if (!target || target.TeamId == 0 || target.TeamId == user.TeamId) { continue; }
            
            base.UseAbility(user, aoe);
            user.ChangeEnergy(-abilityData.epCost);
            user.ChangeTime(-abilityData.tpCost);
                
            var aEffect = GameController.Instance.ObjectPooler.SpawnFromPool(abilityEffect.EffectTag,
                pathNode.node.transform.position, abilityEffect.transform.rotation).GetComponent<AbilityEffect>();
            aEffect = GameController.Instance.ObjectPooler.SpawnFromPool(abilityEffect.EffectTag, 
                GameController.Instance.Grid.nodeList[user.Coords.x, user.Coords.y].transform.position, abilityEffect.transform.rotation).GetComponent<AbilityEffect>();

            var oldHealth = target.UnitStats.Health;
            var value = (int)((abilityData.values[0] * (1 + user.UnitData.AspectDedications[2].Value / 100f) + user.UnitData.Power) / 5f) * 5;
            target.ChangeHealth(-abilityData.values[0]);
                
            var newHealth = target.UnitStats.Health;
            user.ChangeHealth(oldHealth - newHealth);
        }
    }
}
