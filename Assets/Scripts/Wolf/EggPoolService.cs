using UnityEngine;
using UnityEngine.Pool;

public class EggPoolService : IEggPoolService
{
    private readonly GameObject _eggPrefab;
    private ObjectPool<GameObject> _pool;

    public EggPoolService(GameObject eggPrefab)
    {
        _eggPrefab = eggPrefab;
        InitializePool();
    }

    private void InitializePool()
    {
        _pool = new ObjectPool<GameObject>(
            createFunc: () => Object.Instantiate(_eggPrefab),
            actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) => Object.Destroy(obj),
            defaultCapacity: 10,
            maxSize: 10);
    }

    public void GetEgg(Vector3 position)
    {
        var egg = _pool.Get();
        egg.transform.position = position;
    }

    public void ReleaseEgg(GameObject egg)
    {
        egg.transform.rotation = Quaternion.identity;
        _pool.Release(egg);
    }
}
