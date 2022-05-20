using System.Collections.Generic;
using UnityEngine;

public class EnergyDrain : Ability
{
    public override void UseAbility(Unit user, List<PathNode> aoe)
    {
        if (abilityData.epCost > user.energy)
        {
            return; // Not enough energy
        }
        
        Unit target;
        foreach (PathNode pathNode in aoe)
        {
            target = SceneController.Instance.Grid.GetUnitOnNode(pathNode.node.Coords);
            if (target && target.TeamId != 0 && target.TeamId != user.TeamId)
            {
                base.UseAbility(user, aoe);
                user.ChangeEnergy(-abilityData.epCost);

                AbilityEffect aEffect;
                aEffect = ObjectPooler.Instance.SpawnFromPool(abilityEffect.EffectTag, pathNode.node.transform.position, abilityEffect.transform.rotation).GetComponent<AbilityEffect>();
                aEffect = ObjectPooler.Instance.SpawnFromPool(abilityEffect.EffectTag, 
                    SceneController.Instance.Grid.nodeList[user.Coords.x, user.Coords.y].transform.position, abilityEffect.transform.rotation).GetComponent<AbilityEffect>();

                int newEnergy, oldEnergy;

                oldEnergy = target.energy;
                target.ChangeEnergy(-abilityData.values[0]);
                newEnergy = target.energy;
                user.ChangeEnergy(oldEnergy - newEnergy);
            }
        }
    }
}
