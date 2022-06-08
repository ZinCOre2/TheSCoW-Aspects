using System.Collections.Generic;
using UnityEngine;

public class DemonFire : Ability
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
        
        base.UseAbility(user, aoe);
        user.ChangeEnergy(-abilityData.epCost);
        user.ChangeTime(-abilityData.tpCost);

        Unit target;
        foreach (PathNode pathNode in aoe)
        {
            AbilityEffect aEffect;
            aEffect = GameController.Instance.ObjectPooler.SpawnFromPool(abilityEffect.EffectTag, pathNode.node.transform.position, abilityEffect.transform.rotation).GetComponent<AbilityEffect>();

            target = GameController.Instance.Grid.GetUnitOnNode(pathNode.node.Coords);
            var damage = (int)((abilityData.values[0] * (1 + user.UnitStats.AspectDedications[0].Value / 100f) + user.UnitStats.Power) / 5f) * 5;
            
            if (target && target.TeamId != 0 && target.TeamId != user.TeamId)
            {
                target.ChangeHealth(-damage);
            }
        }
        
        // Destroy right-most card of hand
        if (!(user is MasterUnit)) { return; }

        var hand = ((MasterUnit)user).DeckManager.Hand;

        for (var i = hand.Length - 1; i >= 0; i--)
        {
            if (hand[i] == AbilityHolder.AbilityType.None) { continue; }
            
            hand[i] = AbilityHolder.AbilityType.None;
            break;
        }
    }
}
