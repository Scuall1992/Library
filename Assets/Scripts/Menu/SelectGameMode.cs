using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SelectGameMode : BaseBoot
{
    [SerializeField] private Image _selectImageT;
    [SerializeField] private Button _buttonTime;
    [Space] 
    [SerializeField] private Image _selectImageS;
    [SerializeField] private Button _buttonScore;
    [Space]
    [SerializeField] private Image _selectImageM;
    [SerializeField] private Button _buttonMarathon;

    private GameManager _manager;

    public async override UniTask Boot()
    {
        await base.Boot();
        StartSelect();
    }

    private void StartSelect()
    {
        if (_manager == null)
            _manager = GameManager.Instance;
        
        _buttonTime.onClick.AddListener(SetValue);
        _buttonScore.onClick.AddListener(SetValue);
        _buttonMarathon.onClick.AddListener(SetMarathonValue);
        
        _selectImageM.enabled = GameManager.Instance.Data.isMarathonMode;

        SwichBtnState();
    }

    private void SetMarathonValue()
    {
        _manager.Data.isMarathonMode = !_manager.Data.isMarathonMode;
        _selectImageM.enabled = _manager.Data.isMarathonMode;
    }

    private void SetValue()
    {
        _manager.Data.isTimeMode = !_manager.Data.isTimeMode;
        _manager.Data.isScoreMode = !_manager.Data.isScoreMode;
        SwichBtnState();
    }


    private void SwichBtnState()
    {
        _selectImageT.enabled = _manager.Data.isTimeMode;
        _selectImageS.enabled = _manager.Data.isScoreMode;
    }
}
