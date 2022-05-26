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
        foreach (var uiAbility in UnitDataPanel.AbilitySlots)
        {
            uiAbility.OnAbilitySelect += (ability, id) =>
            {
                if (!uiAbility.TryGetComponent(out Image selectedImage)) { return; }
                if (uiAbility.Id != id) { return; }
                
                uiAbility.ScaledCardHolder.localScale *= uiAbility.ScaleOnSelected;
                selectedImage.color = Color.green;

                if (UnitDataPanel.AbilitySlots[selectedAbilityId].TryGetComponent(out Image prevImage))
                {
                    UnitDataPanel.AbilitySlots[selectedAbilityId].ScaledCardHolder.localScale /= UnitDataPanel.AbilitySlots[selectedAbilityId].ScaleOnSelected;
                    prevImage.color = Color.white;
                }

                selectedAbilityId = id;
            };
        }
        foreach (var uiAbility in UnitDataPanel.CardSlots)
        {
            uiAbility.OnAbilitySelect += (ability, id) =>
            {
                if (!uiAbility.TryGetComponent(out Image selectedImage)) { return; }
                if (uiAbility.Id != id) { return; }
                
                uiAbility.ScaledCardHolder.localScale *= uiAbility.ScaleOnSelected;
                selectedImage.color = Color.green;

                if (UnitDataPanel.CardSlots[selectedAbilityId].TryGetComponent(out Image prevImage))
                {
                    UnitDataPanel.CardSlots[selectedAbilityId].ScaledCardHolder.localScale /= UnitDataPanel.CardSlots[selectedAbilityId].ScaleOnSelected;
                    prevImage.color = Color.white;
                }

                selectedAbilityId = id;
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
        
        if (UnitDataPanel.CardSlots[id].TryGetComponent(out selectedImage))
        {
            UnitDataPanel.CardSlots[id].ScaledCardHolder.localScale *= UnitDataPanel.CardSlots[id].ScaleOnSelected;
            selectedImage.color = Color.green;

            if (UnitDataPanel.CardSlots[selectedAbilityId].TryGetComponent(out Image prevImage))
            {
                UnitDataPanel.CardSlots[selectedAbilityId].ScaledCardHolder.localScale /= UnitDataPanel.CardSlots[selectedAbilityId].ScaleOnSelected;
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

        UnitDataPanel.UnitNameText.text = unit.UnitData.UnitName;
        UnitDataPanel.Portrait.sprite = unit.UnitData.Portrait;

        UnitDataPanel.HPBar.fillAmount = unit.UnitStats.Health / (float)unit.UnitData.MaxHealth;
        UnitDataPanel.HPStateText.text = unit.UnitStats.Health.ToString() + "\n/\n" + unit.UnitData.MaxHealth.ToString();
        UnitDataPanel.HPRegenText.text = "+" + unit.UnitData.HPRegen.ToString();

        UnitDataPanel.EPBar.fillAmount = unit.UnitStats.Energy / (float)unit.UnitData.MaxEnergy;
        UnitDataPanel.EPStateText.text = unit.UnitStats.Energy.ToString() + "\n/\n" + unit.UnitData.MaxEnergy.ToString();
        UnitDataPanel.EPRegenText.text = "+" + unit.UnitData.EPRegen.ToString();
        
        UnitDataPanel.TPBar.fillAmount = unit.UnitStats.Time / (float)unit.UnitData.MaxTime;
        UnitDataPanel.TPStateText.text = unit.UnitStats.Time.ToString() + "\n/\n" + unit.UnitData.MaxTime.ToString();

        for (var i = 0; i < 4; i++)
        {
            UnitDataPanel.AspectDedicationsText[i].gameObject.SetActive(selectedUnit.UnitData.AspectDedications[i].IsUsable);
            UnitDataPanel.AspectCovers[i].gameObject.SetActive(!selectedUnit.UnitData.AspectDedications[i].IsUsable);
            
            UnitDataPanel.AspectDedicationsText[i].text = selectedUnit.UnitData.AspectDedications[i].Value.ToString();
        }
        
        UnitDataPanel.PowerText.text = unit.UnitData.Power.ToString();
        UnitDataPanel.DefenceText.text = unit.UnitData.Defence.ToString();
        
        if (UnitDataPanel.CardSlots[selectedAbilityId].TryGetComponent(out Image prevImage))
        {
            UnitDataPanel.CardSlots[selectedAbilityId].transform.localScale = Vector3.one;
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

        if (!(selectedUnit is MasterUnit masterUnit)) return;
        
        for (var i = 0; i < 6; i++)
        {
            UnitDataPanel.CardSlots[i].gameObject.SetActive(true);
            UnitDataPanel.CardSlots[i].SetId(i);
            UnitDataPanel.CardSlots[i]
                .SetAbility(GameController.Instance.AbilityHolder.GetAbility(masterUnit.DeckManager.Hand[i]));
        }
        
    }

    private void UnitHealthChanged(Unit unit, int newHealth)
    {
        if (selectedUnit == unit)
        {
            UnitDataPanel.HPBar.fillAmount = newHealth / (float)unit.UnitData.MaxHealth;
            UnitDataPanel.HPStateText.text = newHealth.ToString() + "\n/\n" + unit.UnitData.MaxHealth.ToString();
        }
    }
    private void UnitEnergyChanged(Unit unit, int newEnergy)
    {
        if (selectedUnit == unit)
        {
            UnitDataPanel.EPBar.fillAmount = newEnergy / (float)unit.UnitData.MaxEnergy;
            UnitDataPanel.EPStateText.text = newEnergy.ToString() + "\n/\n" + unit.UnitData.MaxEnergy.ToString();
        }
    }
    private void UnitTimeChanged(Unit unit, int newTime)
    {
        if (selectedUnit == unit)
        {
            UnitDataPanel.TPBar.fillAmount = newTime / (float)unit.UnitData.MaxTime;
            UnitDataPanel.TPStateText.text = newTime.ToString() + "\n/\n" + unit.UnitData.MaxTime.ToString();
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
