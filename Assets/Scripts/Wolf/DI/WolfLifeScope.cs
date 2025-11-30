using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class WolfLifeScope : LifetimeScope
{
    [SerializeField] private GameStateView stateView;
    [SerializeField] private BaseMenuUI baseMenuView;
    [SerializeField] private WolfMissController missController;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject eggPrefab;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<BaseGameStateModel>(Lifetime.Scoped);
        builder.Register<BaseMenuPresenter>(Lifetime.Scoped);

        builder.RegisterComponent(playerController);
        builder.RegisterComponent(stateView);
        builder.RegisterComponent(baseMenuView);
        builder.RegisterComponent(missController);

        builder.Register<GameStatsPresenterWolf>(Lifetime.Scoped)
                .As<IGameStatePresenter, GameStatsPresenterWolf>();

        builder.Register<WolfMenuPresenter>(Lifetime.Scoped)
            .As<IStartable, WolfMenuPresenter>(); ;

        builder.RegisterInstance(eggPrefab).AsSelf();
        builder.Register<EggPoolService>(Lifetime.Scoped)
                .As<IEggPoolService, EggPoolService>();

        builder.RegisterComponentInHierarchy<InstantiateObjectController>();

        builder.RegisterEntryPoint<WolfGameEntryPoint>(Lifetime.Scoped)
            .As<IStartable, IDisposable>();
    }
}
