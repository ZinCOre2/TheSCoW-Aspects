using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine;
using Object = System.Object;

[RequireComponent(typeof(Timer))]
public class SceneController : MonoBehaviour
{
    public const int TEAM_AMOUNT = 2;
    public const int TICK_UPDATE_DELAY = 4;
    
    public static int[] Counter = new int[TEAM_AMOUNT];

    public event Action<Unit> OnUnitSelect;
    public event Action OnTurnEnd;
    public event Action OnAbilityUsed;

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
    private List<PathNode> _usageArea = new List<PathNode>();
    private List<PathNode> _effectArea = new List<PathNode>();

    private Ray _ray;
    private RaycastHit _hitInfo;
    private int _tickDelay;

    private void Awake()
    {
        for (int i = 0; i < TEAM_AMOUNT; i++)
        {
            Counter[i] = 0;
        }
    }
    private void Start()
    {
        foreach (UIAbility uiAbility in GameController.Instance.UIController.UnitDataPanel.AbilitySlots)
        {
            uiAbility.OnAbilitySelect += (Ability ability, int id) =>
            {
                if (uiAbility.ability != null && uiAbility.ability == ability && uiAbility.Id == id)
                {
                    _selectedAbility = ability;

                    ResetUsageArea();
                }
            };
        }
        foreach (var uiCard in GameController.Instance.UIController.UnitDataPanel.CardSlots)
        {
            uiCard.OnAbilitySelect += (Ability ability, int id) =>
            {
                if (uiCard.ability != null && uiCard.ability == ability && uiCard.Id == id)
                {
                    _selectedAbility = ability;

                    ResetUsageArea();
                }
            };
        }
        OnUnitSelect += (Unit unit) =>
        {
            SelectedUnit = unit;
            SetSelectedAbility(null);

            UnmarkNodes();
            if (unit.TeamId - 1 == turnId)
            {
                _selectedAbility = GameController.Instance.AbilityHolder.GetAbility(AbilityHolder.AbilityType.Move);

                ResetUsageArea();
            }
            else
            {
                _usageArea.Clear();
                _effectArea.Clear();
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
        if (IsMouseOverUI()) { return; }

        _ray = GameController.Instance.CameraController.Camera.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(_ray, out _hitInfo)) { return; }
        
        AutoUpdateAreas();
        // Hovering cursor - always check area of effect
        if (_hitInfo.collider.gameObject.TryGetComponent(out Entity hoveredEntity))
        {
            if (SelectedUnit && !SelectedUnit.usingAbility)
            {
                if (SelectedUnit.TeamId - 1 == turnId)
                {
                    UnmarkNodes();

                    // Find selected node. If not in range, do nothing
                    var targetPathNode = _usageArea.Find(n => n.node.Coords == hoveredEntity.Coords);

                    if (targetPathNode != null && targetPathNode.node.Coords == hoveredEntity.Coords)
                    {
                        _effectArea.Clear();
                        _effectArea = _selectedAbility.GetEffectArea(SelectedUnit, targetPathNode);
                    }
                        
                    MarkNodes();
                }
            }
        }

        // When pressed LMB on unit - set them as selected and redraw all UI accordingly
        if (Input.GetMouseButtonDown(0))
        {
            if (_hitInfo.collider.gameObject.TryGetComponent(out Entity selectedEntity))
            {
                var foundUnit = GameController.Instance.Grid.GetUnitOnNode(selectedEntity.Coords);
                
                if (foundUnit && foundUnit.UnitStats.Health > 0)
                {
                    OnUnitSelect?.Invoke(foundUnit);
                }
            }
        }

        // When pressed RMB on node - if in range of ability, use it.
        if (Input.GetMouseButtonDown(1))
        {
            if (SelectedUnit && SelectedUnit.TeamId - 1 == turnId)
            {
                if (_hitInfo.collider.gameObject.TryGetComponent(out Entity pressedEntity))
                {
                    if (SelectedUnit.TeamId != 0 && !SelectedUnit.usingAbility)
                    {
                        UnmarkNodes();
                        // Find selected node. If not in range, do nothing
                        if (_usageArea.Any(n => n.node.Coords == pressedEntity.Coords))
                        {
                            _selectedAbility.UseAbility(SelectedUnit, _effectArea);
                            OnAbilityUsed?.Invoke();
                                
                            _selectedAbility = GameController.Instance.AbilityHolder.GetAbility(AbilityHolder.AbilityType.Move);
                                
                            ResetUsageArea();
                        }

                        MarkNodes();
                    }
                }
            }
        }
    }

    public void AutoUpdateAreas()
    {
        if (SelectedUnit == null || _selectedAbility == null) { return; }
        if (_tickDelay < TICK_UPDATE_DELAY)
        {
            _tickDelay++;
            return;
        }

        _tickDelay -= TICK_UPDATE_DELAY;
        
        UnmarkNodes();
        
        _usageArea.Clear();
        _usageArea = _selectedAbility.GetUsageArea(SelectedUnit);

        MarkNodes();
    }
    public void ResetUsageArea()
    {
        if (SelectedUnit == null || _selectedAbility == null) { return; }
        
        UnmarkNodes();
        
        _usageArea.Clear();
        _usageArea = _selectedAbility.GetUsageArea(SelectedUnit);
        
        MarkNodes();
    }

    private void MarkNodes()
    {
        // Mark nodes in range
        Color markColor;
        markColor = _selectedAbility is Move ? Color.blue : Color.green;
        
        foreach (PathNode pathNode in _usageArea)
        {
            pathNode.node.MarkCustom(markColor);
        }

        // Mark nodes in aoe
        foreach (PathNode pathNode in _effectArea)
        {
            pathNode.node.MarkCustom(Color.yellow);
        }
    }
    public void UnmarkNodes()
    {
        foreach (PathNode pathNode in _usageArea)
            pathNode.node.Unmark();
        
        foreach (PathNode pathNodeAoe in _effectArea)
            pathNodeAoe.node.Unmark();
    }

    public void UnitDeath(Unit unit)
    {
        if (SelectedUnit == unit)
        {
            SelectedUnit = null;
            UnmarkNodes();
        }

        GameController.Instance.EntityManager.RemoveEntity(unit);
        
        if (!(unit is MasterUnit)) { return; }

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

    public void EndTurn()
    {
        turnId++;
        if (turnId >= TEAM_AMOUNT)
        {
            turnId = 0;
        }
        
        OnTurnEnd?.Invoke();
    }
    public void UpdateTurnData()
    {
        SelectedUnit = null;
        SetSelectedAbility(null);

        foreach (Unit unit in GameController.Instance.EntityManager.Units)
        {
            if (unit.TeamId != 0)
            {
                unit.ChangeHealth(unit.UnitStats.HealthRegen);
                unit.ChangeEnergy(unit.UnitStats.EnergyRegen);
                unit.ChangeTime(unit.UnitStats.MaxTime / 10 * 5);

                if (unit is MasterUnit masterUnit)
                {
                    masterUnit.DeckManager.DrawCard();
                }
            }
        }

        turnTimer.Reset();

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
        _usageArea.Clear();
        
        if (_selectedAbility != null)
        {
            _usageArea = _selectedAbility.GetUsageArea(SelectedUnit);
        }
    }
    
    public void SetSelectedAbility(AbilityHolder.AbilityType abilityType)
    {
        var newAbility = GameController.Instance.AbilityHolder.GetAbility(abilityType);
        
        UnmarkNodes();
        _selectedAbility = newAbility;
        _usageArea.Clear();
        
        if (_selectedAbility != null)
        {
            _usageArea = _selectedAbility.GetUsageArea(SelectedUnit);
        }
    }

    public static bool IsMouseOverUI() { return EventSystem.current.IsPointerOverGameObject(); }
}