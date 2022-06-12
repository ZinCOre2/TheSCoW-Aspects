using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DemonFire : Ability
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
            var damage = (int)((abilityData.values[0] * (1 + user.UnitStats.AspectDedications[0].Value / 100f) + user.UnitStats.Power) / 5f) * 5;
            
            if (target && target.TeamId != user.TeamId)
            {
                target.ChangeHealth(-damage);
            }
        }
        
        // Destroy right-most card of hand
        if (!(user is MasterUnit)) { return false; }

        var hand = ((MasterUnit)user).DeckManager.Hand;

        for (var i = hand.Length - 1; i >= 0; i--)
        {
            if (hand[i] == AbilityHolder.AbilityType.None) { continue; }
            
            hand[i] = AbilityHolder.AbilityType.None;
            break;
        }

        return true;
    }
}
