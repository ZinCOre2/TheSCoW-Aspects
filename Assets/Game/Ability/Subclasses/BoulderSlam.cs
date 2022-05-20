using System.Collections.Generic;
using UnityEngine;

public class BoulderSlam : Ability
{
    public override void UseAbility(Unit user, List<PathNode> aoe)
    {
        if (abilityData.epCost > user.energy)
        {
            return; // Not enough energy
        }

        base.UseAbility(user, aoe);
        user.ChangeEnergy(-abilityData.epCost);

        Unit target;
        foreach (PathNode pathNode in aoe)
        {
            AbilityEffect aEffect;
            aEffect = ObjectPooler.Instance.SpawnFromPool(abilityEffect.EffectTag, pathNode.node.transform.position, abilityEffect.transform.rotation).GetComponent<AbilityEffect>();

            target = SceneController.Instance.Grid.GetUnitOnNode(pathNode.node.Coords);
            if (target && target.TeamId != 0 && target.TeamId != user.TeamId)
            {
                target.ChangeHealth(-abilityData.values[0]);
            }
            else
            {
                // Create rock
            }
        }
    }
}
