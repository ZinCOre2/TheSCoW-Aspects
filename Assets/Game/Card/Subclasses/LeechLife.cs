using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LeechLife : Ability
{
    public override bool UseAbility(Unit user, List<PathNode> aoe)
    {
        if (!EnoughBasicResources(abilityData.epCost, abilityData.tpCost, user))
        {
            return false;
        }

        Unit target;
        target = GameController.Instance.Grid.GetUnitOnNode(aoe[0].node.Coords);
        
        if (target && target.TeamId != user.TeamId)
        {
            SpendBasicResourcesIfEnough(abilityData.epCost, 
                abilityData.tpCost, user);
            CommitUseAbility(user);

            AbilityEffect aEffect;
            aEffect = GameController.Instance.ObjectPooler.SpawnFromPool(abilityEffect.EffectTag, aoe[0].node.transform.position, abilityEffect.transform.rotation).GetComponent<AbilityEffect>();
            aEffect = GameController.Instance.ObjectPooler.SpawnFromPool(abilityEffect.EffectTag, 
                GameController.Instance.Grid.nodeList[user.Coords.x, user.Coords.y].transform.position, abilityEffect.transform.rotation).GetComponent<AbilityEffect>();

            int newHealth, oldHealth;

            oldHealth = target.UnitStats.Health;
            var value = (int)((abilityData.values[0] * (1 + user.UnitData.AspectDedications[2].Value / 100f) + user.UnitData.power) / 5f) * 5;
            target.ChangeHealth(-value);
            newHealth = target.UnitStats.Health;
            user.ChangeHealth(oldHealth - newHealth);
        }

        return true;
    }
}
