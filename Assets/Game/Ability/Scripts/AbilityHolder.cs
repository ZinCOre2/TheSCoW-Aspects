using System.Collections.Generic;
using UnityEngine;

public class AbilityHolder : MonoBehaviour
{
    public enum AType
    {
        None, Move, Strike, Healing, Energize,
        Fireball, Blast, EnergyDrain, StunningSlam,
        HealingCircle, EnergyBurst, FireSpirits, EMP,
        LeechLife, MindFlow
    };

    [SerializeField] private List<Ability> abilityList;
    public List<Ability> AbilityList { get { return abilityList; } private set { abilityList = value; } }

    public Ability GetAbility(int id) { return abilityList[id]; }
    public Ability GetAbility(AType id) { return abilityList[(int)id]; }
}
