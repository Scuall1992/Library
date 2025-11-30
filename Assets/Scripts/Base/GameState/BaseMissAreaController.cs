using UnityEngine;
using VContainer;

public class BaseMissAreaController : MonoBehaviour 
{
    private IGameStatePresenter _presenter;

    [Inject]
    public void Construct(IGameStatePresenter presenter)
    {
        _presenter = presenter;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        _presenter.AddMissValue();
    }
}
