
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : BaseBoot
{
    [SerializeField] private Button _backToSettings;
    [SerializeField] private Button _exitGame;
    [SerializeField] private Button _backToMenu;
    //[SerializeField] private Button _backToMenuFromResult;
    //[SerializeField] private Button _openResultPanel;
    [Space]
    [SerializeField] private List<Button> _levelsList;
    [Space]
    [SerializeField] private GameObject _levelsPanel;
    [SerializeField] private GameObject _gamePanel1;
    //[SerializeField] private GameObject _resultPanel;
    [SerializeField] private InstantiateBallsController _gamePanel;
    [SerializeField] private CoursorBrain _coursorBrain;
    [SerializeField] private StatesController _statesController;
    [SerializeField] private Timer _timer;
    [SerializeField] private GameObject _blurWallR;
    //[SerializeField] private GameObject _blurWallL;
    //[SerializeField] private LoadRecordsData _loadRecordsData;
    [SerializeField] private Transform mintr;
    [SerializeField] private Transform maxtr;
    [SerializeField] private Camera camera;
    [SerializeField] private TMP_InputField _scoreInput;
    [SerializeField] private TMP_InputField _timeInput;


    private float currentT;

    public static Action onOpenLevel;
    public static Action onBackToMenu;
    private GameManager _manager;
    public static int test = 0;

    public async override UniTask Boot()
    {
        await base.Boot();
        StartMenu();
    }

    private void OnDestroy()
    {
        onOpenLevel -= OpenLvl;
        onBackToMenu -= BackToMenu;
    }


    private void StartMenu()
    {
        if (test == 0)
        {
            test = GameManager.Instance.Data.marathonScore;
        }
        if (_manager == null)
            _manager = GameManager.Instance;

        onOpenLevel += OpenLvl;

        onBackToMenu += BackToMenu;


        _scoreInput.text = GameManager.Instance.Data.marathonScore.ToString();
        _timeInput.text = GameManager.Instance.Data.marathonTime.ToString();
        _scoreInput.onEndEdit.AddListener(delegate
        {
            _manager.Data.marathonScore = int.Parse(_scoreInput.text);
        });

        _timeInput.onEndEdit.AddListener(delegate
        {
            _manager.Data.marathonTime = int.Parse(_timeInput.text);
        });


        _backToMenu.onClick.AddListener(BackToMenu);
        _exitGame.onClick.AddListener(ExitGame);
        _backToSettings.onClick.AddListener(() =>
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            Screen.SetResolution(1920, 1080, true);
     
            SceneManager.LoadScene(0);  
        });

        foreach (var levelButton in _levelsList)
        {
            levelButton.onClick.AddListener(delegate{OpenLevel(levelButton);});
        }

        currentT = (float)Screen.width / (float)Screen.height;
        UpdateResolution();
    }

    private void OpenLvl()
    {
        var level = GameManager.Instance.Data.showLevel;

        var button = _levelsList.FindIndex(x => x.gameObject.GetComponent<Level>().GetShowLevel() == level);

        BackToMenu();



        if (button + 1 < _levelsList.Count)
        {
            OpenLevel(_levelsList[button + 1]);
        }
    }

    private void BackToMenu()
    {
        Time.timeScale = 1f;

       /* if (!GameManager.Instance.Data.isMarathonMode || GameManager.Instance.Data.isMarathonModeEnd)
        {*/

            var time = _timer.EndTimer();
            var score = _statesController.ResetData();
            //GameManager.Instance.Data.marathonScoreSum = GameManager.Instance.Data.marathonScore;
            if (GameManager.Instance.Data.isMarathonModeEnd)
            {
                GameManager.Instance.Data.isMarathonModeEnd = false;
            }
            GameManager.Instance.AddRecord(new PlayerRecord(DateTime.Now.Date.ToString("d"),
                GameManager.Instance.Data.isTimeMode, score.Item1, score.Item2, time, GameManager.Instance.Data.showLevel.ToString()));

       /*     test = 0;
        }
        else
        {
            var score = _statesController.ResetData();
            _timer.SetTimer(test);
        }*/

        
        
        _levelsPanel.SetActive(true);
        _statesController.gameObject.SetActive(false);
        
        _gamePanel.ExitLevel();
        _gamePanel.gameObject.SetActive(false);

    }

    private void OpenLevel(Button button)
    {
        button.GetComponent<Level>().LevelStart();
        _levelsPanel.SetActive(false);
        
        _gamePanel.gameObject.SetActive(true);


        _gamePanel.PlayLevel();

        if (!_gamePanel1.activeSelf)
            _gamePanel1.SetActive(true);

        _statesController.gameObject.SetActive(true);
        if (!GameManager.Instance.Data.isMarathonMode || (GameManager.Instance.Data.isMarathonMode && GameManager.Instance.Data.level == 7))
        {
            _statesController.ResetData();

        }
    }

    private void ExitGame() {
        _coursorBrain.StopReading();
        SceneManager.LoadScene(0);
    }


    void UpdateResolution()
    {
        mintr.position = camera.ScreenToWorldPoint(
    new Vector3(0, 0, camera.nearClipPlane)
);

        maxtr.position = camera.ScreenToWorldPoint(
            new Vector3(Screen.width, Screen.height, camera.nearClipPlane)
        );
    }

}
