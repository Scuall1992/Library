using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneParams : MonoBehaviour
{
    private void Awake()
    {
        Screen.fullScreenMode = FullScreenMode.Windowed;
        Screen.SetResolution(1920, 1080, false);
    }
    
}
