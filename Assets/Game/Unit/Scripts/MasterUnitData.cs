using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MasterUnit", menuName = "GameData/MasterUnit")]
public class MasterUnitData : UnitData
{
    public List<AbilityHolder.AbilityType> StartingDeck = new List<AbilityHolder.AbilityType>();
}