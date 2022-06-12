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
    public PausePanel PausePanel;
    
    [SerializeField] private TextMeshProUGUI gameTimer;
    
    public Unit SelectedUnit { get; private set; }
    [HideInInspector] public bool IsCard;
    public int selectedAbilityId { get; private set; } = 0;

    private float _gameTime = 0f;
    
    private void Start()
    {
        SubscribeEvents();
        UnitDataPanel.gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

    private void Update()
    {
        _gameTime += Time.deltaTime;
        gameTimer.text = (int)(_gameTime / 60) + ":" + (int)(_gameTime % 60);
    }
    
    private void SubscribeEvents()
    {
        GameController.Instance.SceneController.OnUnitSelect += ShowUnitUI;
        GameController.Instance.SceneController.OnAbilityUsed += UpdateUnitUI;
        GameController.Instance.SceneController.OnTurnEnd += EndTurn;
        
        foreach (var uiAbility in UnitDataPanel.AbilitySlots)
        {
            uiAbility.OnAbilitySelect += OnUIAbilitySelected;
        }
        foreach (var uiCard in UnitDataPanel.CardSlots)
        {
            uiCard.OnAbilitySelect += OnUICardSelected;
        }
    }
    private void UnsubscribeEvents()
    {
        GameController.Instance.SceneController.OnUnitSelect -= ShowUnitUI;
        GameController.Instance.SceneController.OnAbilityUsed -= UpdateUnitUI;
        GameController.Instance.SceneController.OnTurnEnd -= EndTurn;
        
        foreach (var uiAbility in UnitDataPanel.AbilitySlots)
        {
            uiAbility.OnAbilitySelect -= OnUIAbilitySelected;
        }
        foreach (var uiCard in UnitDataPanel.CardSlots)
        {
            uiCard.OnAbilitySelect -= OnUICardSelected;
        }
    }

    private void OnUIAbilitySelected(Ability ability, int id)
    {
        if (UnitDataPanel.AbilitySlots[id].TryGetComponent(out Image selectedImage))
        {
            UnitDataPanel.AbilitySlots[id].ScaledCardHolder.localScale *= UnitDataPanel.AbilitySlots[id].ScaleOnSelected;
            selectedImage.color = Color.green;

            if (!IsCard && UnitDataPanel.AbilitySlots[selectedAbilityId].TryGetComponent(out Image prevImage))
            {
                UnitDataPanel.AbilitySlots[selectedAbilityId].ScaledCardHolder.localScale /= UnitDataPanel.AbilitySlots[selectedAbilityId].ScaleOnSelected;
                prevImage.color = Color.white;
            }
                    
            if (IsCard && UnitDataPanel.CardSlots[selectedAbilityId].TryGetComponent(out prevImage))
            {
                UnitDataPanel.CardSlots[selectedAbilityId].ScaledCardHolder.localScale /= UnitDataPanel.CardSlots[selectedAbilityId].ScaleOnSelected;
                prevImage.color = Color.white;
            }

            selectedAbilityId = id;
            IsCard = false;
        }
    }
    private void OnUICardSelected(Ability ability, int id)
    {
        if (UnitDataPanel.CardSlots[id].TryGetComponent(out Image selectedImage))
        {
            UnitDataPanel.CardSlots[id].ScaledCardHolder.localScale *= UnitDataPanel.CardSlots[id].ScaleOnSelected;
            selectedImage.color = Color.green;

            if (!IsCard && UnitDataPanel.AbilitySlots[selectedAbilityId].TryGetComponent(out Image prevImage))
            {
                UnitDataPanel.AbilitySlots[selectedAbilityId].ScaledCardHolder.localScale /= UnitDataPanel.AbilitySlots[selectedAbilityId].ScaleOnSelected;
                prevImage.color = Color.white;
            }
                    
            if (IsCard && UnitDataPanel.CardSlots[selectedAbilityId].TryGetComponent(out prevImage))
            {
                UnitDataPanel.CardSlots[selectedAbilityId].ScaledCardHolder.localScale /= UnitDataPanel.CardSlots[selectedAbilityId].ScaleOnSelected;
                prevImage.color = Color.white;
            }

            selectedAbilityId = id;
            IsCard = true;
        }
    }
    
    public void SetId(int id, bool isCard)
    {
        if (!IsCard && UnitDataPanel.AbilitySlots[selectedAbilityId].TryGetComponent(out Image prevImage))
        {
            UnitDataPanel.AbilitySlots[selectedAbilityId].ScaledCardHolder.localScale /= UnitDataPanel.AbilitySlots[selectedAbilityId].ScaleOnSelected;
            prevImage.color = Color.white;
        }
        
        if (IsCard && UnitDataPanel.CardSlots[selectedAbilityId].TryGetComponent(out prevImage))
        {
            UnitDataPanel.CardSlots[selectedAbilityId].ScaledCardHolder.localScale /= UnitDataPanel.CardSlots[selectedAbilityId].ScaleOnSelected;
            prevImage.color = Color.white;
        }

        selectedAbilityId = id;
        IsCard = isCard;
    }
    public void SetSelectedId(int id, bool isCard)
    {
        if (!isCard && UnitDataPanel.AbilitySlots[id].TryGetComponent(out Image selectedImage))
        {
            UnitDataPanel.AbilitySlots[id].ScaledCardHolder.localScale *= UnitDataPanel.AbilitySlots[id].ScaleOnSelected;
            selectedImage.color = Color.green;

            if (UnitDataPanel.AbilitySlots[selectedAbilityId].TryGetComponent(out Image prevImage))
            {
                UnitDataPanel.AbilitySlots[selectedAbilityId].ScaledCardHolder.localScale /= UnitDataPanel.AbilitySlots[selectedAbilityId].ScaleOnSelected;
                prevImage.color = Color.white;
            }

            selectedAbilityId = id;
            IsCard = false;
        }
        
        if (isCard && UnitDataPanel.CardSlots[id].TryGetComponent(out selectedImage))
        {
            UnitDataPanel.CardSlots[id].ScaledCardHolder.localScale *= UnitDataPanel.CardSlots[id].ScaleOnSelected;
            selectedImage.color = Color.green;

            if (UnitDataPanel.CardSlots[selectedAbilityId].TryGetComponent(out Image prevImage))
            {
                UnitDataPanel.CardSlots[selectedAbilityId].ScaledCardHolder.localScale /= UnitDataPanel.CardSlots[selectedAbilityId].ScaleOnSelected;
                prevImage.color = Color.white;
            }

            selectedAbilityId = id;
            IsCard = true;
        }
    }
 
    private void EndTurn()
    {
        UnitDataPanel.gameObject.SetActive(false);
        if (SelectedUnit)
        {
            SelectedUnit.OnHealthChanged -= UnitHealthChanged;
            SelectedUnit.OnEnergyChanged -= UnitEnergyChanged;
            SelectedUnit.OnTimeChanged -= UnitTimeChanged;
            SelectedUnit.OnUnitDeath -= UnitDeath;
            SelectedUnit = null;
        }
    }
    
    public void ShowUnitUI(Unit unit)
    {
        for (int i = 1; i < 4; i++)
        {
            if (unit.UnitStats.InnerAbilities[i - 1] != AbilityHolder.AbilityType.None)
            {
                UnitDataPanel.AbilitySlots[i].gameObject.SetActive(true);
            }
            else
            {
                UnitDataPanel.AbilitySlots[i].gameObject.SetActive(false);
            }
        }

        if (!(unit is MasterUnit))
        {
            UnitDataPanel.CardPanel.SetActive(false);
            
            for (int i = 0; i < 6; i++)
            {
                UnitDataPanel.CardBacks[i].gameObject.SetActive(false);
            }
        }
        else 
        {
            UnitDataPanel.CardPanel.SetActive(true);
            
            if (unit.TeamId - 1 != GameController.Instance.SceneController.turnId)
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
            else
            {
                for (int i = 0; i < 6; i++)
                {
                    UnitDataPanel.CardBacks[i].gameObject.SetActive(false);
                }
            }
        }

        if (SelectedUnit)
        {
            SelectedUnit.OnHealthChanged -= UnitHealthChanged;
            SelectedUnit.OnEnergyChanged -= UnitEnergyChanged;
            SelectedUnit.OnTimeChanged -= UnitTimeChanged;
            SelectedUnit.OnUnitDeath -= UnitDeath;
        }
        SelectedUnit = unit;
        SelectedUnit.OnHealthChanged += UnitHealthChanged;
        SelectedUnit.OnEnergyChanged += UnitEnergyChanged;
        SelectedUnit.OnTimeChanged += UnitTimeChanged;
        SelectedUnit.OnUnitDeath += UnitDeath;
        
        UnitDataPanel.gameObject.SetActive(true);
        
        UpdateUnitUI();
    }

    private void UpdateUnitUI()
    {
        UnitDataPanel.UnitNameText.text = SelectedUnit.UnitStats.UnitName;
        UnitDataPanel.Portrait.sprite = SelectedUnit.UnitStats.Portrait;

        UnitDataPanel.HPBar.fillAmount = SelectedUnit.UnitStats.Health / (float)SelectedUnit.UnitStats.MaxHealth;
        UnitDataPanel.HPStateText.text = SelectedUnit.UnitStats.Health.ToString() + "\n/\n" + SelectedUnit.UnitStats.MaxHealth.ToString();
        UnitDataPanel.HPRegenText.text = "+" + SelectedUnit.UnitStats.HealthRegen.ToString();

        UnitDataPanel.EPBar.fillAmount = SelectedUnit.UnitStats.Energy / (float)SelectedUnit.UnitStats.MaxEnergy;
        UnitDataPanel.EPStateText.text = SelectedUnit.UnitStats.Energy.ToString() + "\n/\n" + SelectedUnit.UnitStats.MaxEnergy.ToString();
        UnitDataPanel.EPRegenText.text = "+" + SelectedUnit.UnitStats.EnergyRegen.ToString();
        
        UnitDataPanel.TPBar.fillAmount = SelectedUnit.UnitStats.Time / (float)SelectedUnit.UnitStats.MaxTime;
        UnitDataPanel.TPStateText.text = SelectedUnit.UnitStats.Time.ToString() + "\n/\n" + SelectedUnit.UnitStats.MaxTime.ToString();

        for (var i = 0; i < 4; i++)
        {
            UnitDataPanel.AspectDedicationsText[i].gameObject.SetActive(SelectedUnit.UnitStats.AspectDedications[i].IsUsable);
            UnitDataPanel.AspectCovers[i].gameObject.SetActive(!SelectedUnit.UnitStats.AspectDedications[i].IsUsable);
            
            UnitDataPanel.AspectDedicationsText[i].text = SelectedUnit.UnitStats.AspectDedications[i].Value.ToString();
        }
        
        UnitDataPanel.PowerText.text = SelectedUnit.UnitStats.Power.ToString();
        UnitDataPanel.DefenceText.text = SelectedUnit.UnitStats.Defence.ToString();
        
        if (!IsCard && UnitDataPanel.AbilitySlots[selectedAbilityId].TryGetComponent(out Image prevImage))
        {
            UnitDataPanel.AbilitySlots[selectedAbilityId].transform.localScale = Vector3.one;
            prevImage.color = Color.white;
        }
        else if (IsCard && UnitDataPanel.CardSlots[selectedAbilityId].TryGetComponent(out prevImage))
        {
            UnitDataPanel.CardSlots[selectedAbilityId].transform.localScale = Vector3.one;
            prevImage.color = Color.white;
        }
        
        UnitDataPanel.AbilitySlots[0].SetAbility(GameController.Instance.AbilityHolder.GetAbility(AbilityHolder.AbilityType.Move));
        GameController.Instance.SceneController.SetSelectedAbility(UnitDataPanel.AbilitySlots[0].ability);
        UnitDataPanel.AbilitySlots[0].SetId(0);
        if (UnitDataPanel.AbilitySlots[0].TryGetComponent(out Image image))
        {
            image.color = Color.green;
        }
        selectedAbilityId = 0;
        
        for (int i = 1; i < 4; i++)
        {
            if (SelectedUnit.UnitStats.InnerAbilities[i - 1] != AbilityHolder.AbilityType.None)
            {
                UnitDataPanel.AbilitySlots[i].gameObject.SetActive(true);
                UnitDataPanel.AbilitySlots[i].SetId(i);
                UnitDataPanel.AbilitySlots[i]
                    .SetAbility(GameController.Instance.AbilityHolder.GetAbility(SelectedUnit.UnitStats.InnerAbilities[i - 1]));
            }
        }

        if (!(SelectedUnit is MasterUnit masterUnit)) { return; }

        for (int i = 0; i < 6; i++)
        {
            UnitDataPanel.CardSlots[i].gameObject.SetActive(true);
            UnitDataPanel.CardSlots[i].SetId(i);
            UnitDataPanel.CardSlots[i].SetAbility(GameController.Instance.AbilityHolder.GetAbility(masterUnit.DeckManager.Hand[i]));
        }

        UnitDataPanel.DrawPileSizeText.text = $"{masterUnit.DeckManager.DrawPile.Count}";
        UnitDataPanel.DiscardPileSizeText.text = $"{masterUnit.DeckManager.DiscardPile.Count}";
    }

    private void UnitHealthChanged(Unit unit, int newHealth)
    {
        if (SelectedUnit == unit)
        {
            UnitDataPanel.HPBar.fillAmount = newHealth / (float)unit.UnitData.maxHealth;
            UnitDataPanel.HPStateText.text = newHealth.ToString() + "\n/\n" + unit.UnitData.maxHealth.ToString();
        }
    }
    private void UnitEnergyChanged(Unit unit, int newEnergy)
    {
        if (SelectedUnit == unit)
        {
            UnitDataPanel.EPBar.fillAmount = newEnergy / (float)unit.UnitData.maxEnergy;
            UnitDataPanel.EPStateText.text = newEnergy.ToString() + "\n/\n" + unit.UnitData.maxEnergy.ToString();
        }
    }
    private void UnitTimeChanged(Unit unit, int newTime)
    {
        if (SelectedUnit == unit)
        {
            UnitDataPanel.TPBar.fillAmount = newTime / (float)unit.UnitData.maxTime;
            UnitDataPanel.TPStateText.text = newTime.ToString() + "\n/\n" + unit.UnitData.maxTime.ToString();
        }
    }
    private void UnitDeath(Unit unit)
    {
        if (SelectedUnit == unit)
        {
            SelectedUnit = null;
            UnitDataPanel.gameObject.SetActive(false);
        }
    }
}
