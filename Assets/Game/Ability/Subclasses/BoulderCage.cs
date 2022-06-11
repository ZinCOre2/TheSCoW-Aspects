using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BoulderCage : Ability
{
    [SerializeField] private PhysicalEntity rockEntity;
    
    public override bool UseAbility(Unit user, List<PathNode> aoe)
    {
        if (!EnoughBasicResources(abilityData.epCost, abilityData.tpCost, user))
        {
            return false;
        }
        SpendBasicResourcesIfEnough(abilityData.epCost, 
            abilityData.tpCost, user);
        CommitUseAbility(user, aoe);

        Unit target;
        foreach (PathNode pathNode in aoe)
        {
            AbilityEffect aEffect;
            aEffect = GameController.Instance.ObjectPooler.SpawnFromPool(abilityEffect.EffectTag, pathNode.node.transform.position, abilityEffect.transform.rotation).GetComponent<AbilityEffect>();

            target = GameController.Instance.Grid.GetUnitOnNode(pathNode.node.Coords);
            var damage = (int)((abilityData.values[0] * (1 + user.UnitStats.AspectDedications[1].Value / 100f) + user.UnitStats.Power) / 5f) * 5;

            if (target && target.TeamId != user.TeamId)
            {
                target.ChangeHealth(-damage);
            }
            
            if (!GameController.Instance.Grid.NodeOccupied(pathNode.node.Coords))
            {
                var rock = Instantiate(rockEntity, pathNode.node.transform.position, Quaternion.identity);
                GameController.Instance.EntityManager.AddEntity(rock);
            }
        }
        
        Task.Delay(100);
        GameController.Instance.SceneController.SetSelectedAbility(AbilityHolder.AbilityType.Move);
        GameController.Instance.SceneController.ResetUsageArea();

        return true;
    }
}
