using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Unit", menuName = "GameData/Unit")]
public class UnitData : ScriptableObject
{
    public Unit Prefab;
    
    public string UnitName;
    public Sprite Portrait;
    
    public int MaxHealth;
    public int MaxEnergy;
    public int MaxTime;
    public int HPRegen;
    public int EPRegen;

    public int Power;
    public int Defence;
    
    public AspectDedication[] AspectDedications = new AspectDedication[4];
    public List<AbilityHolder.AbilityType> InnerAbilities = new List<AbilityHolder.AbilityType>();
}
