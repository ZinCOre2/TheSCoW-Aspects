using System.Collections.Generic;

public class Move : Ability
{
    public override List<PathNode> GetNodesInRange(Unit user)
    {
        Node start = SceneController.Instance.Grid.nodeList[user.Coords.x, user.Coords.y];
        area = new List<PathNode>();
  
        area = Pathfinding.GetNodesInPathfindingRange(start, 0, user.energy / user.UnitData_.moveCost);

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

        if (user.energy < aoe[0].length * user.UnitData_.moveCost)
        {
            return;
        }
        if (SceneController.Instance.Grid.NodeOccupied(aoe[0].node.Coords))
        {
            return;
        }

        user.SetCoords(aoe[0].node.Coords);
        user.ChangeEnergy(-aoe[0].length * user.UnitData_.moveCost);

        StartCoroutine(user.MoveByPath(aoe));
    }
}
