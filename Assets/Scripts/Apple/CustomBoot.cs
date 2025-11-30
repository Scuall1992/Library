
using System.Collections.Generic;
using UnityEngine;

public class CustomBoot : MonoBehaviour
{
    [SerializeField] private List<BaseBoot> _booters;

    private async void Awake()
    {
        foreach (var boot in _booters)
        {
            await boot.Boot();
        }
    }
}
