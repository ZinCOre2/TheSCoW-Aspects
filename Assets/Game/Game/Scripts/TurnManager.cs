using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public const int QUEUE_CAPACITY = 8;
    
    public List<int> TeamIds = new List<int>();
    public List<Color> TeamColors = new List<Color>()
    {
        Color.blue,
        Color.red
    };
    public List<MasterTurnDelay> Delays = new List<MasterTurnDelay>(); 

    [Header("Turn Time")]
    public Timer TurnTimer;
    public float TurnDuration = 30f;

    [HideInInspector]
    public int MasterIndex;
    
    public int CurrentMasterId => Delays[MasterIndex].MasterId;
    public int CurrentTeamId
    {
        get
        {
            if (GameController.Instance.EntityManager.FindMasterUnitByMasterId(Delays[MasterIndex].MasterId,
                out MasterUnit masterUnit))
            {
                return 0;
            }
            return masterUnit.TeamId;
        }
    }

    public static int CalculateDelay(int time)
    {
        return (int)(10000 / Mathf.Sqrt(time));
    }
    
    private void Start()
    {
        // Set on when remaking system
        // SetTimer();
        // SetInitialMasterDelays();
        // ResetStates();
    }
    private void OnDisable()
    {
        TurnTimer.OnTimerElapsed -= TurnTimer_OnTimerElapsed;
    }
    
    private void SetTimer()
    {
        if (TurnTimer == null)
        {
            TurnTimer = gameObject.AddComponent<Timer>();
        }

        TurnTimer.Reset(TurnDuration);

        TurnTimer.OnTimerElapsed += TurnTimer_OnTimerElapsed;
    }

    public void SetInitialMasterDelays()
    {
        AddMasterDelays(GameController.Instance.EntityManager.MasterUnits.ToArray());
    }

    public void AddMasterDelays(MasterUnit[] masterUnits)
    {
        foreach (var masterUnit in masterUnits)
        {
            Delays.Add(new MasterTurnDelay(masterUnit.UnitStats.MasterId, masterUnit.UnitStats.MaxTime));
        }
    }

    private void TurnTimer_OnTimerElapsed()
    {
        ResetStates();
    }

    private void ResetStates()
    {
        MasterIndex = FindLowestDelayWithOffset(0);

        ReduceAllDelays(Delays[MasterIndex].RemainingDelay);
        
        GameController.Instance.UIController.TurnQueuePanel.SetQueue();
        GameController.Instance.SelectionManager.ResetSelections(SelectionStage.None);
    }

    public int[] GetNewQueue()
    {
        var queue = new int[QUEUE_CAPACITY];
        var lowestIndex = FindLowestDelayWithOffset(Delays[MasterIndex].RemainingDelay);
        var offset = Delays[lowestIndex].RemainingDelay;
        
        queue[0] = Delays[lowestIndex].MasterId;

        for (var i = 1; i < QUEUE_CAPACITY; i++)
        {
            
        }

        return queue;
    }

    private int FindLowestDelayWithOffset(int offset)
    {
        var lowestDelayIndex = 0;

        for (var i = 0; i < QUEUE_CAPACITY; i++)
        {
            if (Delays[i].RemainingDelay + offset < Delays[lowestDelayIndex].RemainingDelay + offset &&
                Delays[i].RemainingDelay + offset >= 0)
            {
                lowestDelayIndex = i;
            }
        }

        return lowestDelayIndex;
    }
    
    private int FindLowestDelayWithOffsetPositive(int offset)
    {
        var lowestDelayIndex = 0;

        for (var i = 0; i < QUEUE_CAPACITY; i++)
        {
            if (Delays[i].RemainingDelay + offset < Delays[lowestDelayIndex].RemainingDelay + offset &&
                Delays[i].RemainingDelay + offset > 0)
            {
                lowestDelayIndex = i;
            }
        }

        return lowestDelayIndex;
    }

    private void ReduceAllDelays(int amount)
    {
        for (var i = 0; i < Delays.Count; i++)
        {
            Delays[i].ReduceDelay(amount);
        }
    }
}