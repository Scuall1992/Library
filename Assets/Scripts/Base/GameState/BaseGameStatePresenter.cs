using Cysharp.Threading.Tasks;
using System;
using System.Diagnostics;
using System.Threading;
using UniRx;

public abstract class BaseGameStatePresenter : IGameStatePresenter, IDisposable
{
    protected readonly BaseGameStateModel model;
    protected readonly GameStateView view;

    private object _locker = new object();
    private CancellationTokenSource _source;
    private CompositeDisposable _disposables = new CompositeDisposable();

    protected BaseGameStatePresenter(BaseGameStateModel model, GameStateView view)
    {
        this.model = model;
        this.view = view;
    }

    protected virtual void StartTimer()
    {
        _source?.Cancel();
        _source = new CancellationTokenSource();

        RunTimer(_source.Token).Forget();
    }

    public virtual void Init()
    {
        model.RightCount.Subscribe(value =>
            {
                view.UpdateRight(value);
            })
            .AddTo(_disposables);

        model.MissCount.Subscribe(value =>
            {
                view.UpdateMiss(value);
            })
            .AddTo(_disposables);
        
        model.Time.Subscribe(value =>
            {
                view.UpdateTimer(value);
            })
            .AddTo(_disposables);

        StartTimer();
    }

    public virtual void StopTimer()
    {
        _source?.Cancel();
        _source = null;
    }

    public void AddMissValue()
    {
        model.MissCount.Value = SetCountValue(model.MissCount.Value);
    }

    public void AddRightValue()
    {
        model.RightCount.Value = SetCountValue(model.RightCount.Value);
    }

    public int GetRightValue()
    {
        return model.RightCount.Value;
    }

    public int GetMissValue()
    {
        return model.MissCount.Value;
    }

    public string GetTimeValue()
    {
        return $"{model.Time.Value / 60f:00}:{model.Time.Value % 60f:00}";
    }

    public virtual void ResetData()
    {
        model.Reset();
        model.ResetTime();
    }

    public virtual void Dispose()
    {
        _source?.Cancel();
        _disposables?.Dispose();
    }

    private async UniTaskVoid RunTimer(CancellationToken source)
    {
        while (!source.IsCancellationRequested)
        {
            await UniTask.Delay(1000, cancellationToken: source);
            model.Time.Value++;
        }
    }

    private int SetCountValue(int value)
    {
        bool blocked = false;

        Monitor.TryEnter(_locker, 0, ref blocked);

        try
        {
            if (blocked)
            {
                value++;
            }
        }
        finally
        {
            if (blocked)
            {
                Monitor.Exit(_locker);
            }
        }

        return value;
    }
}
