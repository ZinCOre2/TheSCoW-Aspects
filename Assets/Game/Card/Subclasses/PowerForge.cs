using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PowerForge : Ability
{
    public override bool UseAbility(Unit user, List<PathNode> aoe)
    {
        if (!EnoughBasicResources(abilityData.epCost, abilityData.tpCost, user))
        {
            return false;
        }
        
        var target = GameController.Instance.Grid.GetUnitOnNode(aoe[0].node.Coords);
        
        if (target.TeamId == user.TeamId)
        {
            SpendBasicResourcesIfEnough(abilityData.epCost, 
                abilityData.tpCost, user);
            CommitUseAbility(user);
            
            AbilityEffect aEffect;
            aEffect = GameController.Instance.ObjectPooler.SpawnFromPool(abilityEffect.EffectTag, aoe[0].node.transform.position, 
                abilityEffect.transform.rotation).GetComponent<AbilityEffect>();
            
            var value = (int)((abilityData.values[0] * (1 + user.UnitStats.AspectDedications[0].Value / 100f)) / 5f) * 5;

            target.UnitStats.Power += value;
            
            GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.NotEnoughTime,
                target.transform.position, $"+{value} мощи");
        }

        return true;
    }
}
