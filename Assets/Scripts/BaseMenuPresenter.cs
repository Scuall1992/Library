using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class BaseMenuPresenter
{
    protected readonly BaseMenuUI view;
    protected readonly CompositeDisposable disposables;

    protected BaseMenuPresenter(BaseMenuUI view)
    {
        this.view = view;
    }

    protected async UniTask BackToMenu(Types types)
    {
        await Addressables.LoadSceneAsync(BaseScenesType.Type[types],
            LoadSceneMode.Single).ToUniTask();
    }
}
