using System;
using UnityEngine;
using VContainer.Unity;

public class WolfGameEntryPoint : IStartable, IDisposable
{
    private readonly GameStatsPresenterWolf _gameStatsPresenter;
    private readonly WolfMenuPresenter _menuPresenter;

    public WolfGameEntryPoint(GameStatsPresenterWolf gameStatsPresenter, WolfMenuPresenter menuPresenter)
    {
        _gameStatsPresenter = gameStatsPresenter;
        _menuPresenter = menuPresenter;
    }

    public void Start()
    {
        _gameStatsPresenter.Start();
        _menuPresenter.Start();
    }

    public void Dispose()
    {
        if (_gameStatsPresenter is IDisposable gameStatsDisposable)
        {
            gameStatsDisposable.Dispose();
        }
        if (_menuPresenter is IDisposable menuDisposable)
        {
            menuDisposable.Dispose();
        }
    }
}
