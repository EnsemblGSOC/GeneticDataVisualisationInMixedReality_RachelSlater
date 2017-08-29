using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureHandler : Singleton<GestureHandler>
{
    private bool isActive = false;

    void Update()
    {
        if (isActive)
        {
            Debug.Log("________________________________ABOUT TO ROTATE___________________________________ " + this.transform.position);
            this.transform.Rotate(0, 1, 0);
            Debug.Log("________________________________ABOUT TO ROTATE___________________________________ " + this.transform.position);
        }
    }

    void OnAirTapped()
    {
        isActive = !isActive;
    }
}