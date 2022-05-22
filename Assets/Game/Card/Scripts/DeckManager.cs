
using System.Collections.Generic;

[System.Serializable]
public class DeckManager
{
    public const int HAND_SIZE = 6;
    
    public List<AbilityHolder.AType> DrawPile = new List<AbilityHolder.AType>();
    public AbilityHolder.AType[] Hand = new AbilityHolder.AType[HAND_SIZE];
    public List<AbilityHolder.AType> DiscardPile = new List<AbilityHolder.AType>();

    public void SetStartingDeck(List<AbilityHolder.AType> abilities)
    {
        foreach (var ability in abilities)
        {
            DrawPile.Add(ability);
        }
    }

    public bool DrawCard()
    {
        MoveCard();

        // Draw card. If draw pile is empty, shuffle discard pile in it and check again (in case of both empty piles)
        if (DrawPile.Count <= 0)
        {
            ShuffleDeck();
        }
        if (DrawPile.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, DrawPile.Count - 1);
            var card = GameController.Instance.AbilityHolder.GetAbility(DrawPile[index]);
            
            Hand[0] = DrawPile[index];
            
            GameController.Instance.UIController.AbilitySlots[1].gameObject.SetActive(true);
            GameController.Instance.UIController.AbilitySlots[1].SetAbility(card);

            DrawPile.RemoveAt(index);
            return true;
        }
        
        return false;
    }

    public void MoveCard()
    {
        var i = 0;
        for (; i < HAND_SIZE; i++)
        {
            if (Hand[i] == AbilityHolder.AType.None)
            {
                break;
            }
        }

        if (i == HAND_SIZE)
        {
            DiscardCard(HAND_SIZE - 1);
            i--;
        }
        
        GameController.Instance.UIController.AbilitySlots[i + 1].gameObject.SetActive(true);
        
        for (; i > 0; i--)
        {
            Hand[i] = Hand[i - 1];
            GameController.Instance.UIController.AbilitySlots[i + 1].SetAbility(Hand[i]);
        }

        Hand[0] = AbilityHolder.AType.None;
        GameController.Instance.UIController.AbilitySlots[1].SetAbility(Hand[0]);
    }

    public void DiscardCard(int index)
    {
        DiscardPile.Add(Hand[index]);
        
        Hand[index] = AbilityHolder.AType.None;
        
        GameController.Instance.UIController.AbilitySlots[index + 1].SetAbility(AbilityHolder.AType.None);
    }

    public void ShuffleDeck()
    {
        if (DiscardPile.Count >= 1)
        {
            foreach (var ability in DiscardPile)
            {
                DrawPile.Add(ability);
            }
            DiscardPile.Clear();
        }
    }
}
