using System.Collections.Generic;
using UnityEngine;

public class LeechLife : Ability
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
                aEffect = ObjectPooler.Instance.SpawnFromPool(abilityEffect.EffectTag, pathNode.node.transform.position, abilityEffect.transform.rotation).GetComponent<AbilityEffect>();
                aEffect = ObjectPooler.Instance.SpawnFromPool(abilityEffect.EffectTag, 
                    GameController.Instance.Grid.nodeList[user.Coords.x, user.Coords.y].transform.position, abilityEffect.transform.rotation).GetComponent<AbilityEffect>();

                int newHealth, oldHealth;

                oldHealth = target.health;
                target.ChangeHealth(-abilityData.values[0]);
                newHealth = target.health;
                user.ChangeHealth(oldHealth - newHealth);
            }
        }
    }
}
