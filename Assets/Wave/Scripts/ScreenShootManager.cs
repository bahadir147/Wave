using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShootManager : MonoBehaviour
{
    int count;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            count++;
            ScreenCapture.CaptureScreenshot(count.ToString() + "_SS.png",3);
        }
    }
}
