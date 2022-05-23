using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform _cameraPos;
    private void Start()
    {
        _cameraPos = GameController.Instance.CameraController.transform;
    }
    
    private void Update()
    {
        var billboardTransform = transform;
        var billboardAngles = billboardTransform.localEulerAngles;
        
        billboardAngles = new Vector3(billboardAngles.x, _cameraPos.localEulerAngles.y, billboardAngles.z);
        billboardTransform.localEulerAngles = billboardAngles;
    }
}
