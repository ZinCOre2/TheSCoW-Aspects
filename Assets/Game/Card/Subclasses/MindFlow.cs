using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MindFlow : Ability
{
    public override bool UseAbility(Unit user, List<PathNode> aoe)
    {
        if (!EnoughBasicResources(abilityData.epCost, abilityData.tpCost, user))
        {
            return false;
        }
        
        if (!(user is MasterUnit masterUser)) { return false; }
        
        SpendBasicResourcesIfEnough(abilityData.epCost, 
            abilityData.tpCost, user);
        CommitUseAbility(user);
        
        AbilityEffect aEffect;
        aEffect = GameController.Instance.ObjectPooler.SpawnFromPool(abilityEffect.EffectTag, 
            GameController.Instance.Grid.nodeList[user.Coords.x, user.Coords.y].transform.position, abilityEffect.transform.rotation).GetComponent<AbilityEffect>();

        for (var i = 0; i < abilityData.values[0]; i++)
        {
            masterUser.DeckManager.DrawCard();
        }

        return true;
    }
}
