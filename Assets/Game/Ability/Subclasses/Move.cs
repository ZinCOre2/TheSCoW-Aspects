using System.Collections.Generic;

public class Move : Ability
{
    public override List<PathNode> GetNodesInRange(Unit user)
    {
        var start = GameController.Instance.Grid.nodeList[user.Coords.x, user.Coords.y];
        area = new List<PathNode>();

        var maxRange = user.UnitStats.Energy / abilityData.epCost < user.UnitStats.Time / abilityData.tpCost
            ? user.UnitStats.Energy / abilityData.epCost
            : user.UnitStats.Time / abilityData.tpCost;
        area = Pathfinding.GetNodesInPathfindingRange(start, 0, maxRange);

        return area;
    }
    public override List<PathNode> GetAoe(Unit user, PathNode pathNode)
    {
        aoe = new List<PathNode>();
        aoe = Pathfinding.GetPath(pathNode);
        
        return aoe;
    }
    public override void UseAbility(Unit user, List<PathNode> aoe)
    {
        if (user.UnitStats.Energy < aoe[0].length * abilityData.epCost)
        {
            GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.NotEnoughEnergy,
                user.transform.position, "Недостаточно энергии!");
            return;
        }
        if (user.UnitStats.Time < aoe[0].length * abilityData.tpCost)
        {
            GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.NotEnoughTime,
                user.transform.position, "Недостаточно времени!");
            return;
        }

        if (GameController.Instance.Grid.NodeOccupied(aoe[0].node.Coords))
        {
            return;
        }
        
        if (audioSource && soundEffect)
        {
            audioSource.clip = soundEffect;
            audioSource.loop = true;
            audioSource.Play();
            user.OnFinishAbilityUse += () =>
            {
                audioSource.Stop();
            };
        }

        user.SetCoords(aoe[0].node.Coords);
        user.ChangeEnergy(-aoe[0].length * abilityData.epCost);
        user.ChangeTime(-aoe[0].length * abilityData.tpCost);

        StartCoroutine(user.MoveByPath(aoe));
    }
}
