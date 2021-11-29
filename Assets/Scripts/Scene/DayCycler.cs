using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayCycler : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 20f;

    private void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime, 0f, 0f);
    }
}
