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
    public int[] DedicationsEnergy = new int[4];
    public AbilityHolder.AbilityType[] InnerAbilities = new AbilityHolder.AbilityType[3];
    
    public int TeamId;
    public int MasterId;
    
    // public List<Effect> Effects = new List<Effect>();    
}