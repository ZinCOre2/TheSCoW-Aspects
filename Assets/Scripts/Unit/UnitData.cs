using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Unit", menuName = "Unit")]
public class UnitData : ScriptableObject
{
    public string unitName;
    public Sprite portrait;
    public int maxHealth, maxEnergy, hpRegen, epRegen;
    public int moveCost, power, defence;
}
