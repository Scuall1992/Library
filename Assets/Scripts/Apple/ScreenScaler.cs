using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ScreenScaler : BaseBoot
{
    [SerializeField] private int screenWidth;
    [SerializeField] private int screenHeight;

    public async override UniTask Boot()
    {
        await base.Boot();
        UpdateResolution();
    }


    void UpdateResolution()
    {
        
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        Screen.SetResolution(screenWidth, screenHeight, true);

    }
}
