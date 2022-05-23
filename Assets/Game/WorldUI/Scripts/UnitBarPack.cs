using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitBarPack : MonoBehaviour
{
    [SerializeField] private Vector3 offset = new Vector3(0, 10f, 0);
    [SerializeField] private TextMeshProUGUI cardCountText;
    [SerializeField] private Image hpBar;
    [SerializeField] private Image epBar;
    [SerializeField] private Image tpBar;
    
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

        if (!(_boundUnit is MasterUnit masterUnit)) { return; } 
        
        var cardCount = 0;
        for (var i = 0; i < masterUnit.DeckManager.Hand.Length; i++)
        {
            if (masterUnit.DeckManager.Hand[i] != AbilityHolder.AbilityType.None)
            {
                cardCount++;
            }
        }
        cardCountText.text = $"x{cardCount}";
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
    }
    private void UnsubscribeEvents()
    {
        _boundUnit.OnHealthChanged -= BoundUnit_OnHealthChanged;
        _boundUnit.OnEnergyChanged -= BoundUnit_OnEnergyChanged;
        _boundUnit.OnTimeChanged -= BoundUnit_OnTimeChanged;
        _boundUnit.OnUnitDeath -= BoundUnit_OnUnitDeath;
    }

    private void BoundUnit_OnHealthChanged(Unit unit, int value)
    {
        hpBar.fillAmount = value / (float)_boundUnit.UnitData.maxHealth;
    }
    private void BoundUnit_OnEnergyChanged(Unit unit, int value)
    {
        epBar.fillAmount = value / (float)_boundUnit.UnitData.maxEnergy;
    }
    private void BoundUnit_OnTimeChanged(Unit unit, int value)
    {
        tpBar.fillAmount = value / (float)_boundUnit.UnitData.maxTime;
    }
    private void BoundUnit_OnUnitDeath(Unit unit)
    {
        UnsubscribeEvents();
        Destroy(gameObject);
    }
}
