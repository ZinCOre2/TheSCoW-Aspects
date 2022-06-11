using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitBarPack : MonoBehaviour
{
    [SerializeField] private Transform barPackHolder;
    [SerializeField] private float shrinkValue = 1.5f;
    
    [SerializeField] private Vector3 offset = new Vector3(0, 10f, 0);
    [SerializeField] private TextMeshProUGUI cardCountText;
    [SerializeField] private Image hpBar;
    [SerializeField] private Image epBar;
    [SerializeField] private Image tpBar;
    [SerializeField] private Image tpHolder;
    
    private Unit _boundUnit;

    public void BindUnit(Unit unit)
    {
        _boundUnit = unit;
        transform.position = _boundUnit.transform.position + offset;
        
        SubscribeEvents();
    }
    private void Update()
    {
        if (_boundUnit != null)
        {
            transform.position = _boundUnit.transform.position + offset;
        }

        SceneController_OnAbilityUsed();
    }
    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void SubscribeEvents()
    {
        _boundUnit.OnHealthChanged += BoundUnit_OnHealthChanged;
        _boundUnit.OnEnergyChanged += BoundUnit_OnEnergyChanged;
        _boundUnit.OnTimeChanged += BoundUnit_OnTimeChanged;
        _boundUnit.OnUnitDeath += BoundUnit_OnUnitDeath;
        GameController.Instance.SceneController.OnTurnEnd += SceneController_OnTurnEnd;
        GameController.Instance.SceneController.OnAbilityUsed += SceneController_OnAbilityUsed;
        
        BoundUnit_OnHealthChanged(_boundUnit, _boundUnit.UnitStats.Health);
        BoundUnit_OnEnergyChanged(_boundUnit, _boundUnit.UnitStats.Energy);
        BoundUnit_OnTimeChanged(_boundUnit, _boundUnit.UnitStats.Time);
        SceneController_OnTurnEnd();
    }

    private void SceneController_OnAbilityUsed()
    {
        if (!(_boundUnit is MasterUnit)) { return; } 
        
        var cardCount = 0;
        foreach (var t in ((MasterUnit)_boundUnit).DeckManager.Hand)
        {
            if (t != AbilityHolder.AbilityType.None)
            {
                cardCount++;
            }
        }
        cardCountText.text = $"x{cardCount}";
    }

    private void UnsubscribeEvents()
    {
        _boundUnit.OnHealthChanged -= BoundUnit_OnHealthChanged;
        _boundUnit.OnEnergyChanged -= BoundUnit_OnEnergyChanged;
        _boundUnit.OnTimeChanged -= BoundUnit_OnTimeChanged;
        _boundUnit.OnUnitDeath -= BoundUnit_OnUnitDeath;
        GameController.Instance.SceneController.OnTurnEnd -= SceneController_OnTurnEnd;
        GameController.Instance.SceneController.OnAbilityUsed -= SceneController_OnAbilityUsed;
    }

    private void BoundUnit_OnHealthChanged(Unit unit, int value)
    {
        hpBar.fillAmount = value / (float)_boundUnit.UnitStats.MaxHealth;
    }
    private void BoundUnit_OnEnergyChanged(Unit unit, int value)
    {
        epBar.fillAmount = value / (float)_boundUnit.UnitStats.MaxEnergy;
    }
    private void BoundUnit_OnTimeChanged(Unit unit, int value)
    {
        tpBar.fillAmount = value / (float)_boundUnit.UnitStats.MaxTime;
    }
    private void BoundUnit_OnUnitDeath(Unit unit)
    {
        UnsubscribeEvents();
        Destroy(gameObject);
    }
    private void SceneController_OnTurnEnd()
    {
        if (_boundUnit.UnitStats.TeamId - 1 != GameController.Instance.SceneController.turnId)
        {
            var holderTransform = barPackHolder.transform;
            var localScale = holderTransform.localScale;
            
            holderTransform.localScale = new Vector3(1, 1 / shrinkValue, 1);
            tpHolder.gameObject.SetActive(false);
        }
        else
        {
            var holderTransform = barPackHolder.transform;
            var localScale = holderTransform.localScale;
            
            holderTransform.localScale = Vector3.one;
            
            tpHolder.gameObject.SetActive(true);
            BoundUnit_OnTimeChanged(_boundUnit, _boundUnit.UnitStats.Time);
        }
    }
}
