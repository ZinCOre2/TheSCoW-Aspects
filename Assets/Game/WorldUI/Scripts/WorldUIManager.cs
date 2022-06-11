using System;
using UnityEngine;
using UnityEngine.Serialization;


public class WorldUIManager : MonoBehaviour
{
    public Transform WorldUIParent;
    [Header("Unit Bar Pack")]
    public UnitBarPack MasterUnitBarPackPrefab;
    public UnitBarPack UnitBarPackPrefab;
    [Header("Hovering World Texts (HWTs)")]
    public HoveringWorldText HealthRestoreHWT;
    public HoveringWorldText DamageTakenHWT;
    public HoveringWorldText EnergyRestoredHWT;
    public HoveringWorldText EnergyBurnedHWT;
    public HoveringWorldText TimeBurnedHWT;
    public HoveringWorldText NotEnoughEnergyHWT;
    public HoveringWorldText NotEnoughTimeHWT;

    public UnitBarPack CreateBarPack(Unit boundUnit)
    {
        UnitBarPack barPack;
        
        barPack = Instantiate(boundUnit is MasterUnit ? MasterUnitBarPackPrefab : UnitBarPackPrefab, WorldUIParent);

        barPack.BindUnit(boundUnit);

        return barPack;
    }

    public HoveringWorldText CreateHoveringWorldText(HWTType type, Vector3 position, string info)
    {
        HoveringWorldText hwt;
        switch (type)
        {
            case HWTType.HealthRestored:
                hwt = Instantiate(HealthRestoreHWT, WorldUIParent);
                break;
            case HWTType.DamageTaken:
                hwt = Instantiate(DamageTakenHWT, WorldUIParent);
                break;
            case HWTType.EnergyRestored:
                hwt = Instantiate(EnergyRestoredHWT, WorldUIParent);
                break;
            case HWTType.EnergyBurned:
                hwt = Instantiate(EnergyBurnedHWT, WorldUIParent);
                break;
            case HWTType.TimeBurned:
                hwt = Instantiate(TimeBurnedHWT, WorldUIParent);
                break;
            case HWTType.NotEnoughEnergy:
                hwt = Instantiate(NotEnoughEnergyHWT, WorldUIParent);
                break;
            case HWTType.NotEnoughTime:
                hwt = Instantiate(NotEnoughTimeHWT, WorldUIParent);
                break;
            default:
                hwt = Instantiate(NotEnoughTimeHWT, WorldUIParent);
                break;
        }
        
        hwt.StartHovering(position + Vector3.up * 5f, info);
        
        return hwt;
    }
}
