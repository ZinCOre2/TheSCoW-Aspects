using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Educate : Ability
{
    public override bool UseAbility(Unit user, List<PathNode> aoe)
    {
        if (!EnoughBasicResources(abilityData.epCost, abilityData.tpCost, user))
        {
            return false;
        }

        var target = GameController.Instance.Grid.GetUnitOnNode(aoe[0].node.Coords);
        
        if (!target || !(target is MasterUnit)) { return false; }
        
        if (target.TeamId == user.TeamId)
        {
            SpendBasicResourcesIfEnough(abilityData.epCost, 
                        abilityData.tpCost, user); 
            CommitUseAbility(user);

            AbilityEffect aEffect;
            aEffect = GameController.Instance.ObjectPooler.SpawnFromPool(abilityEffect.EffectTag, aoe[0].node.transform.position, 
                abilityEffect.transform.rotation).GetComponent<AbilityEffect>();

            for (var i = 0; i < abilityData.values[0]; i++)
            {
                ((MasterUnit)target).DeckManager.DrawCard();
            }
        }

        return true;
    }
}
