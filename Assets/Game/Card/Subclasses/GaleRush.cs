using System.Collections.Generic;

[System.Serializable]
public class GaleRush : Ability
{
    public override bool UseAbility(Unit user, List<PathNode> aoe)
    {
        if (!EnoughBasicResources(abilityData.epCost, abilityData.tpCost, user))
        {
            return false;
        }

        if (GameController.Instance.Grid.NodeOccupied(aoe[0].node.Coords)) { return false; }
        
        SpendBasicResourcesIfEnough(abilityData.epCost, 
            abilityData.tpCost, user);
        CommitUseAbility(user);
        
        user.SetCoords(aoe[0].node.Coords);
        
        user.StartRushingToPosition(aoe[0].node);

        return true;
    }
}
