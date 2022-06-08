using System.Collections.Generic;

public class Move : Ability
{
    public override List<PathNode> GetNodesInRange(Unit user)
    {
        Node start = GameController.Instance.Grid.nodeList[user.Coords.x, user.Coords.y];
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
        var path = new List<PathNode>();
        foreach (var pNode in aoe)
        {
            path.Add(pNode);
        }

        if (user.UnitStats.Energy < path[0].length * abilityData.epCost)
        {
            GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.NotEnoughEnergy,
                user.transform.position, "Недостаточно энергии!");
            return;
        }
        if (user.UnitStats.Time < path[0].length * abilityData.tpCost)
        {
            GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.NotEnoughTime,
                user.transform.position, "Недостаточно времени!");
            return;
        }

        if (GameController.Instance.Grid.NodeOccupied(path[0].node.Coords))
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

        user.SetCoords(path[0].node.Coords);
        user.ChangeEnergy(-path[0].length * abilityData.epCost);
        user.ChangeTime(-path[0].length * abilityData.tpCost);

        StartCoroutine(user.MoveByPath(path));
    }
}
