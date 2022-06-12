using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class BoulderSlam : Ability
{
    [SerializeField] private PhysicalEntity rockEntity;

    public override bool UseAbility(Unit user, List<PathNode> aoe)
    {
        Unit target;
        
        if (!EnoughBasicResources(abilityData.epCost, abilityData.tpCost, user))
        {
            return false;
        }
        SpendBasicResourcesIfEnough(abilityData.epCost, 
            abilityData.tpCost, user);

        target = GameController.Instance.Grid.GetUnitOnNode(aoe[0].node.Coords);
        
        if (target && target.TeamId != user.TeamId)
        {
            CommitUseAbility(user);
            AbilityEffect aEffect;
            aEffect = GameController.Instance.ObjectPooler.SpawnFromPool(abilityEffect.EffectTag, aoe[0].node.transform.position, abilityEffect.transform.rotation).GetComponent<AbilityEffect>();
            
            var damage = (int)((abilityData.values[0] * (1 + user.UnitStats.AspectDedications[1].Value / 100f) + user.UnitStats.Power) / 5) * 5;

            target.ChangeHealth(-damage);
        }
        else if (!GameController.Instance.Grid.NodeOccupied(aoe[0].node.Coords))
        {
            CommitUseAbility(user);
            GameController.Instance.EntityManager.CreateEntity(rockEntity, aoe[0].node.transform.position);
        }

        return true;
    }
}
