using UnityEngine;

public class ScreenshotMaker : MonoBehaviour
{
    [SerializeField] private KeyCode keyCode;
    void Update()
    {
        if (Input.GetKeyDown(keyCode))
        {
            ScreenCapture.CaptureScreenshot("C:\\Users\\mkuzm\\OneDrive\\Рабочий стол\\UnityScreenshots\\thumb.png", 2);
        }
    }
}