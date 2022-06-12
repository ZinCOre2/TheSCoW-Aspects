using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ability : MonoBehaviour
{
    private const float ABILITY_ASPECT_REDUCTION = 3f;
    
    [SerializeField] protected AbilityData abilityData;
    public AbilityData AbilityData { get { return abilityData; } protected set { abilityData = value; } }
    [SerializeField] protected AbilityEffect abilityEffect;
    [SerializeField] protected AudioClipData soundEffect;

    public List<PathNode> UsageArea { get; protected set; }
    public List<PathNode> EffectArea { get; protected set; }

    public virtual List<PathNode> GetUsageArea(Unit user)
    {
        if (!user || !GameController.Instance.Grid.NodeExists(user.Coords))
            return new List<PathNode>();

        Node start = GameController.Instance.Grid.nodeList[user.Coords.x, user.Coords.y];
        UsageArea = new List<PathNode>();

        switch (abilityData.rangeType)
        {
            case AbilityData.AreaType.Pathfinding:
                UsageArea = Pathfinding.GetNodesInPathfindingRange(start, abilityData.minRange, abilityData.maxRange);
                break;
            case AbilityData.AreaType.Impulse:
                UsageArea = Pathfinding.GetNodesInImpulseRange(start, abilityData.minRange, abilityData.maxRange, false,
                    true, true);
                break;
            case AbilityData.AreaType.Absolute:
                UsageArea = Pathfinding.GetNodesInAbsoluteRange(start, abilityData.minRange, abilityData.maxRange);
                break;
            case AbilityData.AreaType.Line:
                UsageArea = Pathfinding.GetNodesInImpulseRange(start, abilityData.minRange, abilityData.maxRange, false,
                    true, true);
                break;
        }
        return UsageArea;
    }
    public virtual List<PathNode> GetEffectArea(Unit user, PathNode pathNode)
    {
        EffectArea = new List<PathNode>();

        switch (abilityData.areaType)
        {
            case AbilityData.AreaType.Pathfinding:
                EffectArea = Pathfinding.GetNodesInPathfindingRange(pathNode.node, abilityData.minAreaRange, abilityData.maxAreaRange);
                break;
            case AbilityData.AreaType.Impulse:
                EffectArea = Pathfinding.GetNodesInImpulseRange(pathNode.node, abilityData.minAreaRange, abilityData.maxAreaRange, false, true, true);
                break;
            case AbilityData.AreaType.Absolute:
                EffectArea = Pathfinding.GetNodesInAbsoluteRange(pathNode.node, abilityData.minAreaRange, abilityData.maxAreaRange);
                break;
            case AbilityData.AreaType.Line:
                EffectArea = Pathfinding.GetLinePath(GameController.Instance.Grid.nodeList[user.Coords.x, user.Coords.y], 
                    pathNode.node.Coords, false, false, false);
                break;
            default:
                break;
        }
        return EffectArea;
    }

    public virtual bool UseAbility(Unit user, List<PathNode> aoe)
    {
        return true;
    }

    protected virtual bool CommitUseAbility(Unit user)
    {
        var isAbility = true;
        
        if (soundEffect)
        {
            GameController.Instance.AudioManager.UseAudioSourceData(soundEffect);
        }

        if (GameController.Instance.UIController.IsCard)
        {
            if (!(user is MasterUnit)) { return false; }

            isAbility = false;
            ((MasterUnit)user).DeckManager.DiscardCard(GameController.Instance.UIController.selectedAbilityId);
        }

        CalculateDedication(user, isAbility);
            
        user.Animator.SetTrigger("UseSpell");
        GameController.Instance.UIController.SetId(0, false);
        GameController.Instance.SceneController.SetSelectedAbility(null);

        return true;
    }

    protected virtual void CalculateDedication(Unit user, bool isAbility)
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
                // ReSharper disable once PossibleLossOfFraction
                user.UnitStats.DedicationsEnergy[i] += (int)((abilityData.epCost * 2 + abilityData.tpCost) 
                                                             / 5 / aspectAmount / 
                                                             (isAbility ? ABILITY_ASPECT_REDUCTION : 1)) 
                                                       * 5;
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
        var aspectName = string.Empty;

        switch (aspectId)
        {
            case 0:
                aspectName = "Инетры";                
                if (amount > 0)
                {
                    GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.InetraBoost,
                        user.transform.position, $"+{amount} {aspectName}");
                }
                else if (amount < 0)
                {
                    GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.InetraSpent,
                        user.transform.position, $"{amount} {aspectName}");
                }
                break;
            case 1:
                aspectName = "Диатры";                
                if (amount > 0)
                {
                    GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.DiatraBoost,
                        user.transform.position, $"+{amount} {aspectName}");
                }
                else if (amount < 0)
                {
                    GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.DiatraSpent,
                        user.transform.position, $"{amount} {aspectName}");
                }
                break;
            case 2:
                aspectName = "Эфры";
                if (amount > 0)
                {
                    GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.EfraBoost,
                        user.transform.position, $"+{amount} {aspectName}");
                }
                else if (amount < 0)
                {
                    GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.EfraSpent,
                        user.transform.position, $"{amount} {aspectName}");
                }
                break;
            case 3:
                aspectName = "Саквы";
                if (amount > 0)
                {
                    GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.SaquaBoost,
                        user.transform.position, $"+{amount} {aspectName}");
                }
                else if (amount < 0)
                {
                    GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.SaquaSpent,
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
            AspectChangeText(i, abilityData.Dedications[i].Value, user);
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
