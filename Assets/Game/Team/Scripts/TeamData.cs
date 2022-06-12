using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Team", menuName = "Team")]
public class TeamData : ScriptableObject
{
    public string TeamName;
    
    public UnitHolder.UnitType[] TeamMembers = new UnitHolder.UnitType[3];
    public MasterUnitData[] MembersData = new MasterUnitData[3];
}
