using System.Collections.Generic;
using UnityEngine;

public class Strike : Ability
{
    public override void UseAbility(Unit user, List<PathNode> aoe)
    {
        if (abilityData.epCost > user.energy)
        {
            return;
        }

        Unit target;
        foreach (PathNode pathNode in aoe)
        {
            target = SceneController.Instance.Grid.GetUnitOnNode(pathNode.node.Coords);
            if (target && target.TeamId != 0 && target.TeamId != user.TeamId)
            {
                base.UseAbility(user, aoe);

                AbilityEffect aEffect;
                aEffect = ObjectPooler.Instance.SpawnFromPool(abilityEffect.EffectTag, pathNode.node.transform.position, abilityEffect.transform.rotation).GetComponent<AbilityEffect>();

                user.ChangeEnergy(-abilityData.epCost);
                target.ChangeHealth(-abilityData.values[0]);
            }
        }
    }
}
