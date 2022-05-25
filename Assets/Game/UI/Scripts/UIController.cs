using System;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class UIController : MonoBehaviour
{
    public UnitDataPanel UnitDataPanel;
    public TurnQueuePanel TurnQueuePanel;
    
    [SerializeField] private TextMeshProUGUI gameTimer;
    
    public Unit selectedUnit { get; private set; }
    public int selectedAbilityId { get; private set; } = 0;
    public void SetId(int id) { selectedAbilityId = id; }

    private float _gameTime = 0f;
    
    private void Start()
    {
        GameController.Instance.SceneController.OnUnitSelect += ShowUnitUI;
        GameController.Instance.SceneController.OnTurnEnd += () =>
        {
            UnitDataPanel.gameObject.SetActive(false);
            if (selectedUnit)
            {
                selectedUnit.OnHealthChanged -= UnitHealthChanged;
                selectedUnit.OnEnergyChanged -= UnitEnergyChanged;
                selectedUnit.OnTimeChanged -= UnitTimeChanged;
                selectedUnit.OnUnitDeath -= UnitDeath;
                selectedUnit = null;
            }
        };
        foreach (UIAbility uiAbility in UnitDataPanel.AbilitySlots)
        {
            uiAbility.OnAbilitySelect += (ability, id) =>
            {
                if (uiAbility.TryGetComponent(out Image selectedImage))
                {
                    if (uiAbility.Id == id)
                    {
                        uiAbility.ScaledCardHolder.localScale *= uiAbility.ScaleOnSelected;
                        selectedImage.color = Color.green;

                        if (UnitDataPanel.AbilitySlots[selectedAbilityId].TryGetComponent(out Image prevImage))
                        {
                            UnitDataPanel.AbilitySlots[selectedAbilityId].ScaledCardHolder.localScale /= UnitDataPanel.AbilitySlots[selectedAbilityId].ScaleOnSelected;
                            prevImage.color = Color.white;
                        }

                        selectedAbilityId = id;
                    }
                }
            };
        }
        UnitDataPanel.gameObject.SetActive(false);
    }
    public void SetSelectedId(int id)
    {
        if (UnitDataPanel.AbilitySlots[id].TryGetComponent(out Image selectedImage))
        {
            UnitDataPanel.AbilitySlots[id].ScaledCardHolder.localScale *= UnitDataPanel.AbilitySlots[id].ScaleOnSelected;
            selectedImage.color = Color.green;

            if (UnitDataPanel.AbilitySlots[selectedAbilityId].TryGetComponent(out Image prevImage))
            {
                UnitDataPanel.AbilitySlots[selectedAbilityId].ScaledCardHolder.localScale /= UnitDataPanel.AbilitySlots[selectedAbilityId].ScaleOnSelected;
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
            if (unit is MasterUnit)
            {
                for (int i = 0; i < 6; i++)
                {
                    if (((MasterUnit)unit).DeckManager.Hand[i] != AbilityHolder.AbilityType.None)
                    {
                        UnitDataPanel.CardBacks[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        UnitDataPanel.CardBacks[i].gameObject.SetActive(false);
                    }
                }
            }
        }
        else
        {
            if (unit is MasterUnit)
            {
                for (int i = 0; i < 6; i++)
                {
                    UnitDataPanel.CardBacks[i].gameObject.SetActive(false);
                }
            }
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

        UnitDataPanel.gameObject.SetActive(true);

        UnitDataPanel.UnitNameText.text = unit.UnitData.unitName;
        UnitDataPanel.Portrait.sprite = unit.UnitData.portrait;

        UnitDataPanel.HPBar.fillAmount = unit.health / (float)unit.UnitData.maxHealth;
        UnitDataPanel.HPStateText.text = unit.health.ToString() + "\n/\n" + unit.UnitData.maxHealth.ToString();
        UnitDataPanel.HPRegenText.text = "+" + unit.UnitData.hpRegen.ToString();

        UnitDataPanel.EPBar.fillAmount = unit.energy / (float)unit.UnitData.maxEnergy;
        UnitDataPanel.EPStateText.text = unit.energy.ToString() + "\n/\n" + unit.UnitData.maxEnergy.ToString();
        UnitDataPanel.EPRegenText.text = "+" + unit.UnitData.epRegen.ToString();
        
        UnitDataPanel.TPBar.fillAmount = unit.time / (float)unit.UnitData.maxTime;
        UnitDataPanel.TPStateText.text = unit.time.ToString() + "\n/\n" + unit.UnitData.maxTime.ToString();

        for (var i = 0; i < 4; i++)
        {
            UnitDataPanel.AspectDedicationsText[i].gameObject.SetActive(selectedUnit.UnitData.AspectDedications[i].IsUsable);
            UnitDataPanel.AspectCovers[i].gameObject.SetActive(!selectedUnit.UnitData.AspectDedications[i].IsUsable);
            
            UnitDataPanel.AspectDedicationsText[i].text = selectedUnit.UnitData.AspectDedications[i].Value.ToString();
        }
        
        UnitDataPanel.PowerText.text = unit.UnitData.power.ToString();
        UnitDataPanel.DefenceText.text = unit.UnitData.defence.ToString();
        
        if (UnitDataPanel.AbilitySlots[selectedAbilityId].TryGetComponent(out Image prevImage))
        {
            UnitDataPanel.AbilitySlots[selectedAbilityId].transform.localScale = Vector3.one;
            prevImage.color = Color.white;
        }
        
        UnitDataPanel.AbilitySlots[0].SetAbility(GameController.Instance.AbilityHolder.GetAbility(AbilityHolder.AbilityType.Move));
        GameController.Instance.SceneController.SetSelectedAbility(UnitDataPanel.AbilitySlots[0].ability);
        UnitDataPanel.AbilitySlots[0].SetId(0);
        if (UnitDataPanel.AbilitySlots[0].TryGetComponent(out Image image))
        {
            UnitDataPanel.AbilitySlots[0].ScaledCardHolder.localScale = Vector3.one * UnitDataPanel.AbilitySlots[0].ScaleOnSelected;
            image.color = Color.green;
        }
        selectedAbilityId = 0;

        if (selectedUnit is MasterUnit)
        {
            for (int i = 1; i < 7; i++)
            {
                UnitDataPanel.AbilitySlots[i].gameObject.SetActive(true);
                UnitDataPanel.AbilitySlots[i].SetId(i);
                UnitDataPanel.AbilitySlots[i]
                    .SetAbility(GameController.Instance.AbilityHolder.GetAbility(((MasterUnit)selectedUnit).DeckManager.Hand[i - 1]));
            }
        }
    }

    private void UnitHealthChanged(Unit unit, int newHealth)
    {
        if (selectedUnit == unit)
        {
            UnitDataPanel.HPBar.fillAmount = newHealth / (float)unit.UnitData.maxHealth;
            UnitDataPanel.HPStateText.text = newHealth.ToString() + "\n/\n" + unit.UnitData.maxHealth.ToString();
        }
    }
    private void UnitEnergyChanged(Unit unit, int newEnergy)
    {
        if (selectedUnit == unit)
        {
            UnitDataPanel.EPBar.fillAmount = newEnergy / (float)unit.UnitData.maxEnergy;
            UnitDataPanel.EPStateText.text = newEnergy.ToString() + "\n/\n" + unit.UnitData.maxEnergy.ToString();
        }
    }
    private void UnitTimeChanged(Unit unit, int newTime)
    {
        if (selectedUnit == unit)
        {
            UnitDataPanel.TPBar.fillAmount = newTime / (float)unit.UnitData.maxTime;
            UnitDataPanel.TPStateText.text = newTime.ToString() + "\n/\n" + unit.UnitData.maxTime.ToString();
        }
    }
    private void UnitDeath(Unit unit)
    {
        if (selectedUnit == unit)
        {
            selectedUnit = null;
            UnitDataPanel.gameObject.SetActive(false);
        }
    }
}
