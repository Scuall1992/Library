using UnityEngine;
using VContainer;

public class WolfMissController : BaseMissAreaController
{

    [Inject] private IEggPoolService _eggPoolService;

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        base.OnTriggerEnter2D(collider);

        _eggPoolService.ReleaseEgg(collider.gameObject);
    }

}
