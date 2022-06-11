using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BoulderSlam : Ability
{
    [SerializeField] private PhysicalEntity rockEntity;

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
            
            if (target && target.TeamId != user.TeamId)
            {
                AbilityEffect aEffect;
                aEffect = GameController.Instance.ObjectPooler.SpawnFromPool(abilityEffect.EffectTag, pathNode.node.transform.position, abilityEffect.transform.rotation).GetComponent<AbilityEffect>();
                
                var damage = (int)((abilityData.values[0] * (1 + user.UnitStats.AspectDedications[1].Value / 100f) + user.UnitStats.Power) / 5) * 5;
                
                base.UseAbility(user, aoe);
                user.ChangeEnergy(-abilityData.epCost);
                user.ChangeTime(-abilityData.tpCost);    
                
                target.ChangeHealth(-damage);
            }
            else if (!GameController.Instance.Grid.NodeOccupied(pathNode.node.Coords))
            {
                base.UseAbility(user, aoe);
                user.ChangeEnergy(-abilityData.epCost);
                user.ChangeTime(-abilityData.tpCost);    
                
                var rock = Instantiate(rockEntity, pathNode.node.transform.position, Quaternion.identity);
                GameController.Instance.EntityManager.AddEntity(rock);
            }
        }

        Task.Delay(100);
        GameController.Instance.SceneController.SetSelectedAbility(AbilityHolder.AbilityType.Move);
        GameController.Instance.SceneController.ResetUsageArea();
    }
}
