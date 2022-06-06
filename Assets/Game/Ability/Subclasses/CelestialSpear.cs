using System.Collections.Generic;
using UnityEngine;

public class CelestialSpear : Ability
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
        foreach (PathNode pathNode in aoe)
        {
            target = GameController.Instance.Grid.GetUnitOnNode(pathNode.node.Coords);
            if (target && target.TeamId != 0 && target.TeamId != user.TeamId)
            {
                base.UseAbility(user, aoe);
                user.ChangeEnergy(-abilityData.epCost);
                user.ChangeTime(-abilityData.tpCost);

                AbilityEffect aEffect;
                for (var i = 0; i < 10; i++)
                {
                    aEffect = GameController.Instance.ObjectPooler.SpawnFromPool(abilityEffect.EffectTag, pathNode.node.transform.position + 
                        Vector3.up * i * 2f, abilityEffect.transform.rotation).GetComponent<AbilityEffect>();
                }

                var value1 = (int)((abilityData.values[0] * (1 + user.UnitStats.AspectDedications[0].Value / 100f) + user.UnitStats.Power) / 5f) * 5;
                target.ChangeHealth(-value1);
                var value2 = (int)((abilityData.values[1] * (1 + user.UnitStats.AspectDedications[3].Value / 100f) + user.UnitStats.Power) / 5f) * 5;
                target.ChangeEnergy(-value2);
            }
        }
    }
}
