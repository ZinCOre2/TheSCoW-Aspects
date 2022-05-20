using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine;

[RequireComponent(typeof(Grid2d))]
[RequireComponent(typeof(Timer))]
public class SceneController : MonoBehaviour
{
    public const int TEAM_AMOUNT = 2;

    public static SceneController Instance;
    public static int[] Counter = new int[TEAM_AMOUNT];

    public event Action<Unit> OnUnitSelect;
    public event Action OnTurnEnd;

    [Header("Game Data")]
    [SerializeField] private Grid2d grid;
    [SerializeField] private Timer turnTimer;
    [SerializeField] private float turnDuration = 75f;
    [Header("UI Data")]
    [SerializeField] private Image[] blueTeamSlots = new Image[3];
    [SerializeField] private Image[] redTeamSlots = new Image[3];
    [SerializeField] private Image[] timerFrame = new Image[2];
    [SerializeField] private TextMeshProUGUI[] timerText = new TextMeshProUGUI[2];
    [SerializeField] private Image victoryScreen;
    [SerializeField] private TextMeshProUGUI victoryText;

    public Grid2d Grid { get { return grid; } private set { grid = value; } }
    public Timer TurnTimer { get { return turnTimer; } private set { turnTimer = value; } }
    public int turnId { get; private set; } = 0;

    public Unit selectedUnit { get; private set; }
    private Ability _selectedAbility = new Ability();
    private List<PathNode> _nodesInRange = new List<PathNode>();
    private List<PathNode> _aoe = new List<PathNode>();

    Ray _ray;
    RaycastHit _hitInfo;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        for (int i = 0; i < TEAM_AMOUNT; i++)
        {
            Counter[i] = 0;
        }
    }
    private void Start()
    {
        foreach (UIAbility uiAbility in UIController.Instance.AbilitySlots)
        {
            uiAbility.OnAbilitySelect += (Ability _ability, int id) =>
            {
                if (uiAbility.ability != null && uiAbility.ability == _ability && uiAbility.Id == id)
                {
                    _selectedAbility = _ability;
                    UnmarkNodes();
                    _nodesInRange = _ability.GetNodesInRange(selectedUnit);
                    MarkNodes();
                }
            };
        }
        OnUnitSelect += (Unit unit) =>
        {
            selectedUnit = unit;

            UnmarkNodes();
            if (unit.TeamId - 1 == turnId)
            {
                _selectedAbility = AbilityHolder.Instance.GetAbility(AbilityHolder.AType.Move);

                _nodesInRange = _selectedAbility.GetNodesInRange(selectedUnit);
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
            SceneManager.LoadScene(0); // Main menu
        }
        if (!IsMouseOverUI())
        {
            _ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(_ray, out _hitInfo))
            {
                // Hovering cursor - always check area of effect
                if (_hitInfo.collider.gameObject.TryGetComponent(out Node node))
                {
                    if (selectedUnit && selectedUnit.TeamId != 0 && !selectedUnit.usingAbility)
                    {
                        if (selectedUnit.TeamId - 1 == turnId)
                        {
                            // Clear aoe/path
                            UnmarkNodes();

                            // Find selected node. If not in range, do nothing
                            foreach (PathNode pathNode in _nodesInRange)
                            {
                                if (pathNode.node == node)
                                {
                                    _aoe = _selectedAbility.GetAoe(selectedUnit, pathNode);
                                    break;
                                }
                            }

                            MarkNodes();
                        }
                    }
                }
                if (_hitInfo.collider.gameObject.TryGetComponent(out Unit unit))
                {
                    if (selectedUnit && selectedUnit.TeamId != 0 && !selectedUnit.usingAbility)
                    {
                        if (selectedUnit.TeamId - 1 == turnId)
                        {
                            // Clear aoe/path
                            UnmarkNodes();

                            // Find selected node. If not in range, do nothing
                            foreach (PathNode pathNode in _nodesInRange)
                            {
                                if (pathNode.node.Coords == unit.Coords)
                                {
                                    _aoe = _selectedAbility.GetAoe(selectedUnit, pathNode);
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
                    if (_hitInfo.collider.gameObject.TryGetComponent(out Unit unitSelect) && unit.TeamId != 0 && unit.health > 0)
                    {
                        OnUnitSelect?.Invoke(unitSelect);
                    }
                }

                // When pressed RMB on node - if in range of ability, use it.
                if (Input.GetMouseButtonDown(1))
                {
                    if (selectedUnit && selectedUnit.TeamId - 1 == turnId)
                    {
                        if (_hitInfo.collider.gameObject.TryGetComponent(out Node nodeTarget))
                        {
                            if (selectedUnit.TeamId != 0 && !selectedUnit.usingAbility)
                            {
                                UnmarkNodes();
                                // Find selected node. If not in range, do nothing
                                foreach (PathNode pathNode in _nodesInRange)
                                    if (pathNode.node == nodeTarget)
                                    {
                                        _aoe = _selectedAbility.GetAoe(selectedUnit, pathNode);
                                        _selectedAbility.UseAbility(selectedUnit, _aoe);

                                        _selectedAbility = AbilityHolder.Instance.GetAbility(AbilityHolder.AType.Move);
                                        _nodesInRange = _selectedAbility.GetNodesInRange(selectedUnit);

                                        break;
                                    }
                                MarkNodes();
                            }
                        }
                        if (_hitInfo.collider.gameObject.TryGetComponent(out Unit unitTarget))
                        {
                            if (selectedUnit.TeamId != 0 && !selectedUnit.usingAbility)
                            {
                                UnmarkNodes();
                                // Find selected node. If not in range, do nothing
                                foreach (PathNode pathNode in _nodesInRange)
                                    if (pathNode.node == grid.nodeList[unitTarget.Coords.x, unitTarget.Coords.y])
                                    {
                                        _aoe = _selectedAbility.GetAoe(selectedUnit, pathNode);
                                        _selectedAbility.UseAbility(selectedUnit, _aoe);

                                        _selectedAbility = AbilityHolder.Instance.GetAbility(AbilityHolder.AType.Move);
                                         _nodesInRange = _selectedAbility.GetNodesInRange(selectedUnit);

                                        break;
                                    }
                                MarkNodes();
                            }
                        }
                    }
                }
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
            Unit unitInRange;
            unitInRange = grid.GetUnitOnNode(pathNode.node.Coords);

            pathNode.node.MarkCustom(markColor);
        }

        // Mark nodes in aoe
        foreach (PathNode pathNode in _aoe)
        {
            Unit unitInRange;
            unitInRange = grid.GetUnitOnNode(pathNode.node.Coords);
            pathNode.node.MarkCustom(Color.yellow);
        }
    }

    public void UnitDeath(Unit unit)
    {
        if (selectedUnit == unit)
        {
            selectedUnit = null;
            UnmarkNodes();
        }

        grid.unitList.Remove(unit);

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

        foreach (Unit unit in grid.unitList)
        {
            if (unit.TeamId != 0)
            {
                unit.ChangeHealth(unit.UnitData_.hpRegen);
                unit.ChangeEnergy(unit.UnitData_.epRegen);

                unit.DrawCard();
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
            _nodesInRange = _selectedAbility.GetNodesInRange(selectedUnit);
        }
        else
        {
            _nodesInRange = new List<PathNode>();
        }
    }

    public static bool IsMouseOverUI() { return EventSystem.current.IsPointerOverGameObject(); }
}