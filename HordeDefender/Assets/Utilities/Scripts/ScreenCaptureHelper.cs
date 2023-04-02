using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenCaptureHelper : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
            CaptureScreen();
    }
    int count=1;
    void CaptureScreen()
    {
        ScreenCapture.CaptureScreenshot("SomeLevel"+count+".png");
        count++;
    }
}
