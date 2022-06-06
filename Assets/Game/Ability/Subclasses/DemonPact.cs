using System.Collections.Generic;
using UnityEngine;

public class DemonPact : Ability
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
        if (target && target.TeamId == user.TeamId)
        {
            base.UseAbility(user, aoe);
            user.ChangeEnergy(-abilityData.epCost);
            user.ChangeTime(-abilityData.tpCost);

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
                user.transform.position, $"+{value2} мощи");
            
            if (!(user is MasterUnit masterUser)) { return; }
            if (!target || !(target is MasterUnit masterTarget)) { return; }
            
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
    }
}
