using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MasterUnit", menuName = "MasterUnit")]
public class MasterUnitData : UnitData
{
    public MasterUnit MasterUnitPrefab;
    
    public List<AbilityHolder.AbilityType> StartingDeck;
}