using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

[RequireComponent(typeof(Move))]
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
    
    // Stats
    public int health { get; private set; }
    public int energy { get; private set; }
    public int time { get; private set; }

    // General
    public UnitData UnitData { get { return unitData; } private set { unitData = value; } }
    public Animator Animator { get { return animator; } private set { animator = value; } }
    public int TeamId { get { return teamId; } private set { teamId = value; } }

    public bool usingAbility { get; private set; } = false;

    // private void OnEnable()
    // {
    //     GameController.Instance.SceneController.OnUnitSelect += MarkUnit;
    //     
    //     if (teamId == 0) { return; }
    //     
    //     OnUnitDeath += UnitDeath;
    //     OnUnitDeath += GameController.Instance.SceneController.UnitDeath;
    // }

    protected override void Start()
    {
        base.Start();
        
        GameController.Instance.SceneController.OnUnitSelect += MarkUnit;

        if (teamId == 0) { return; }

        health = unitData.maxHealth;
        energy = unitData.maxEnergy;
        time = unitData.maxTime;

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

        SceneController.Counter[teamId - 1]++;
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
        }
    }

    public void ChangeHealth(int value)
    {
        if (value < 0)
        {
            value += UnitData.defence;
            value = Mathf.Clamp(value, Int32.MinValue, 0);
        }
        
        health += value;
        health = Mathf.Clamp(health, 0, unitData.maxHealth);
        OnHealthChanged?.Invoke(this, health);

        if (value < 0)
        {
            GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.DamageTaken, transform.position, $"{value}");

            if (health <= 0)
            {
                animator?.SetTrigger("Death");
                OnUnitDeath?.Invoke(this);
                // Dead, show animation, remove unit from scene soon, subtract from counter above
            }
            else
            {
                animator?.SetTrigger("TakeHit");
            }
        }
        else
        {
            GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.HealthRestored, transform.position, $"+{value}");
        }
    }
    public void ChangeEnergy(int value)
    {
        energy += value;
        energy = Mathf.Clamp(energy, 0, unitData.maxEnergy);
        OnEnergyChanged?.Invoke(this, energy);

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
        time += value;
        time = Mathf.Clamp(time, 0, unitData.maxTime);
        OnTimeChanged?.Invoke(this, time);

        if (value < 0)
        {
            GameController.Instance.WorldUIManager.CreateHoveringWorldText(HWTType.TimeBurned, transform.position, $"{value}");
        }
    }

    public IEnumerator MoveByPath(List<PathNode> path)
    {
        usingAbility = true;
        animator?.SetBool("Moving", true);

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
            animator?.SetBool("Moving", false);
            OnFinishAbilityUse?.Invoke();
        }
    }
}
