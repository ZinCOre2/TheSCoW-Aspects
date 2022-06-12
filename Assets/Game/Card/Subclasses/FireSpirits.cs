using System.Collections.Generic;

[System.Serializable]
public class FireSpirits : Ability
{
    public override bool UseAbility(Unit user, List<PathNode> aoe)
    {
        if (!EnoughBasicResources(abilityData.epCost, abilityData.tpCost, user))
        {
            return false;
        }
        SpendBasicResourcesIfEnough(abilityData.epCost, 
            abilityData.tpCost, user);
        CommitUseAbility(user);
        
        Unit target;

        foreach (PathNode pathNode in aoe)
        {
            target = GameController.Instance.Grid.GetUnitOnNode(pathNode.node.Coords);
            if (target && target.TeamId != user.TeamId)
            {
                AbilityEffect aEffect;
                aEffect = GameController.Instance.ObjectPooler.SpawnFromPool(abilityEffect.EffectTag, pathNode.node.transform.position, abilityEffect.transform.rotation).GetComponent<AbilityEffect>();
                
                var value = (int)((abilityData.values[0] * (1 + user.UnitData.AspectDedications[0].Value / 100f) + user.UnitData.power) / 5f) * 5;
                target.ChangeHealth(-value);
            }
        }

        return true;
    }
}
