using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class HoveringWorldText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 3f;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float yOffset;

    private float _time;
    
    public void StartHovering(Vector3 startPoint, string info)
    {
        transform.position = startPoint + Vector3.up * yOffset;
        text.text = info;

        _time = 0f;
        
        Destroy(gameObject, fadeDuration);
    }

    private void Update()
    {
        transform.position += Vector3.up * speed * Time.deltaTime;
        
        _time += Time.deltaTime;
        canvasGroup.alpha = 1f - _time / fadeDuration;
    }
}
