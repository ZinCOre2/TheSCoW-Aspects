using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Move))]
public class Unit : Entity
{
    public event Action<Unit, int> OnHealthChanged;
    public event Action<Unit, int> OnEnergyChanged;
    public event Action<Unit> OnUnitDeath;
    public event Action OnFinishAbilityUse;

    [Header("General")]
    [SerializeField] private UnitData unitData;
    [SerializeField] private Animator animator;
    [SerializeField] private float moveSpeed = 50f;
    [SerializeField] private Transform pivot;
    [SerializeField] private GameObject marker;
    [SerializeField] private MeshRenderer highlight;
    [SerializeField] private int teamId;

    [Header("Abilities")]
    [SerializeField] private List<AbilityHolder.AType> deck = new List<AbilityHolder.AType>();

    // Stats
    public int health { get; private set; }
    public int energy { get; private set; }
    // General
    public UnitData UnitData_ { get { return unitData; } private set { unitData = value; } }
    public Animator AnimatorUnit { get { return animator; } private set { animator = value; } }
    public int TeamId { get { return teamId; } private set { teamId = value; } }
    // Abilities
    public List<AbilityHolder.AType> drawPile { get; private set; } = new List<AbilityHolder.AType>();
    public AbilityHolder.AType[] hand { get; private set; } = new AbilityHolder.AType[6];
    public List<AbilityHolder.AType> discardPile { get; private set; } = new List<AbilityHolder.AType>();

    public bool usingAbility { get; private set; } = false;

    private void Start()
    {
        SceneController.Instance.Grid.unitList.Add(this);
        SceneController.Instance.OnUnitSelect += MarkerUnit;

        marker.SetActive(false);
        SetNearbyCoordsAndPosition();

        if (teamId == 0) { return; }

        health = unitData.maxHealth;
        energy = unitData.maxEnergy;

        drawPile = deck;
        for (int i = 0; i < 3; i++)
        {
            DrawCard();
        }
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
        Debug.Log("Team " + teamId + " counter: " + SceneController.Counter[teamId - 1]);
        OnUnitDeath += (Unit unit) =>
        {
            if (unit == this)
            {
                SceneController.Instance.OnUnitSelect -= MarkerUnit;
                Destroy(pivot.gameObject, 3f);
            }
        };
        OnUnitDeath += SceneController.Instance.UnitDeath;
    }

    private Vector2Int GetNearbyCoords(Vector3 startPoint, Vector2Int gridSize, float nodeSize)
    {
        Vector2Int newCoords = new Vector2Int(Mathf.RoundToInt((transform.position.x - startPoint.x) / nodeSize),
            Mathf.RoundToInt((transform.position.z - startPoint.z) / nodeSize));

        if (newCoords.x < 0 || newCoords.x >= gridSize.x || newCoords.y < 0 || newCoords.y >= gridSize.y) // if not in grid range
            newCoords = new Vector2Int(-1, -1);
        return newCoords;
    }
    private void SetNearbyCoordsAndPosition()
    {
        Vector2Int testCoords;

        testCoords = GetNearbyCoords(SceneController.Instance.transform.position, new Vector2Int(SceneController.Instance.Grid.XSize,
            SceneController.Instance.Grid.YSize), SceneController.Instance.Grid.NodeSize);
        if (testCoords != new Vector2Int(-1, -1))
        {
            coords = testCoords;
        }
        pivot.transform.position = SceneController.Instance.transform.position +
                new Vector3(coords.x * SceneController.Instance.Grid.NodeSize, pivot.transform.position.y, coords.y * SceneController.Instance.Grid.NodeSize);
    }

    private void MarkerUnit(Unit unit)
    {
        if (unit == this)
        {
            marker?.SetActive(true);
            return;
        }
        marker?.SetActive(false);
    }
    public void ChangeHealth(int value)
    {
        health += value;
        health = Mathf.Clamp(health, 0, unitData.maxHealth);
        OnHealthChanged?.Invoke(this, health);

        if (health <= 0)
        {
            animator?.SetTrigger("Death");
            OnUnitDeath?.Invoke(this);
            // Dead, show animation, remove unit from scene soon, subtract from counter above
        }
        else
            if (value < 0)
            {
                animator?.SetTrigger("TakeHit");
            }
    }
    public void ChangeEnergy(int value)
    {
        energy += value;
        energy = Mathf.Clamp(energy, 0, unitData.maxEnergy);
        OnEnergyChanged?.Invoke(this, energy);
    }

    public bool DrawCard()
    {
        Ability card;
        int handIndex = -1;

        // Find free card slot in hand, else don't draw and exit method w/ false
        for (int i = 0; i < 6; i++)
        {
            if (hand[i] == AbilityHolder.AType.None)
            {
                handIndex = i;
                break;
            }
        }
        if (handIndex == -1) { return false; }

        // Draw card. If draw pile is empty, shuffle discard pile in it and check again (in case of both empty piles)
        if (drawPile.Count <= 0)
        {
            drawPile = discardPile;
            discardPile = new List<AbilityHolder.AType>();
        }
        if (drawPile.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, drawPile.Count - 1);
            card = AbilityHolder.Instance.GetAbility(drawPile[index]);
            hand[handIndex] = drawPile[index];
            UIController.Instance.AbilitySlots[handIndex + 1].gameObject.SetActive(true);
            UIController.Instance.AbilitySlots[handIndex + 1].SetAbility(card);

            drawPile.RemoveAt(index);
            return true;
        }
        
        return false;
    }
    public void DiscardCard(int index)
    {
        if (hand[index] != AbilityHolder.AType.None)
        {
            AbilityHolder.AType discardedCard = hand[index];
            discardPile.Add(discardedCard);

            hand[index] = new AbilityHolder.AType();
            hand[index] = AbilityHolder.AType.None;
            UIController.Instance.AbilitySlots[index + 1].SetAbility(null);
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
