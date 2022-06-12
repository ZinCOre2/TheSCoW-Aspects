using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DemonSeal : Ability
{
    public override bool UseAbility(Unit user, List<PathNode> aoe)
    {
        if (!EnoughBasicResources(abilityData.epCost, abilityData.tpCost, user))
        {
            return false;
        }

        Unit target;

        target = GameController.Instance.Grid.GetUnitOnNode(aoe[0].node.Coords);
        if (target && target.TeamId != 0 && target.TeamId != user.TeamId)
        {
            if (!(user is MasterUnit masterUser)) { return false; }
            if (!target || !(target is MasterUnit masterTarget)) { return false; }
            
            SpendBasicResourcesIfEnough(abilityData.epCost, 
                        abilityData.tpCost, user);
            CommitUseAbility(user);

            AbilityEffect aEffect;
            aEffect = GameController.Instance.ObjectPooler.SpawnFromPool(abilityEffect.EffectTag, 
                aoe[0].node.transform.position, abilityEffect.transform.rotation).GetComponent<AbilityEffect>();
            aEffect = GameController.Instance.ObjectPooler.SpawnFromPool(abilityEffect.EffectTag, 
                GameController.Instance.Grid.nodeList[user.Coords.x, user.Coords.y].transform.position, abilityEffect.transform.rotation).GetComponent<AbilityEffect>();

            for (int i = 0, count = 0; i < masterTarget.DeckManager.Hand.Length && 
                                       count < abilityData.values[0]; i++)
            {
                if (masterTarget.DeckManager.Hand[i] != AbilityHolder.AbilityType.None)
                {
                    masterTarget.DeckManager.DiscardCard(i);
                    count++;
                }
            }
            
            // Destroy right-most card of hand
            var hand = masterUser.DeckManager.Hand;

            for (var i = hand.Length - 1; i >= 0; i--)
            {
                if (hand[i] == AbilityHolder.AbilityType.None) { continue; }
            
                hand[i] = AbilityHolder.AbilityType.None;
                break;
            }
        }

        return false;
    }
}
