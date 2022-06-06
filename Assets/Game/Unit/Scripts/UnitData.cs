using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Unit", menuName = "Unit")]
public class UnitData : ScriptableObject
{
    public Unit Prefab;
    
    public string unitName;
    public Sprite portrait;
    public int maxHealth, maxEnergy, maxTime, hpRegen, epRegen;
    public AspectDedication[] AspectDedications = new AspectDedication[4];
    public int power, defence;

    public AbilityHolder.AbilityType[] innerAbilities = new AbilityHolder.AbilityType[3];
}
