using System;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class UIAbility : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public event Action<Ability, int> OnAbilitySelect;
    
    public Transform ScaledCardHolder;    

    [SerializeField] private float scaleOnHover = 2.5f, scaleOnSelected = 1f;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI cardName, description, epCost, tpCost, range, area;
    [SerializeField] private GameObject[] aspectCovers = new GameObject[4];
    [SerializeField] private TextMeshProUGUI[] aspectDedications = new TextMeshProUGUI[4]; 

    public float ScaleOnSelected { get { return scaleOnSelected; } private set { scaleOnSelected = value; } }
    public int Id { get; private set; }
    public Ability ability { get; private set; }

    public void SetId(int id) { Id = id; }
    public void SetAbility(Ability ability)
    {
        if (ability != null)
        {
            this.ability = ability;
            
            var abilityInfo = ability.AbilityData;
            
            if (icon != null) icon.sprite = abilityInfo.icon;
            if (cardName != null) cardName.text = abilityInfo.cardName;
            if (description != null)
            {
                if (abilityInfo.values.Length > 0)
                {
                    var values = new object[abilityInfo.values.Length];
                    for (var i = 0; i < values.Length; i++)
                    {
                        values[i] = abilityInfo.values[i];
                    }
                    
                    description.text = String.Format(abilityInfo.description, values);
                }
                else
                {
                    description.text = abilityInfo.description;
                }
            }
            if (epCost != null) epCost.text = abilityInfo.epCost.ToString();
            if (tpCost != null) tpCost.text = abilityInfo.tpCost.ToString();
            if (range != null) range.text = $"{abilityInfo.minRange.ToString()}\n-\n{abilityInfo.maxRange.ToString()}";
            if (area != null) area.text = $"{abilityInfo.minAreaRange.ToString()}\n-\n{abilityInfo.maxAreaRange.ToString()}";
            
            for (var i = 0; i < 4; i++)
            {
                if (aspectDedications[i] != null)
                {
                    aspectDedications[i].text = $"{ability.AbilityData.Dedications[i].Value}";
                    aspectDedications[i].gameObject.SetActive(ability.AbilityData.Dedications[i].IsUsable);
                }
                
                if (aspectCovers[i] != null)
                {
                    aspectCovers[i].SetActive(!ability.AbilityData.Dedications[i].IsUsable);
                }
            }
        }
        else
        {
            if (GameController.Instance.UIController.selectedAbilityId == Id)
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

    public void SetAbility(AbilityHolder.AbilityType abilityType)
    {
        var abl = GameController.Instance.AbilityHolder.GetAbility(abilityType);
        
        SetAbility(abl);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (ability)
        {
            OnAbilitySelect?.Invoke(ability, Id);
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        ScaledCardHolder.localScale = Vector3.one * scaleOnHover;
        transform.SetAsLastSibling();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        ScaledCardHolder.localScale = Vector3.one;
    }
}
