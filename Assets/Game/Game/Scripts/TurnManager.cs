using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public const int QUEUE_CAPACITY = 8;
    
    public List<int> UnitTimeDelays = new List<int>();
    public Queue<int> TurnQueue = new Queue<int>();
    
    [Header("Turn Time")]
    public Timer TurnTimer;
    public float TurnDuration = 30;

    [HideInInspector]
    public int TeamId;

    private void Start()
    {
        if (TurnTimer == null)
        {
            TurnTimer = gameObject.AddComponent<Timer>();
        }

        TurnTimer.Reset(TurnDuration);

        TurnTimer.OnTimerElapsed += TurnTimer_OnTimerElapsed;
    }
    private void OnDisable()
    {
        TurnTimer.OnTimerElapsed -= TurnTimer_OnTimerElapsed;
    }

    // public void UpdateQueue()
    // {
    //     UnitTimeDelays.Clear();
    //     foreach (var masterUnit in GameController.Instance.EntityManager.MasterUnits)
    //     {
    //         for (var i = 0; i < QUEUE_CAPACITY; i++)
    //         {
    //             UnitTimeDelays.Add();
    //         }
    //     }
    // }
    
    private void TurnTimer_OnTimerElapsed()
    {
        // TeamId++;
        // if (TeamId >= GameMainManager.Instance.Players.Count)
        // {
        //     TeamId -= GameMainManager.Instance.Players.Count;
        //     GameMainManager.Instance.SelectionManager.ResetSelections(SelectionStage.None);
        // }
    }
}