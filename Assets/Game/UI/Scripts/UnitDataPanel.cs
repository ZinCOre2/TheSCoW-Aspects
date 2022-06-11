using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UnitDataPanel : MonoBehaviour
{
    public Image Portrait;
    public TextMeshProUGUI UnitNameText;
    
    [Header("Stats")]
    public Image HPBar;
    public Image EPBar;
    public Image TPBar;
    
    public TextMeshProUGUI HPStateText;
    public TextMeshProUGUI EPStateText;
    public TextMeshProUGUI TPStateText;
    
    public TextMeshProUGUI HPRegenText;
    public TextMeshProUGUI EPRegenText;

    public TextMeshProUGUI[] AspectDedicationsText = new TextMeshProUGUI[4];
    public GameObject[] AspectCovers = new GameObject[4];
    
    public TextMeshProUGUI PowerText;
    public TextMeshProUGUI DefenceText;

    [Header("Ability Panel")]
    public UIAbility[] AbilitySlots = new UIAbility[4];
    
    [Header("Card Panel")]
    public GameObject CardPanel;
    public UICard[] CardSlots = new UICard[6];
    public Image[] CardBacks = new Image[6];

    public TextMeshProUGUI DrawPileSizeText;
    public TextMeshProUGUI DiscardPileSizeText;
}
