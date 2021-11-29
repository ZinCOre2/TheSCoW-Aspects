using System;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    [SerializeField] private GameObject unitDataPanel;

    [Header("Unit Data")]
    [SerializeField] private Image portrait;
    [SerializeField] private TextMeshProUGUI unitNameText;
    [SerializeField] private Image hpBar, epBar;
    [SerializeField] private TextMeshProUGUI hpStateText, epStateText, hpRegenText, epRegenText, moveCostText, powerText, defenceText;
    [SerializeField] private UIAbility[] abilitySlots = new UIAbility[7];
    [SerializeField] private Image[] cardBacks = new Image[6];
    [SerializeField] private TextMeshProUGUI gameTimer;

    public UIAbility[] AbilitySlots { get { return abilitySlots; } private set { abilitySlots = value; } }
    public Unit selectedUnit { get; private set; }
    public int selectedAbilityId { get; private set; } = 0;
    public void SetId(int id) { selectedAbilityId = id; }

    private float _gameTime = 0f;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        SceneController.Instance.OnUnitSelect += ShowUnitUI;
        SceneController.Instance.OnTurnEnd += () =>
        {
            unitDataPanel.SetActive(false);
            if (selectedUnit)
            {
                selectedUnit.OnHealthChanged -= UnitHealthChanged;
                selectedUnit.OnEnergyChanged -= UnitEnergyChanged;
                selectedUnit.OnUnitDeath -= UnitDeath;
                selectedUnit = null;
            }
        };
        foreach (UIAbility uiAbility in abilitySlots)
        {
            uiAbility.OnAbilitySelect += (ability, id) =>
            {
                if (uiAbility.TryGetComponent(out Image selectedImage))
                {
                    if (uiAbility.Id == id)
                    {
                        uiAbility.transform.localScale *= uiAbility.ScaleOnSelected;
                        selectedImage.color = Color.green;

                        if (abilitySlots[selectedAbilityId].TryGetComponent(out Image prevImage))
                        {
                            abilitySlots[selectedAbilityId].transform.localScale /= abilitySlots[selectedAbilityId].ScaleOnSelected;
                            prevImage.color = Color.white;
                        }

                        selectedAbilityId = id;
                    }
                }
            };
        }
        unitDataPanel.SetActive(false);
    }
    public void SetSelectedId(int id)
    {
        if (abilitySlots[id].TryGetComponent(out Image selectedImage))
        {
            abilitySlots[id].transform.localScale *= abilitySlots[id].ScaleOnSelected;
            selectedImage.color = Color.green;

            if (abilitySlots[selectedAbilityId].TryGetComponent(out Image prevImage))
            {
                abilitySlots[selectedAbilityId].transform.localScale /= abilitySlots[selectedAbilityId].ScaleOnSelected;
                prevImage.color = Color.white;
            }

            selectedAbilityId = id;
        }
    }
    private void Update()
    {
        _gameTime += Time.deltaTime;
        gameTimer.text = (int)(_gameTime / 60) + ":" + (int)(_gameTime % 60);
    }

    public void ShowUnitUI(Unit unit)
    {
        if (unit.TeamId - 1 != SceneController.Instance.turnId)
        {
            for (int i = 0; i < 6; i++)
            {
                if (unit.hand[i] != AbilityHolder.AType.None)
                    cardBacks[i].gameObject.SetActive(true);
                else
                    cardBacks[i].gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < 6; i++)
                cardBacks[i].gameObject.SetActive(false);
        }

        if (selectedUnit)
        {
            selectedUnit.OnHealthChanged -= UnitHealthChanged;
            selectedUnit.OnEnergyChanged -= UnitEnergyChanged;
            selectedUnit.OnUnitDeath -= UnitDeath;
        }
        selectedUnit = unit;
        selectedUnit.OnHealthChanged += UnitHealthChanged;
        selectedUnit.OnEnergyChanged += UnitEnergyChanged;
        selectedUnit.OnUnitDeath += UnitDeath;

        unitDataPanel.SetActive(true);

        unitNameText.text = unit.UnitData_.unitName;
        portrait.sprite = unit.UnitData_.portrait;

        hpBar.fillAmount = unit.health / (float)unit.UnitData_.maxHealth;
        hpStateText.text = unit.health.ToString() + "/\n" + unit.UnitData_.maxHealth.ToString();
        hpRegenText.text = "+" + unit.UnitData_.hpRegen.ToString();

        epBar.fillAmount = unit.energy / (float)unit.UnitData_.maxEnergy;
        epStateText.text = unit.energy.ToString() + "/\n" + unit.UnitData_.maxEnergy.ToString();
        epRegenText.text = "+" + unit.UnitData_.epRegen.ToString();

        moveCostText.text = unit.UnitData_.moveCost.ToString();
        powerText.text = unit.UnitData_.power.ToString();
        defenceText.text = unit.UnitData_.defence.ToString();


        if (abilitySlots[selectedAbilityId].TryGetComponent(out Image prevImage))
        {
            abilitySlots[selectedAbilityId].transform.localScale = Vector3.one;
            prevImage.color = Color.white;
        }
        abilitySlots[0].SetAbility(AbilityHolder.Instance.GetAbility(AbilityHolder.AType.Move));
        SceneController.Instance.SetSelectedAbility(abilitySlots[0].ability);
        abilitySlots[0].SetId(0);
        if (abilitySlots[0].TryGetComponent(out Image image))
        {
            abilitySlots[0].transform.localScale = Vector3.one * abilitySlots[0].ScaleOnSelected;
            image.color = Color.green;
        }
        selectedAbilityId = 0;

        for (int i = 1; i < 7; i++)
        {
            abilitySlots[i].gameObject.SetActive(true);
            abilitySlots[i].SetId(i);
            abilitySlots[i].SetAbility(AbilityHolder.Instance.GetAbility(selectedUnit.hand[i - 1]));
        }       
    }

    private void UnitHealthChanged(Unit unit, int newHealth)
    {
        if (selectedUnit == unit)
        {
            hpBar.fillAmount = newHealth / (float)unit.UnitData_.maxHealth;
            hpStateText.text = newHealth.ToString() + "/\n" + unit.UnitData_.maxHealth.ToString();
        }
    }
    private void UnitEnergyChanged(Unit unit, int newEnergy)
    {
        if (selectedUnit == unit)
        {
            epBar.fillAmount = newEnergy / (float)unit.UnitData_.maxEnergy;
            epStateText.text = newEnergy.ToString() + "/\n" + unit.UnitData_.maxEnergy.ToString();
        }
    }
    private void UnitDeath(Unit unit)
    {
        if (selectedUnit == unit)
        {
            selectedUnit = null;
            unitDataPanel.SetActive(false);
        }
    }
}
