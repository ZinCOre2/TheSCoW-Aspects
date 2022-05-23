using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public Queue<int> TurnQueue = new Queue<int>();
    
    [Header("Turn Time")]
    public Timer TurnTimer;
    public float TurnDuration;

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