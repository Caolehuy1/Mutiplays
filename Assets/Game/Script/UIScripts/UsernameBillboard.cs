using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsernameBillboard : MonoBehaviour
{
    Camera mainCam;
    public void Update()
    {
        if( mainCam == null)
        {
            mainCam = FindObjectOfType<Camera>();
        }
        if(mainCam == null)
        {
            return;
        }
        transform.LookAt(mainCam.transform);
        transform.Rotate(Vector3.up * 1800);
    }
}
