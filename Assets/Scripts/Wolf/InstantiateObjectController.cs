using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class InstantiateObjectController : BaseBoot
{
    [SerializeField] private GameObject _eggPrefab;
    [SerializeField] private Transform[] _eggPositionTransform;
    [SerializeField] private float _delay;
    [SerializeField] private float _step;
    [SerializeField] private Slider _speed;

    [Inject] private IEggPoolService _eggPoolService;

    private int _lastRandomPosition = -1;



    private void FixedUpdate()
    {
        _delay -= Time.deltaTime;
        if (_delay <= 0)
        {
            int randomIndex = Random.Range(0, _eggPositionTransform.Length - 1);

            int randomPosition = randomIndex >= _lastRandomPosition 
                ? randomIndex + 1 
                : randomIndex;

            _lastRandomPosition = randomPosition;

            Debug.Log(_eggPoolService == null);
            _eggPoolService.GetEgg(_eggPositionTransform[randomPosition].position);

            //Instantiate(_eggPrefab, _eggPositionTransform[randomPosition].position, Quaternion.identity);

            _step = Random.Range(1f, _speed.value);
            _delay = _step;
        }
    }
}
