using System.Collections.Generic;

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
        CommitUseAbility(user, aoe);
        
        user.SetCoords(aoe[0].node.Coords);
        StartCoroutine(user.RushToPosition(aoe[0].node));

        return true;
    }
}
