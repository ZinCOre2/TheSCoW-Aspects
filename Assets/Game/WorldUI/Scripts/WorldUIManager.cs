using System;
using UnityEngine;


public class WorldUIManager : MonoBehaviour
{
    public Transform WorldUIParent;
    
    public UnitBarPack UnitBarPackPrefab;
    public HoveringWorldText HealthRestoreHWT, DamageTakenHWT, EnergyRestoredHWT, EnergyBurnedHWT, TimeBurnedHWT, NotEnoughEnergyHWT, NotEnoughTimeHWT;

    public UnitBarPack CreateBarPack(Unit boundUnit)
    {
        var barPack = Instantiate(UnitBarPackPrefab, WorldUIParent);
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
