using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine;

[RequireComponent(typeof(Timer))]
public class SceneController : MonoBehaviour
{
    public const int TEAM_AMOUNT = 2;
    
    public static int[] Counter = new int[TEAM_AMOUNT];

    public event Action<Unit> OnUnitSelect;
    public event Action OnTurnEnd;

    [Header("Game Data")]
    [SerializeField] private Timer turnTimer;
    [SerializeField] private float turnDuration = 75f;
    [Header("UI Data")]
    [SerializeField] private Image[] blueTeamSlots = new Image[3];
    [SerializeField] private Image[] redTeamSlots = new Image[3];
    [SerializeField] private Image[] timerFrame = new Image[2];
    [SerializeField] private TextMeshProUGUI[] timerText = new TextMeshProUGUI[2];
    [SerializeField] private Image victoryScreen;
    [SerializeField] private TextMeshProUGUI victoryText;
    
    public Timer TurnTimer { get { return turnTimer; } private set { turnTimer = value; } }
    public int turnId { get; private set; } = 0;

    [HideInInspector] public Unit SelectedUnit;
    private Ability _selectedAbility = new Ability();
    private List<PathNode> _nodesInRange = new List<PathNode>();
    private List<PathNode> _aoe = new List<PathNode>();

    Ray _ray;
    RaycastHit _hitInfo;

    private void Awake()
    {
        for (int i = 0; i < TEAM_AMOUNT; i++)
        {
            Counter[i] = 0;
        }
    }
    private void Start()
    {
        foreach (var uiAbility in GameController.Instance.UIController.UnitDataPanel.AbilitySlots)
        {
            uiAbility.OnAbilitySelect += (Ability _ability, int id) =>
            {
                if (uiAbility.ability != null && uiAbility.ability == _ability && uiAbility.Id == id)
                {
                    _selectedAbility = _ability;
                    UnmarkNodes();
                    _nodesInRange = _ability.GetNodesInRange(SelectedUnit);
                    MarkNodes();
                }
            };
        }
        foreach (var uiAbility in GameController.Instance.UIController.UnitDataPanel.CardSlots)
        {
            uiAbility.OnAbilitySelect += (Ability _ability, int id) =>
            {
                if (uiAbility.ability != null && uiAbility.ability == _ability && uiAbility.Id == id)
                {
                    _selectedAbility = _ability;
                    UnmarkNodes();
                    _nodesInRange = _ability.GetNodesInRange(SelectedUnit);
                    MarkNodes();
                }
            };
        }
        OnUnitSelect += (Unit unit) =>
        {
            SelectedUnit = unit;

            UnmarkNodes();
            if (unit.TeamId - 1 == turnId)
            {
                _selectedAbility = GameController.Instance.AbilityHolder.GetAbility(AbilityHolder.AbilityType.Move);

                _nodesInRange = _selectedAbility.GetNodesInRange(SelectedUnit);
            }
            else
            {
                _nodesInRange.Clear();
            }
            MarkNodes();
        };
        OnTurnEnd += UpdateTurnData;

        turnTimer.Reset(turnDuration);
        turnTimer.OnTimerChanged += () => { timerText[turnId].text = ((int)turnTimer.TimeLeft).ToString(); };
        turnTimer.OnTimerElapsed += EndTurn;

        timerFrame[1].gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0); // or Pause
        }
        if (!IsMouseOverUI())
        {
            _ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(_ray, out _hitInfo))
            {
                UnmarkNodes();
                MarkNodes();
                
                // Hovering cursor - always check area of effect
                if (_hitInfo.collider.gameObject.TryGetComponent(out Node node))
                {
                    if (SelectedUnit && SelectedUnit.TeamId != 0 && !SelectedUnit.usingAbility)
                    {
                        if (SelectedUnit.TeamId - 1 == turnId)
                        {
                            // Clear aoe/path
                            UnmarkNodes();

                            // Find selected node. If not in range, do nothing
                            foreach (PathNode pathNode in _nodesInRange)
                            {
                                if (pathNode.node == node)
                                {
                                    _aoe = _selectedAbility.GetAoe(SelectedUnit, pathNode);
                                    break;
                                }
                            }

                            MarkNodes();
                        }
                    }
                }
                if (_hitInfo.collider.gameObject.TryGetComponent(out Unit unit))
                {
                    if (SelectedUnit && SelectedUnit.TeamId != 0 && !SelectedUnit.usingAbility)
                    {
                        if (SelectedUnit.TeamId - 1 == turnId)
                        {
                            // Clear aoe/path
                            UnmarkNodes();

                            // Find selected node. If not in range, do nothing
                            foreach (PathNode pathNode in _nodesInRange)
                            {
                                if (pathNode.node.Coords == unit.Coords)
                                {
                                    _aoe = _selectedAbility.GetAoe(SelectedUnit, pathNode);
                                    break;
                                }
                            }

                            MarkNodes();
                        }
                    }
                }

                // When pressed LMB on unit - set them as selected and redraw all UI acccordingly
                if (Input.GetMouseButtonDown(0))
                {
                    if (_hitInfo.collider.gameObject.TryGetComponent(out Unit unitSelect) && unit.TeamId != 0 && unit.UnitStats.Health > 0)
                    {
                        OnUnitSelect?.Invoke(unitSelect);
                    }
                }

                // When pressed RMB on node - if in range of ability, use it.
                if (Input.GetMouseButtonDown(1))
                {
                    if (SelectedUnit && SelectedUnit.TeamId - 1 == turnId)
                    {
                        if (_hitInfo.collider.gameObject.TryGetComponent(out Node nodeTarget))
                        {
                            if (SelectedUnit.TeamId != 0 && !SelectedUnit.usingAbility)
                            {
                                UnmarkNodes();
                                // Find selected node. If not in range, do nothing
                                foreach (PathNode pathNode in _nodesInRange)
                                {
                                    if (pathNode.node == nodeTarget)
                                    {
                                        _aoe = _selectedAbility.GetAoe(SelectedUnit, pathNode);
                                        _selectedAbility.UseAbility(SelectedUnit, _aoe);

                                        _selectedAbility = GameController.Instance.AbilityHolder.GetAbility(AbilityHolder.AbilityType.Move);
                                        _nodesInRange = _selectedAbility.GetNodesInRange(SelectedUnit);

                                        break;
                                    }
                                }

                                MarkNodes();
                            }
                        }
                        if (_hitInfo.collider.gameObject.TryGetComponent(out Unit unitTarget))
                        {
                            if (SelectedUnit.TeamId != 0 && !SelectedUnit.usingAbility)
                            {
                                UnmarkNodes();
                                // Find selected node. If not in range, do nothing
                                foreach (PathNode pathNode in _nodesInRange)
                                {
                                    if (pathNode.node == GameController.Instance.Grid.nodeList[unitTarget.Coords.x, unitTarget.Coords.y])
                                    {
                                        _aoe = _selectedAbility.GetAoe(SelectedUnit, pathNode);
                                        _selectedAbility.UseAbility(SelectedUnit, _aoe);

                                        _selectedAbility = GameController.Instance.AbilityHolder.GetAbility(AbilityHolder.AbilityType.Move);
                                        _nodesInRange = _selectedAbility.GetNodesInRange(SelectedUnit);

                                        break;
                                    }
                                }

                                MarkNodes();
                            }
                        }
                    }
                }
            }
        }
    }

    private void MarkNodes()
    {
        // Mark nodes in range
        Color markColor;
        if (_selectedAbility is Move)
            markColor = Color.blue;
        else
            markColor = Color.green;
        
        foreach (PathNode pathNode in _nodesInRange)
        {
            if (GameController.Instance.Grid.NodeOccupied(pathNode.node.Coords))
            {
                if (GameController.Instance.Grid.GetUnitOnNode(pathNode.node.Coords) != null)
                {
                    pathNode.node.MarkCustom(Color.red);
                }
                else
                {
                    pathNode.node.MarkCustom(Color.cyan);
                }
            }
            else
            {
                pathNode.node.MarkCustom(markColor);
            }
        }

        // Mark nodes in aoe
        foreach (PathNode pathNode in _aoe)
        {
            if (GameController.Instance.Grid.NodeOccupied(pathNode.node.Coords))
            {
                if (GameController.Instance.Grid.GetUnitOnNode(pathNode.node.Coords) != null)
                {
                    pathNode.node.MarkCustom(Color.red);
                }
                else
                {
                    pathNode.node.MarkCustom(Color.cyan);
                }
            }
            else
            {
                pathNode.node.MarkCustom(Color.yellow);
            }
        }
    }
    private void UnmarkNodes()
    {
        foreach (PathNode pathNodeAoe in _aoe)
            pathNodeAoe.node.Unmark();

        foreach (PathNode pathNode in _nodesInRange)
            pathNode.node.Unmark();
    }

    public void UnitDeath(Unit unit)
    {
        if (SelectedUnit == unit)
        {
            SelectedUnit = null;
            UnmarkNodes();
        }

        GameController.Instance.EntityManager.RemoveEntity(unit);

        Counter[unit.TeamId - 1]--;
        Debug.Log("Team " + (unit.TeamId - 1) + " counter: " + Counter[unit.TeamId - 1]);
        switch (unit.TeamId)
        {
            case 1:
                blueTeamSlots[Counter[0]].gameObject.SetActive(false);
                if (Counter[0] <= 0)
                {
                    ShowVictoryScreen(1);
                }
                break;
            case 2:
                redTeamSlots[Counter[1]].gameObject.SetActive(false);
                if (Counter[1] <= 0)
                {
                    ShowVictoryScreen(2);
                }
                break;
        }
    }
    public void ShowVictoryScreen(int teamId)
    {
        victoryScreen.gameObject.SetActive(true);
        switch (teamId)
        {
            case 1:
                victoryText.color = Color.red;
                victoryText.text = "Победа красных!";
                Debug.Log("Red win!");
                break;
            case 2:
                victoryText.color = Color.blue;
                victoryText.text = "Победа синих!";
                Debug.Log("Blue win!");
                break;
        }
    }

    public void EndTurn() { OnTurnEnd?.Invoke(); }
    public void UpdateTurnData()
    {
        UnmarkNodes();

        foreach (Unit unit in GameController.Instance.EntityManager.Units)
        {
            if (unit.TeamId != 0)
            {
                unit.ChangeHealth(unit.UnitData.HPRegen);
                unit.ChangeEnergy(unit.UnitData.EPRegen);
                unit.ChangeTime(unit.UnitData.MaxTime / 2);

                if (unit is MasterUnit masterUnit)
                {
                    masterUnit.DeckManager.DrawCard();
                }
            }
        }

        turnTimer.Reset();

        turnId++;
        if (turnId >= TEAM_AMOUNT)
        {
            turnId = 0;
        }

        switch (turnId)
        {
            case 0:
                timerFrame[0].gameObject.SetActive(true);
                timerFrame[1].gameObject.SetActive(false);
                break;
            case 1:
                timerFrame[0].gameObject.SetActive(false);
                timerFrame[1].gameObject.SetActive(true);
                break;
        }
    }

    public void SetSelectedAbility(Ability newAbility)
    {
        UnmarkNodes();
        _selectedAbility = newAbility;
        if (_selectedAbility != null)
        {
            _nodesInRange = _selectedAbility.GetNodesInRange(SelectedUnit);
        }
        else
        {
            _nodesInRange = new List<PathNode>();
        }
    }

    public static bool IsMouseOverUI() { return EventSystem.current.IsPointerOverGameObject(); }
}