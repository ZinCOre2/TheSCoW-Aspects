using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "Ability")]
public class AbilityData : ScriptableObject
{
    public enum AreaType { Pathfinding, Impulse, Absolute, Line }

    [Header("General Data")]
    public string cardName;
    public Sprite icon;
    [TextArea(3, 6)]
    public string description;
    [Header("Area Data")]
    //public bool targetSelf, targetAlly, targetEnemy, targetGround;
    public AreaType rangeType; 
    public AreaType areaType;
    public int minRange, maxRange, minAreaRange, maxAreaRange;
    [Header("Ability Data")]
    public int epCost;
    public int tpCost;
    public AspectDedication[] Dedications = new AspectDedication[4];
    public int[] values;
}
