using UnityEngine;

[RequireComponent(typeof(Timer))]
public class AbilityEffect : MonoBehaviour, IPooledObject
{
    [SerializeField] private Timer timer;
    [SerializeField] private string effectTag;
    [SerializeField] private float duration = 1.2f; 
    public string EffectTag { get { return effectTag; } private set { effectTag = value; } }

    private void Start()
    {
        timer.OnTimerElapsed += () =>
        {
            OnObjectDespawn();
        };
    }

    public void OnObjectSpawn()
    {
        timer.Reset(duration);
    }
    public void OnObjectDespawn()
    {
        gameObject.SetActive(false);
    }
}
