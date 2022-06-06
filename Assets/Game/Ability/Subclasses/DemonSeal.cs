using System.Collections.Generic;
using UnityEngine;

public class DemonSeal : Ability
{
    public override void UseAbility(Unit user, List<PathNode> aoe)
    {
        if (abilityData.epCost > user.UnitStats.Energy)
        {
            GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.NotEnoughEnergy,
                user.transform.position, "Недостаточно энергии!");
            return;
        }
        
        if (abilityData.tpCost > user.UnitStats.Time)
        {
            GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.NotEnoughTime,
                user.transform.position, "Недостаточно времени!");
            return;
        }

        Unit target;

        target = GameController.Instance.Grid.GetUnitOnNode(aoe[0].node.Coords);
        if (target && target.TeamId != 0 && target.TeamId != user.TeamId)
        {
            base.UseAbility(user, aoe);
            user.ChangeEnergy(-abilityData.epCost);
            user.ChangeTime(-abilityData.tpCost);

            AbilityEffect aEffect;
            aEffect = GameController.Instance.ObjectPooler.SpawnFromPool(abilityEffect.EffectTag, 
                aoe[0].node.transform.position, abilityEffect.transform.rotation).GetComponent<AbilityEffect>();
            aEffect = GameController.Instance.ObjectPooler.SpawnFromPool(abilityEffect.EffectTag, 
                GameController.Instance.Grid.nodeList[user.Coords.x, user.Coords.y].transform.position, abilityEffect.transform.rotation).GetComponent<AbilityEffect>();
            
            if (!(user is MasterUnit masterUser)) { return; }
            if (!target || !(target is MasterUnit masterTarget)) { return; }

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
    }
}
