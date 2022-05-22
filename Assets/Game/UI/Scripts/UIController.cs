using System;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public Transform WorldUIParent;
    [SerializeField] private UnitDataPanel unitDataPanel;

    [Header("Unit Data")]
    [SerializeField] private Image portrait;
    [SerializeField] private TextMeshProUGUI unitNameText;
    [SerializeField] private Image hpBar, epBar, tpBar;
    [SerializeField] private TextMeshProUGUI hpStateText, epStateText, tpStateText, hpRegenText, epRegenText, powerText, defenceText;
    [SerializeField] private UIAbility[] abilitySlots = new UIAbility[7];
    [SerializeField] private Image[] cardBacks = new Image[6];
    
    
    [SerializeField] private TextMeshProUGUI gameTimer;

    public UIAbility[] AbilitySlots { get { return abilitySlots; } private set { abilitySlots = value; } }
    public Unit selectedUnit { get; private set; }
    public int selectedAbilityId { get; private set; } = 0;
    public void SetId(int id) { selectedAbilityId = id; }

    private float _gameTime = 0f;
    
    private void Start()
    {
        GameController.Instance.SceneController.OnUnitSelect += ShowUnitUI;
        GameController.Instance.SceneController.OnTurnEnd += () =>
        {
            unitDataPanel.gameObject.SetActive(false);
            if (selectedUnit)
            {
                selectedUnit.OnHealthChanged -= UnitHealthChanged;
                selectedUnit.OnEnergyChanged -= UnitEnergyChanged;
                selectedUnit.OnTimeChanged -= UnitTimeChanged;
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
                        uiAbility.ScaledCardHolder.localScale *= uiAbility.ScaleOnSelected;
                        selectedImage.color = Color.green;

                        if (abilitySlots[selectedAbilityId].TryGetComponent(out Image prevImage))
                        {
                            abilitySlots[selectedAbilityId].ScaledCardHolder.localScale /= abilitySlots[selectedAbilityId].ScaleOnSelected;
                            prevImage.color = Color.white;
                        }

                        selectedAbilityId = id;
                    }
                }
            };
        }
        unitDataPanel.gameObject.SetActive(false);
    }
    public void SetSelectedId(int id)
    {
        if (abilitySlots[id].TryGetComponent(out Image selectedImage))
        {
            abilitySlots[id].ScaledCardHolder.localScale *= abilitySlots[id].ScaleOnSelected;
            selectedImage.color = Color.green;

            if (abilitySlots[selectedAbilityId].TryGetComponent(out Image prevImage))
            {
                abilitySlots[selectedAbilityId].ScaledCardHolder.localScale /= abilitySlots[selectedAbilityId].ScaleOnSelected;
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
        if (unit.TeamId - 1 != GameController.Instance.SceneController.turnId)
        {
            for (int i = 0; i < 6; i++)
            {
                if (unit.DeckManager.Hand[i] != AbilityHolder.AType.None)
                {
                    cardBacks[i].gameObject.SetActive(true);
                }
                else
                {
                    cardBacks[i].gameObject.SetActive(false);
                }
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
            selectedUnit.OnTimeChanged -= UnitTimeChanged;
            selectedUnit.OnUnitDeath -= UnitDeath;
        }
        selectedUnit = unit;
        selectedUnit.OnHealthChanged += UnitHealthChanged;
        selectedUnit.OnEnergyChanged += UnitEnergyChanged;
        selectedUnit.OnTimeChanged += UnitTimeChanged;
        selectedUnit.OnUnitDeath += UnitDeath;

        unitDataPanel.gameObject.SetActive(true);

        unitNameText.text = unit.UnitData.unitName;
        portrait.sprite = unit.UnitData.portrait;

        hpBar.fillAmount = unit.health / (float)unit.UnitData.maxHealth;
        hpStateText.text = unit.health.ToString() + "\n/\n" + unit.UnitData.maxHealth.ToString();
        hpRegenText.text = "+" + unit.UnitData.hpRegen.ToString();

        epBar.fillAmount = unit.energy / (float)unit.UnitData.maxEnergy;
        epStateText.text = unit.energy.ToString() + "\n/\n" + unit.UnitData.maxEnergy.ToString();
        epRegenText.text = "+" + unit.UnitData.epRegen.ToString();
        
        tpBar.fillAmount = unit.time / (float)unit.UnitData.maxTime;
        tpStateText.text = unit.time.ToString() + "\n/\n" + unit.UnitData.maxTime.ToString();
        
        powerText.text = unit.UnitData.power.ToString();
        defenceText.text = unit.UnitData.defence.ToString();
        
        if (abilitySlots[selectedAbilityId].TryGetComponent(out Image prevImage))
        {
            abilitySlots[selectedAbilityId].transform.localScale = Vector3.one;
            prevImage.color = Color.white;
        }
        abilitySlots[0].SetAbility(GameController.Instance.AbilityHolder.GetAbility(AbilityHolder.AType.Move));
        GameController.Instance.SceneController.SetSelectedAbility(abilitySlots[0].ability);
        abilitySlots[0].SetId(0);
        if (abilitySlots[0].TryGetComponent(out Image image))
        {
            abilitySlots[0].ScaledCardHolder.localScale = Vector3.one * abilitySlots[0].ScaleOnSelected;
            image.color = Color.green;
        }
        selectedAbilityId = 0;

        for (int i = 1; i < 7; i++)
        {
            abilitySlots[i].gameObject.SetActive(true);
            abilitySlots[i].SetId(i);
            abilitySlots[i].SetAbility(GameController.Instance.AbilityHolder.GetAbility(selectedUnit.DeckManager.Hand[i - 1]));
        }       
    }

    private void UnitHealthChanged(Unit unit, int newHealth)
    {
        if (selectedUnit == unit)
        {
            hpBar.fillAmount = newHealth / (float)unit.UnitData.maxHealth;
            hpStateText.text = newHealth.ToString() + "\n/\n" + unit.UnitData.maxHealth.ToString();
        }
    }
    private void UnitEnergyChanged(Unit unit, int newEnergy)
    {
        if (selectedUnit == unit)
        {
            epBar.fillAmount = newEnergy / (float)unit.UnitData.maxEnergy;
            epStateText.text = newEnergy.ToString() + "\n/\n" + unit.UnitData.maxEnergy.ToString();
        }
    }
    private void UnitTimeChanged(Unit unit, int newTime)
    {
        if (selectedUnit == unit)
        {
            tpBar.fillAmount = newTime / (float)unit.UnitData.maxTime;
            tpStateText.text = newTime.ToString() + "\n/\n" + unit.UnitData.maxTime.ToString();
        }
    }
    private void UnitDeath(Unit unit)
    {
        if (selectedUnit == unit)
        {
            selectedUnit = null;
            unitDataPanel.gameObject.SetActive(false);
        }
    }
}
