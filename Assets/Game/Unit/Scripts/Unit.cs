﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Unit : PhysicalEntity
{
    public event Action<Unit, int> OnHealthChanged;
    public event Action<Unit, int> OnEnergyChanged;
    public event Action<Unit, int> OnTimeChanged;
    public event Action<Unit> OnUnitDeath;
    public event Action OnFinishAbilityUse;

    [HideInInspector] public UnitStats UnitStats;
    
    [Header("General")]
    [SerializeField] private UnitData unitData;
    [SerializeField] private Animator animator;
    [SerializeField] private float moveSpeed = 50f;
    [SerializeField] private int teamId;
    
    // General
    public UnitData UnitData { get { return unitData; } private set { unitData = value; } }
    public Animator Animator { get { return animator; } private set { animator = value; } }
    public int TeamId { get { return teamId; } private set { teamId = value; } }

    public bool usingAbility { get; private set; } = false;

    protected override void Start()
    {
        base.Start();
        InitializeStats();
        AssignIds(teamId, GameController.Instance.EntityManager.GenerateUniqueId());
        
        GameController.Instance.SceneController.OnUnitSelect += MarkUnit;

        GameController.Instance.WorldUIManager.CreateBarPack(this);
        
        OnUnitDeath += UnitDeath;
        OnUnitDeath += GameController.Instance.SceneController.UnitDeath;
        
        switch (teamId)
        {
            case 1:
                highlight.material.color *= Color.blue;
                break;
            case 2:
                highlight.material.color *= Color.red;
                break;
        }

        if (!(this is MasterUnit)) { return; }
        
        SceneController.Counter[teamId - 1]++;
    }

    private void InitializeStats()
    {
        UnitStats.UnitName = unitData.unitName;
        UnitStats.Portrait = unitData.portrait;

        UnitStats.MaxHealth = unitData.maxHealth;
        UnitStats.HealthRegen = unitData.hpRegen;
        UnitStats.Health = unitData.maxHealth;

        UnitStats.MaxEnergy = unitData.maxEnergy;
        UnitStats.EnergyRegen = unitData.epRegen;
        UnitStats.Energy = unitData.maxEnergy;

        UnitStats.MaxTime = unitData.maxTime;
        UnitStats.Time = unitData.maxTime;

        UnitStats.Power = unitData.power;
        UnitStats.Defence = unitData.defence;

        for (var i = 0; i < UnitStats.AspectDedications.Length; i++)
        {
            UnitStats.AspectDedications[i] = unitData.AspectDedications[i];
        }

        UnitStats.InnerAbilities = unitData.innerAbilities;
    }
    private void AssignIds(int initTeamId, int initMasterId)
    {
        UnitStats.TeamId = initTeamId;
        UnitStats.MasterId = initMasterId;
    }

    private void OnDisable()
    {
        GameController.Instance.SceneController.OnUnitSelect -= MarkUnit;
        
        if (teamId == 0) { return; }
        
        OnUnitDeath -= UnitDeath;
        OnUnitDeath -= GameController.Instance.SceneController.UnitDeath;
    }

    private void OnDestroy()
    {
        GameController.Instance.EntityManager.RemoveEntity(this);
    }

    private void MarkUnit(Unit unit)
    {
        if (unit == this)
        {
            marker?.SetActive(true);
            return;
        }
        marker?.SetActive(false);
    }
    private void UnitDeath(Unit unit)
    {
        if (unit == this)
        {
            Destroy(pivot.gameObject, 3f);
            GameController.Instance.EntityManager.RemoveEntity(this);
        }
    }

    public void ChangeHealth(int value)
    {
        if (value < 0)
        {
            value += UnitData.defence;
            value = Mathf.Clamp(value, Int32.MinValue, 0);
        }
        
        UnitStats.Health += value;
        UnitStats.Health = Mathf.Clamp(UnitStats.Health, 0, UnitStats.MaxHealth);
        OnHealthChanged?.Invoke(this, UnitStats.Health);

        if (value < 0)
        {
            GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.DamageTaken, transform.position, $"{value}");

            if (UnitStats.Health <= 0)
            {
                if (animator) animator?.SetTrigger("Death");
                OnUnitDeath?.Invoke(this);
                // Dead, show animation, remove unit from scene soon, subtract from counter above
            }
            else
            {
                if (animator) animator?.SetTrigger("TakeHit");
            }
        }
        else
        {
            GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.HealthRestored, transform.position, $"+{value}");
        }
    }
    public void ChangeEnergy(int value)
    {
        UnitStats.Energy += value;
        UnitStats.Energy = Mathf.Clamp(UnitStats.Energy, 0, UnitStats.MaxEnergy);
        OnEnergyChanged?.Invoke(this, UnitStats.Energy);

        if (value < 0)
        {
            GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.EnergyBurned, transform.position, $"{value}");
        }
        else
        {
            GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.EnergyRestored, transform.position, $"+{value}");
        }
    }
    public void ChangeTime(int value)
    {
        UnitStats.Time += value;
        UnitStats.Time = Mathf.Clamp(UnitStats.Time, 0, UnitStats.MaxTime);
        OnTimeChanged?.Invoke(this, UnitStats.Time);

        if (value < 0)
        {
            GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.TimeBurned, transform.position, $"{value}");
        }
    }

    public void StartMovingByPath(List<PathNode> path)
    {
        StartCoroutine(MoveByPath(path));
    }
    public IEnumerator MoveByPath(List<PathNode> path)
    {
        usingAbility = true;
        if (animator) animator?.SetBool("Moving", true);

        Vector3 destination = path[path.Count - 1].node.transform.position;
        path.RemoveAt(path.Count - 1);
        pivot.rotation = Quaternion.LookRotation(destination - pivot.position);

        while ((destination - pivot.position).magnitude > moveSpeed * Time.deltaTime * 2)
        {
            pivot.position += (destination - pivot.position).normalized * moveSpeed * Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        pivot.position = destination;

        if (path.Count > 0)
        {
            StartCoroutine(MoveByPath(path));
        }
        else
        {
            usingAbility = false;
            if (animator) animator?.SetBool("Moving", false);
            OnFinishAbilityUse?.Invoke();
            path.Clear();
        }
    }
    
    public void StartRushingToPosition(Node destinationNode)
    {
        StartCoroutine(RushToPosition(destinationNode));
    }
    public IEnumerator RushToPosition(Node destinationNode)
    {
        usingAbility = true;
        if (animator) animator?.SetBool("Moving", true);

        Vector3 destination = destinationNode.transform.position;
        Vector3 startPos = pivot.position;
        pivot.rotation = Quaternion.LookRotation(destination - pivot.position);

        while ((destination - pivot.position).magnitude > moveSpeed * Time.deltaTime * 2)
        {
            pivot.position += (destination - pivot.position).normalized * moveSpeed * 5f * Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        pivot.position = destination;
        
        usingAbility = false;
        if (animator) animator?.SetBool("Moving", false);
        OnFinishAbilityUse?.Invoke();
    }
}
