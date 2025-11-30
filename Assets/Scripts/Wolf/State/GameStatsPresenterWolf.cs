using UnityEngine;
using VContainer.Unity;

public class GameStatsPresenterWolf : BaseGameStatePresenter
{
    public GameStatsPresenterWolf(BaseGameStateModel model, GameStateView view) 
        : base(model, view)
    {

    }

    public void Start()
    {
        Init();
    }
}
