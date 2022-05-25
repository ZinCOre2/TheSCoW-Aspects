using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UnitDataPanel : MonoBehaviour
{
    public Image Portrait;
    public TextMeshProUGUI UnitNameText;
    
    public Image HPBar;
    public Image EPBar;
    public Image TPBar;
    
    public TextMeshProUGUI HPStateText;
    public TextMeshProUGUI EPStateText;
    public TextMeshProUGUI TPStateText;
    
    public TextMeshProUGUI HPRegenText;
    public TextMeshProUGUI EPRegenText;

    [FormerlySerializedAs("AspectDedications")] public TextMeshProUGUI[] AspectDedicationsText = new TextMeshProUGUI[4];
    public GameObject[] AspectCovers = new GameObject[4];
    
    public TextMeshProUGUI PowerText;
    public TextMeshProUGUI DefenceText;
    
    public UIAbility[] AbilitySlots = new UIAbility[7];
    public Image[] CardBacks = new Image[6];
}
