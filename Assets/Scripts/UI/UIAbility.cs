using System;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class UIAbility : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public event Action<Ability, int> OnAbilitySelect;

    [SerializeField] private float scaleOnHover = 2f, scaleOnSelected = 1.3f;
    [SerializeField] private Image icon;
    [SerializeField] private Image cardBack;
    [SerializeField] private TextMeshProUGUI cardName, description, epCost, range, area;

    public float ScaleOnSelected { get { return scaleOnSelected; } private set { scaleOnSelected = value; } }
    public int Id { get; private set; }
    public Ability ability { get; private set; }

    public void SetId(int id) { Id = id; }
    public void SetAbility(Ability ability)
    {
        if (ability != null)
        {
            this.ability = ability;
            if (icon != null) icon.sprite = ability.AbilityInfo.icon;
            if (cardName != null) cardName.text = ability.AbilityInfo.cardName;
            if (description != null) description.text = ability.AbilityInfo.description;
            if (epCost != null) epCost.text = ability.AbilityInfo.epCost.ToString();
            if (range != null) range.text = ability.AbilityInfo.minRange.ToString() + "-" + ability.AbilityInfo.maxRange.ToString();
            if (area != null) area.text = ability.AbilityInfo.minAreaRange.ToString() + "-" + ability.AbilityInfo.maxAreaRange.ToString();
        }
        else
        {
            if (UIController.Instance.selectedAbilityId == Id)
            {
                transform.localScale = Vector3.one;
                if (TryGetComponent(out Image image))
                {
                    image.color = Color.white;
                }
            }
            gameObject.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (ability)
            OnAbilitySelect?.Invoke(ability, Id);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = Vector3.one * scaleOnHover;
        transform.SetAsLastSibling();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = Vector3.one;
    }
}
