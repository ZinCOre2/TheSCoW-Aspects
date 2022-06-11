using System;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    [SerializeField] protected AbilityData abilityData;
    public AbilityData AbilityData { get { return abilityData; } protected set { abilityData = value; } }
    [SerializeField] protected AbilityEffect abilityEffect;
    [SerializeField] protected AudioClipData soundEffect;

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

    public virtual bool UseAbility(Unit user, List<PathNode> aoe)
    {
        return true;
    }

    protected virtual bool CommitUseAbility(Unit user, List<PathNode> aoe)
    {
        if (soundEffect)
        {
            GameController.Instance.AudioManager.UseAudioSource(soundEffect);
        }

        if (GameController.Instance.UIController.IsCard)
        {
            if (!(user is MasterUnit)) { return false; }

            CalculateDedication(user);

            ((MasterUnit)user).DeckManager.DiscardCard(GameController.Instance.UIController.selectedAbilityId);
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

        return true;
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
            if (abilityData.Dedications[i].IsUsable && 
                user.UnitStats.AspectDedications[i].IsUsable)
            {
                user.UnitStats.DedicationsEnergy[i] += (int)((abilityData.epCost * 2 + abilityData.tpCost) / 5 / aspectAmount) * 5;
                Debug.Log($"Aspect {i}: power {user.UnitStats.DedicationsEnergy[i]}");
                
                if (user.UnitStats.DedicationsEnergy[i] >= 100)
                {
                    var multiplier = user.UnitStats.DedicationsEnergy[i] / 100;
                    user.UnitStats.DedicationsEnergy[i] -= multiplier * 100;
                    user.UnitStats.AspectDedications[i].Value += multiplier * 5;

                    AspectChangeText(i, multiplier * 5, user);
                    
                    Debug.Log($"Aspect {i}: new power {user.UnitStats.DedicationsEnergy[i]}");
                }
            }
        }
    }
    protected void AspectChangeText(int aspectId, int amount, Unit user)
    {
        string aspectName = string.Empty;
        HoveringWorldText hwt;
        
        switch (aspectId)
        {
            case 0:
                aspectName = "Инетры";                
                if (amount > 0)
                {
                    hwt = GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.InetraBoost,
                        user.transform.position, $"+{amount} {aspectName}");
                }
                else
                {
                    hwt = GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.InetraSpent,
                        user.transform.position, $"{amount} {aspectName}");
                }
                break;
            case 1:
                aspectName = "Диатры";                
                if (amount > 0)
                {
                    hwt = GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.DiatraBoost,
                        user.transform.position, $"+{amount} {aspectName}");
                }
                else
                {
                    hwt = GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.DiatraSpent,
                        user.transform.position, $"{amount} {aspectName}");
                }
                break;
            case 2:
                aspectName = "Эфры";
                if (amount > 0)
                {
                    hwt = GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.EfraBoost,
                        user.transform.position, $"+{amount} {aspectName}");
                }
                else
                {
                    hwt = GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.EfraSpent,
                        user.transform.position, $"{amount} {aspectName}");
                }
                break;
            case 3:
                aspectName = "Саквы";
                if (amount > 0)
                {
                    hwt = GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.SaquaBoost,
                        user.transform.position, $"+{amount} {aspectName}");
                }
                else
                {
                    hwt = GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.SaquaSpent,
                        user.transform.position, $"{amount} {aspectName}");
                }
                break;
        }
    }

    protected bool SpendBasicResourcesIfEnough(int energyCost, int timeCost, Unit user)
    {
        if (!EnoughBasicResources(energyCost, timeCost, user)) { return false; }
        
        user.ChangeEnergy(-energyCost);
        user.ChangeTime(-timeCost);
        
        for (var i = 0; i < abilityData.Dedications.Length; i++)
        {
            if (!abilityData.Dedications[i].IsUsable) { continue; }

            user.UnitStats.AspectDedications[i].Value -= abilityData.Dedications[i].Value;
        }

        return true;
    }
    protected bool EnoughBasicResources(int energyCost, int timeCost, Unit user)
    {
        if (user.UnitStats.Energy < energyCost)
        {
            GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.NotEnoughEnergy,
                user.transform.position, "Недостаточно энергии!");
            return false;
        }
        if (user.UnitStats.Time < timeCost)
        {
            GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.NotEnoughTime,
                user.transform.position, "Недостаточно времени!");
            return false;
        }

        for (var i = 0; i < abilityData.Dedications.Length; i++)
        {
            if (!abilityData.Dedications[i].IsUsable) { continue; }
            
            if (user.UnitStats.AspectDedications[i].Value < abilityData.Dedications[i].Value)
            {
                GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.NotEnoughTime,
                    user.transform.position, "Недостаточно аспекта!");
                return false;
            }
        }

        return true;
    }
}
