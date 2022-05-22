using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    
    
    public Queue<int> TurnQueue = new Queue<int>();

    public int TurnId;
    
    [SerializeField] private Timer turnTimer;
}
