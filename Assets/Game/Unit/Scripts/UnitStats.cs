using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitStats
{
    public string UnitName;
    public Sprite Portrait;

    public int MaxHealth;
    public int HealthRegen;
    public int Health;

    public int MaxEnergy;
    public int EnergyRegen;
    public int Energy;

    public int MaxTime;
    public int Time;

    public int Power;
    public int Defence;

    public AspectDedication[] AspectDedications = new AspectDedication[4];
    public List<AbilityHolder.AbilityType> InnerAbilities = new List<AbilityHolder.AbilityType>();
    
    public int TeamId;
    public int MasterId;
    
    // public List<Effect> Effects = new List<Effect>();    
}