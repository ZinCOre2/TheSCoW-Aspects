using System.Collections.Generic;
using UnityEngine;

public class UnitHolder : MonoBehaviour
{
    public enum UnitType
    {
        None, 
        Knight, Priest, Wizard,
        Boulder
    };
    
    [SerializeField] private List<UnitData> unitList;
    public List<UnitData> UnitList { get { return unitList; } private set { unitList = value; } }

    public UnitData GetUnitData(int id) { return unitList[id]; }
    public UnitData GetUnitData(UnitType id) { return unitList[(int)id]; }
}
