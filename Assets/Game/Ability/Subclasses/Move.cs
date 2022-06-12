using System.Collections.Generic;

[System.Serializable]
public class Move : Ability
{
    public override List<PathNode> GetUsageArea(Unit user)
    {
        Node start = GameController.Instance.Grid.nodeList[user.Coords.x, user.Coords.y];
        UsageArea = new List<PathNode>();

        var maxRange = user.UnitStats.Energy / abilityData.epCost < user.UnitStats.Time / abilityData.tpCost
            ? user.UnitStats.Energy / abilityData.epCost
            : user.UnitStats.Time / abilityData.tpCost;
        UsageArea = Pathfinding.GetNodesInPathfindingRange(start, 0, maxRange);

        return UsageArea;
    }
    public override List<PathNode> GetEffectArea(Unit user, PathNode pathNode)
    {
        EffectArea = new List<PathNode>();
        EffectArea = Pathfinding.GetPath(pathNode);
        
        return EffectArea;
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
            var audioSourceData = GameController.Instance.AudioManager.UseAudioSourceData(soundEffect);
            
            user.OnFinishAbilityUse += () =>
            {
                audioSourceData.AudioSource.Stop();
            };
        }

        user.SetCoords(path[0].node.Coords);

        user.StartMovingByPath(path);

        return true;
    }
}
