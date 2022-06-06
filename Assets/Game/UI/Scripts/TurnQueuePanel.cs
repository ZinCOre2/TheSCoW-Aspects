using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnQueuePanel : MonoBehaviour
{
    [SerializeField] private TurnQueueUnitPanel unitPanelPrefab;

    private readonly TurnQueueUnitPanel[] _queuePanels = new TurnQueueUnitPanel[TurnManager.QUEUE_CAPACITY];

    public void SetQueue()
    {
        var masterIdQueue = GameController.Instance.TurnManager.GetNewQueue();
        for (var i = 0; i < _queuePanels.Length; i++)
        {
            if (!GameController.Instance.EntityManager.FindMasterUnitByMasterId(masterIdQueue[i], out var masterUnit)) { continue; }
            
            _queuePanels[i].ChangePortrait(masterUnit.UnitStats.Portrait);
            _queuePanels[i].ChangeBackgroundColor(GameController.Instance.TurnManager.TeamColors[masterUnit.UnitStats.TeamId - 1]);
        }
    }
}