using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Aeroblade : Ability
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
            if (!target || target.TeamId == user.TeamId) { continue; }
            
            var value = (int)((abilityData.values[0] * (1 + user.UnitStats.AspectDedications[2].Value / 100f) + user.UnitData.power) / 5f) * 5;
            target.ChangeHealth(-value);
        }

        return true;
    }
}
