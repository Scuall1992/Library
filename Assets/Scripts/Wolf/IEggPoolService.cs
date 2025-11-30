using UnityEngine;

public interface IEggPoolService
{
    void GetEgg(Vector3 position); 
    void ReleaseEgg(GameObject egg);
}
