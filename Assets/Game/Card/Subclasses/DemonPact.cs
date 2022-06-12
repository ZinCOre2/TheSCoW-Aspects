using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DemonPact : Ability
{
    public override bool UseAbility(Unit user, List<PathNode> aoe)
    {
        if (!EnoughBasicResources(abilityData.epCost, abilityData.tpCost, user))
        {
            return false;
        }

        Unit target;

        target = GameController.Instance.Grid.GetUnitOnNode(aoe[0].node.Coords);
        if (target && target.TeamId == user.TeamId)
        {
            SpendBasicResourcesIfEnough(abilityData.epCost, 
                     abilityData.tpCost, user);
            CommitUseAbility(user);

            AbilityEffect aEffect;
            aEffect = GameController.Instance.ObjectPooler.SpawnFromPool(abilityEffect.EffectTag, 
                aoe[0].node.transform.position, abilityEffect.transform.rotation).GetComponent<AbilityEffect>();
            aEffect = GameController.Instance.ObjectPooler.SpawnFromPool(abilityEffect.EffectTag, 
                GameController.Instance.Grid.nodeList[user.Coords.x, user.Coords.y].transform.position, abilityEffect.transform.rotation).GetComponent<AbilityEffect>();

            user.ChangeHealth(-abilityData.values[0]);
            
            var value1 = (int)((abilityData.values[1] * (1 + user.UnitStats.AspectDedications[0].Value / 100f) + user.UnitStats.Power) / 5f) * 5;
            var value2 = (int)((abilityData.values[2] * (1 + user.UnitStats.AspectDedications[3].Value / 100f)) / 5f) * 5;

            target.ChangeEnergy(value1);
            target.UnitStats.Power += value2;
            
            GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.NotEnoughTime,
                target.transform.position, $"+{value2} мощи");
            
            if (!(user is MasterUnit masterUser)) { return false; }
            if (!target || !(target is MasterUnit masterTarget)) { return false; }
            
            // Destroy right-most card of hand
            var hand = masterUser.DeckManager.Hand;

            for (var i = hand.Length - 1; i >= 0; i--)
            {
                if (hand[i] == AbilityHolder.AbilityType.None) { continue; }
            
                hand[i] = AbilityHolder.AbilityType.None;
                break;
            }
            
            for (var i = 0; i < abilityData.values[3]; i++)
            {
                masterTarget.DeckManager.DrawCard();
            }
        }

        return true;
    }
}
