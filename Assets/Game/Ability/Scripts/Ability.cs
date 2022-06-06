using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    [SerializeField] protected AbilityData abilityData;
    public AbilityData AbilityData { get { return abilityData; } protected set { abilityData = value; } }
    [SerializeField] protected AbilityEffect abilityEffect;
    [SerializeField] protected AudioClip soundEffect;
    [SerializeField] protected AudioSource audioSource;

    public List<PathNode> area { get; protected set; }
    public List<PathNode> aoe { get; protected set; }

    public virtual List<PathNode> GetNodesInRange(Unit user)
    {
        if (!user || !GameController.Instance.Grid.NodeExists(user.Coords))
            return new List<PathNode>();

        Node start = GameController.Instance.Grid.nodeList[user.Coords.x, user.Coords.y];
        area = new List<PathNode>();

        switch (abilityData.rangeType)
        {
            case AbilityData.AreaType.Pathfinding:
                area = Pathfinding.GetNodesInPathfindingRange(start, abilityData.minRange, abilityData.maxRange);
                break;
            case AbilityData.AreaType.Impulse:
                area = Pathfinding.GetNodesInImpulseRange(start, abilityData.minRange, abilityData.maxRange, false, true, true);
                break;
            case AbilityData.AreaType.Absolute:
                area = Pathfinding.GetNodesInAbsoluteRange(start, abilityData.minRange, abilityData.maxRange);
                break;
        }
        return area;

    }
    public virtual List<PathNode> GetAoe(Unit user, PathNode pathNode)
    {
        aoe = new List<PathNode>();

        switch (abilityData.areaType)
        {
            case AbilityData.AreaType.Pathfinding:
                aoe = Pathfinding.GetNodesInPathfindingRange(pathNode.node, abilityData.minAreaRange, abilityData.maxAreaRange);
                break;
            case AbilityData.AreaType.Impulse:
                aoe = Pathfinding.GetNodesInImpulseRange(pathNode.node, abilityData.minAreaRange, abilityData.maxAreaRange, false, true, true);
                break;
            case AbilityData.AreaType.Absolute:
                aoe = Pathfinding.GetNodesInAbsoluteRange(pathNode.node, abilityData.minAreaRange, abilityData.maxAreaRange);
                break;
            case AbilityData.AreaType.Line:
                aoe = Pathfinding.GetLinePath(GameController.Instance.Grid.nodeList[user.Coords.x, user.Coords.y], 
                    pathNode.node.Coords, false, false, false);
                break;
            default:
                break;
        }
        return aoe;
    }

    public virtual void UseAbility(Unit user, List<PathNode> aoe)
    {
        if (audioSource && soundEffect)
        {
            audioSource.clip = soundEffect;
            audioSource.Play();
        }

        if (GameController.Instance.UIController.IsCard)
        {
            if (!(user is MasterUnit masterUnit)) { return; }

            CalculateDedication(user);

            masterUnit.DeckManager.DiscardCard(GameController.Instance.UIController.selectedAbilityId);
            user.Animator.SetTrigger("UseSpell");
            GameController.Instance.UIController.SetId(0, false);
            GameController.Instance.SceneController.SetSelectedAbility(null);
        }
        else
        {
            user.Animator.SetTrigger("UseSpell");
            GameController.Instance.UIController.SetId(0, false);
            GameController.Instance.SceneController.SetSelectedAbility(null);
        }
    }

    protected virtual void CalculateDedication(Unit user)
    {
        var aspectAmount = 0;
        foreach (var dedication in abilityData.Dedications)
        {
            if (dedication.IsUsable)
            {
                aspectAmount++;
            }
        }

        for (var i = 0; i < abilityData.Dedications.Length; i++)
        {
            if (abilityData.Dedications[i].IsUsable)
            {
                user.UnitStats.DedicationsEnergy[i] += (int)((abilityData.epCost * 2 + abilityData.tpCost) / 5 / aspectAmount) * 5;
                Debug.Log($"Aspect {i}: power {user.UnitStats.DedicationsEnergy[i]}");
                
                if (user.UnitStats.DedicationsEnergy[i] >= 100)
                {
                    var multiplier = user.UnitStats.DedicationsEnergy[i] / 100;
                    user.UnitStats.DedicationsEnergy[i] -= multiplier * 100;
                    user.UnitStats.AspectDedications[i].Value += multiplier * 5;
                    
                    Debug.Log($"Aspect {i}: new power {user.UnitStats.DedicationsEnergy[i]}");
                }
            }
        }
    }
}
