using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitDataPanel : MonoBehaviour
{
    [Header("Unit Data")]
    [SerializeField] private Image portrait;
    [SerializeField] private TextMeshProUGUI unitNameText;
    [SerializeField] private Image hpBar, epBar, tpBar;
    [SerializeField] private TextMeshProUGUI hpStateText, epStateText, tpStateText, hpRegenText, epRegenText, powerText, defenceText;
    [SerializeField] private UIAbility[] abilitySlots = new UIAbility[7];
    [SerializeField] private Image[] cardBacks = new Image[6];
}
