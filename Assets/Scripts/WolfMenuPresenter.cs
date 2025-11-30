using Cysharp.Threading.Tasks;
using System;
using UniRx;
using VContainer.Unity;

public class WolfMenuPresenter : BaseMenuPresenter, IStartable, IDisposable
{
    private readonly BaseMenuUI _view;
    private CompositeDisposable _disposables = new CompositeDisposable();

    protected WolfMenuPresenter(BaseMenuUI view) : base(view)
    {
        _view = view;
    }

    public void Start()
    {
       /* _view.OnClick
            .Subscribe(async _ => await BackToMenu(Types.Menu))
            .AddTo(_disposables);*/
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }
}
