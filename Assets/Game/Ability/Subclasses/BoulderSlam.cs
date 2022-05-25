using System.Collections.Generic;
using UnityEngine;

public class BoulderSlam : Ability
{
    [SerializeField] private PhysicalEntity rockEntity;

    public override void UseAbility(Unit user, List<PathNode> aoe)
    {
        if (abilityData.epCost > user.energy)
        {
            GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.NotEnoughEnergy,
                user.transform.position, "Недостаточно энергии!");
            return;
        }
        
        if (abilityData.tpCost > user.time)
        {
            GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.NotEnoughTime,
                user.transform.position, "Недостаточно времени!");
            return;
        }

        base.UseAbility(user, aoe);
        user.ChangeEnergy(-abilityData.epCost);
        user.ChangeTime(-abilityData.tpCost);        

        Unit target;
        foreach (PathNode pathNode in aoe)
        {
            AbilityEffect aEffect;
            aEffect = GameController.Instance.ObjectPooler.SpawnFromPool(abilityEffect.EffectTag, pathNode.node.transform.position, abilityEffect.transform.rotation).GetComponent<AbilityEffect>();

            target = GameController.Instance.Grid.GetUnitOnNode(pathNode.node.Coords);
            var damage = (int)((abilityData.values[0] * (1 + user.UnitData.AspectDedications[1].Value / 100f) + user.UnitData.power) / 5f) * 5;

            if (target && target.TeamId != 0 && target.TeamId != user.TeamId)
            {
                target.ChangeHealth(-damage);
            }
            else
            {
                var rock = Instantiate(rockEntity, pathNode.node.transform.position, Quaternion.identity);
                GameController.Instance.EntityManager.AddEntity(rock);
            }
        }
    }
}
