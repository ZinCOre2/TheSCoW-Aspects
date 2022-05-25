using System;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public event Action OnTimerElapsed;
    public event Action OnTimerChanged;
    [SerializeField] private float startingTime;
    public float StartingTime { get { return startingTime; } }
    public float TimeLeft { get; private set; }

    public bool Stopped { get; private set; } = false;

    private void Start()
    {
        Reset();
    }
    private void Update()
    {
        if (Stopped) return;

        if (TimeLeft > 0f)
        {
            TimeLeft -= Time.deltaTime;
            OnTimerChanged?.Invoke();
        }
        else
        {
            Stopped = true;
            OnTimerElapsed?.Invoke();
        }
    }

    public bool IsElapsed() { return TimeLeft <= 0; }
    public void SetTime(float newTime)
    {
        TimeLeft = newTime;
    }
    public void Reset()
    {
        TimeLeft = startingTime;
        Stopped = false;
    }
    public void Reset(float newTime)
    {
        startingTime = newTime;
        TimeLeft = startingTime;
        Stopped = false;
    }
}
