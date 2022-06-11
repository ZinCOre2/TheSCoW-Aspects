using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    [HideInInspector] public PhysicalEntity HoveredSelectable;
    [HideInInspector] public PhysicalEntity SelectedSelectable;

    [HideInInspector] public AbilityHolder.AbilityType HoveredAbility;
    [HideInInspector] public AbilityHolder.AbilityType SelectedAbility;

    [SerializeField] private Material basicNodeMaterial;
    [SerializeField] private Material moveUsageNodeMaterial;
    [SerializeField] private Material usageAreaNodeMaterial;
    [SerializeField] private Material effectAreaNodeMaterial;
    [SerializeField] private Material occupiedNodeMaterial;

    [HideInInspector] public List<Node> UsageArea = new List<Node>();
    [HideInInspector] public List<Node> EffectArea = new List<Node>();

    private SelectionStage _selectionStage = SelectionStage.None;

    private void Update()
    {
        var ray = GameController.Instance.CameraController.Camera.ScreenPointToRay(Input.mousePosition);

        if (IsMouseOverUI()) { return; }
        if (!Physics.Raycast(ray, out var hitInfo)) { return; }

        if (hitInfo.collider.TryGetComponent(out Unit unit))
        {
            HoverEntityStageOne(unit);
            // if (unit.UnitStats.MasterId == GameController.Instance.TurnManager.CurrentMasterId)
            // {
            //     // GetAdditionalInfoAndAbilityPermissions
            // }
            // if (unit.UnitStats.TeamId == GameController.Instance.TurnManager.CurrentMasterId)
            // {
            //     // GetAdditionalInfo
            //     // No ability permissions
            // }
        }
        if (Input.GetMouseButtonDown(0) && hitInfo.collider.TryGetComponent(out unit))
        {
            SelectEntityStageOne(unit);
        }

        if (hitInfo.collider.TryGetComponent(out Node node))
        {
            //HoverTargetStageThree(node);
        }

        if (_selectionStage < SelectionStage.Ability) { return; }

        if (Input.GetMouseButtonDown(0) && hitInfo.collider.TryGetComponent(out PhysicalEntity entity))
        {
            SelectEntityStageOne(entity);
        }
    }

    public static bool IsMouseOverUI() { return EventSystem.current.IsPointerOverGameObject(); }
    public void ResetSelections(SelectionStage selection)
    {
        if (selection < SelectionStage.Target)
        {
            // Clear material
            foreach (var uNode in UsageArea)
            {
                uNode.MarkCustom(Color.white);
            }
            UsageArea.Clear();

            foreach (var eNode in EffectArea)
            {
                eNode.MarkCustom(Color.white);
            }
            EffectArea.Clear();
        }

        if (selection < SelectionStage.Ability)
        {
            HoveredAbility = AbilityHolder.AbilityType.None;
            SelectedAbility = AbilityHolder.AbilityType.None;
        }

        if (selection < SelectionStage.Selectable)
        {
            HoveredSelectable = null;
            SelectedSelectable = null;
        }
    }
    private bool GetCameraRaycastHitInfo(out RaycastHit hitInfo)
    {
        Ray ray = GameController.Instance.CameraController.Camera.ScreenPointToRay(Input.mousePosition);

        return Physics.Raycast(ray, out hitInfo);
    }

    private void SelectEntityStageOne(PhysicalEntity entity)
    {
        ResetSelections(SelectionStage.Selectable);

        _selectionStage = SelectionStage.Selectable;
        SelectedSelectable = entity;

        Debug.Log($"Selectable {entity.name}; Coords: {entity.Coords.x}, {entity.Coords.y}.");

        if (entity is Unit)
        {
            SelectAbilityStageTwo(AbilityHolder.AbilityType.Move);
        }
    }
    private void HoverEntityStageOne(PhysicalEntity entity)
    {
        if (_selectionStage < SelectionStage.Selectable) { return; }


    }

    private void SelectAbilityStageTwo(AbilityHolder.AbilityType ability)
    {
        ResetSelections(SelectionStage.Ability);

        _selectionStage = SelectionStage.Ability;
        SelectedAbility = ability;

        Debug.Log($"Ability {nameof(ability)} selected.");

        var abilityPrefab = GameController.Instance.AbilityHolder.GetAbility(ability);
        var abilityData = abilityPrefab.AbilityData;

        UsageArea.Clear();
        // UsageArea = Pathfinding.GetNodesInArea(abilityPrefab.SearchType, abilityPrefab.)
    }
    // private void HoverAbilityStageTwo(BaseAbilityUI abilityUI)
    // {
    //     if (_selectionStage < SelectionStage.Ability) { return; }
    //
    //
    // }

    private void SelectTargetStageThree(PhysicalEntity target)
    {
        ResetSelections(SelectionStage.Target);
        _selectionStage = SelectionStage.Target;

        Debug.Log($"Ability targeted at coords {target.Coords.x}, {target.Coords.y}.");

        UsageArea.Clear();
        // UsageArea = Pathfinding.GetNodesInArea(abilityPrefab.UsageSearchType, abilityPrefab.minRange, abilityPrefab.maxRange)
    }
    private void HoverTargetStageThree(Node target)
    {
        //if (_selectionStage < SelectionStage.Target) { return; }
        foreach (var uNode in UsageArea)
        {
            uNode.MarkCustom(Color.white);
        }
        UsageArea.Clear();

        foreach (var eNode in EffectArea)
        {
            eNode.MarkCustom(Color.white);
        }
        EffectArea.Clear();

        EffectArea.Add(target);

        var usageColor = SelectedAbility is AbilityHolder.AbilityType.Move ? Color.blue : Color.yellow;
        foreach (var node in UsageArea)
        {
            node.MarkCustom(usageColor);
        }

        foreach (var node in EffectArea)
        {
            node.MarkCustom(Color.yellow);
        }

        // EffectArea = Pathfinding.GetNodesInArea(abilityPrefab.EffectSearchType, abilityPrefab.minRange, abilityPrefab.maxRange)
    }
}
