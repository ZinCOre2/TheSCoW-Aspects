using System.Collections.Generic;

public class Move : Ability
{
    public override List<PathNode> GetNodesInRange(Unit user)
    {
        Node start = GameController.Instance.Grid.nodeList[user.Coords.x, user.Coords.y];
        area = new List<PathNode>();

        var maxRange = user.energy / abilityData.epCost < user.time / abilityData.tpCost
            ? user.energy / abilityData.epCost
            : user.time / abilityData.tpCost;
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

        if (user.energy < aoe[0].length * abilityData.epCost)
        {
            return;
        }
        if (user.time < aoe[0].length * abilityData.tpCost)
        {
            return; // Not enough energy
        }
        
        if (GameController.Instance.Grid.NodeOccupied(aoe[0].node.Coords))
        {
            return;
        }

        user.SetCoords(aoe[0].node.Coords);
        user.ChangeEnergy(-aoe[0].length * abilityData.epCost);
        user.ChangeTime(-aoe[0].length * abilityData.tpCost);

        StartCoroutine(user.MoveByPath(aoe));
    }
}
