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
    public override bool UseAbility(Unit user, List<PathNode> aoe)
    {
        var path = new List<PathNode>();
        foreach (var pNode in aoe)
        {
            path.Add(pNode);
        }

        if (GameController.Instance.Grid.NodeOccupied(path[0].node.Coords))
        {
            return false;
        }
        
        if (!EnoughBasicResources(path[0].length * abilityData.epCost,
            path[0].length * abilityData.tpCost, user))
        {
            return false;
        }
        SpendBasicResourcesIfEnough(path[0].length * abilityData.epCost,
            path[0].length * abilityData.tpCost,
            user);

        if (soundEffect)
        {
            var audioSource = GameController.Instance.AudioManager.UseAudioSource(soundEffect);
            
            user.OnFinishAbilityUse += () =>
            {
                audioSource.Stop();
            };
        }

        user.SetCoords(path[0].node.Coords);

        StartCoroutine(user.MoveByPath(path));

        return true;
    }
}
