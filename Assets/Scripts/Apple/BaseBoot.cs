using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BaseBoot : MonoBehaviour
{
    [SerializeField] private int _bootDelay; 
    
    public async virtual UniTask Boot()
    {
        await UniTask.Delay(_bootDelay);
    }
    
    
}
