using UnityEngine;

[System.Serializable]
public class MasterTurnDelay
{
    public int MasterId;
    public int RemainingDelay;

    public MasterTurnDelay(int masterId, int remainingDelay)
    {
        MasterId = masterId;
        RemainingDelay = remainingDelay;
    }

    public bool MatchesMasterId(int masterId) { return MasterId == masterId; }

    public int ReduceDelay(int amount)
    {
        RemainingDelay -= amount;
        if (RemainingDelay < 0)
        {
            if (GameController.Instance.EntityManager.FindMasterUnitByMasterId(MasterId, out MasterUnit masterUnit))
            {
                var increaseDelay = TurnManager.CalculateDelay(masterUnit.UnitStats.Time);
                RemainingDelay += increaseDelay * Mathf.CeilToInt(RemainingDelay / (float)increaseDelay);
            }
        }

        return RemainingDelay;
    }
}