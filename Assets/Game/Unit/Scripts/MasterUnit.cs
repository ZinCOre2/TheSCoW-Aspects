using UnityEngine;


public class MasterUnit : Unit
{
    public MasterUnitData MasterUnitData;
    // Abilities
    public DeckManager DeckManager = new DeckManager();
    
    protected override void Start()
    {
        base.Start();
        
        DeckManager.SetStartingDeck(MasterUnitData.StartingDeck);
        for (int i = 0; i < 3; i++)
        {
            DeckManager.DrawCard();
        }
    }
}
