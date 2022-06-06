using System;
using System.Collections.Generic;
using UnityEngine;

public class AbilityHolder : MonoBehaviour
{
    public enum AbilityType
    {
        None, Move, SweepStrike, Healing, Energize,
        Fireball, Blast, EnergyDrain, StunningSlam,
        HealingCircle, EnergyBurst, FireSpirits, Emp,
        LeechLife, MindFlow, //BoulderSlam, BoulderCage,
        Educate, CelestialSpear, PowerForge, FocusedEnergy,
        InnerPower, LifeGrowth, GaleRush, Aeroblade,
        DemonFire, DemonPact, DemonSeal
    };

    [SerializeField] private List<Ability> abilityList;
    public List<Ability> AbilityList { get { return abilityList; } private set { abilityList = value; } }

    public Ability GetAbility(int id) { return abilityList[id]; }
    public Ability GetAbility(AbilityType id) { return abilityList[(int)id]; }

    private void Update()
    {
        transform.position = GameController.Instance.CameraController.Camera.transform.position;
    }
}
