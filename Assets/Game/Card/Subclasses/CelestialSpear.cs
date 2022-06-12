using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CelestialSpear : Ability
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
            CommitUseAbility(user);
            SpendBasicResourcesIfEnough(abilityData.epCost, 
                        abilityData.tpCost, user);

            AbilityEffect aEffect;
            for (var i = 0; i < 10; i++)
            {
                aEffect = GameController.Instance.ObjectPooler.SpawnFromPool(abilityEffect.EffectTag, aoe[0].node.transform.position + 
                    Vector3.up * i * 2f, abilityEffect.transform.rotation).GetComponent<AbilityEffect>();
            }

            var value1 = (int)((abilityData.values[0] * (1 + user.UnitStats.AspectDedications[0].Value / 100f) + user.UnitStats.Power) / 5f) * 5;
            target.ChangeHealth(-value1);
            var value2 = (int)((abilityData.values[1] * (1 + user.UnitStats.AspectDedications[3].Value / 100f) + user.UnitStats.Power) / 5f) * 5;
            target.ChangeEnergy(-value2);
        }
        
        return true;
    }
}
