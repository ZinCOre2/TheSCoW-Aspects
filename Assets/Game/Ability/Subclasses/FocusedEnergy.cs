using System.Collections.Generic;
using UnityEngine;

public class FocusedEnergy : Ability
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
        
        var target = GameController.Instance.Grid.GetUnitOnNode(aoe[0].node.Coords);
        
        if (target.TeamId == user.TeamId)
        {
            base.UseAbility(user, aoe);
            user.ChangeEnergy(-abilityData.epCost);
            user.ChangeTime(-abilityData.tpCost);

            AbilityEffect aEffect;
            aEffect = GameController.Instance.ObjectPooler.SpawnFromPool(abilityEffect.EffectTag, aoe[0].node.transform.position, 
                abilityEffect.transform.rotation).GetComponent<AbilityEffect>();
            
            var value = (int)((abilityData.values[0] * (1 + user.UnitStats.AspectDedications[0].Value / 100f)) / 5f) * 5;

            target.UnitStats.MaxEnergy += value;
            target.ChangeEnergy(value);
            
            GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.NotEnoughEnergy,
                user.transform.position, $"+{value} макс. энергии");
        }
    }
}
