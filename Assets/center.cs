using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class center : BaseBoot
{
    [SerializeField] private Button button1;
    [SerializeField] private CoursorBrain coursor;

    public async override UniTask Boot()
    {
        await base.Boot();
        Start1();
    }

    void Start1()
    {
        button1.onClick.AddListener(Center);
    }

    private void Center()
    {
        //coursor.AddX = coursor.additionalPose.x * -1;
        //coursor.AddY = coursor.additionalPose.y * -1;
        //Debug.Log("done");
    }
}
