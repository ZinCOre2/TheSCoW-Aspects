using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnergyBurst : Ability
{
    public override bool UseAbility(Unit user, List<PathNode> aoe)
    {
        if (!EnoughBasicResources(abilityData.epCost, abilityData.tpCost, user))
        {
            return false;
        }
        SpendBasicResourcesIfEnough(abilityData.epCost, 
            abilityData.tpCost, user);
        CommitUseAbility(user);
        
        Unit target;
        foreach (PathNode pathNode in aoe)
        {
            AbilityEffect aEffect;
            aEffect = GameController.Instance.ObjectPooler.SpawnFromPool(abilityEffect.EffectTag, pathNode.node.transform.position, abilityEffect.transform.rotation).GetComponent<AbilityEffect>();

            target = GameController.Instance.Grid.GetUnitOnNode(pathNode.node.Coords);
            if (target && target.TeamId != user.TeamId)
            {
                var value1 = (int)((abilityData.values[0] * (1 + user.UnitStats.AspectDedications[2].Value / 100f) + user.UnitStats.Power) / 5f) * 5;
                target.ChangeHealth(-value1);
                var value2 = (int)((abilityData.values[1] * (1 + user.UnitStats.AspectDedications[3].Value / 100f)) / 5f) * 5;
                target.ChangeEnergy(-value2);
            }
        }

        return true;
    }
}
