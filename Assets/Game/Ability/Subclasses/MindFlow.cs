using System.Collections.Generic;
using UnityEngine;

public class MindFlow : Ability
{
    public override void UseAbility(Unit user, List<PathNode> aoe)
    {
        if (abilityData.epCost > user.energy)
        {
            return; // Not enough energy
        }
        if (abilityData.tpCost > user.time)
        {
            return; // Not enough energy
        }
        
        base.UseAbility(user, aoe);
        user.ChangeEnergy(-abilityData.epCost);
        user.ChangeTime(-abilityData.tpCost);
        
        AbilityEffect aEffect;
        aEffect = ObjectPooler.Instance.SpawnFromPool(abilityEffect.EffectTag, 
            GameController.Instance.Grid.nodeList[user.Coords.x, user.Coords.y].transform.position, abilityEffect.transform.rotation).GetComponent<AbilityEffect>();

        for (var i = 0; i < 2; i++)
        {
            user.DeckManager.DrawCard();
        }
    }
}
