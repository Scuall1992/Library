using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : BaseBoot
{
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private TextMeshProUGUI _centerTimerText;
    [SerializeField] private GameObject _centerTimerPanel;
    [SerializeField] private GameObject _endGamePanel;
    [SerializeField] private GameObject _gamePanel;
    [SerializeField] private Button _nextLevelButton;
    [SerializeField] private Button _endButton;
    [SerializeField] private TextMeshProUGUI _count;
    [SerializeField] private InterctableObject _obj;
    [SerializeField] private CoursorBrain _coursorBrain;

    private CancellationTokenSource source;
    
    private float _timeLeft = 0f;
    private static int test = 0;

    private bool ttt;
    public static Timer Instance;

    public async override UniTask Boot()
    {
        await base.Boot();
        
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);

        _nextLevelButton.onClick.AddListener(NextLevelButton);
        _endButton.onClick.AddListener(EndButton);
    }

    private void NextLevelButton()
    {
        EndTimer();
        MenuController.onOpenLevel?.Invoke();
    }

    private void EndButton()
    {
        GameManager.Instance.Data.marathonScoreSum = 0;
        GameManager.Instance.Data.marathonTimeSum = 0;
        GameManager.Instance.Data.isMarathonModeEnd = true;
        ttt = false;
        Debug.LogError($"{GameManager.Instance.Data.marathonScoreSum}_|_{GameManager.Instance.Data.marathonTimeSum}");
        MenuController.onBackToMenu?.Invoke();
    }

    public void CenterTimer()
    {
        source?.Cancel();
        source = new CancellationTokenSource();

        _obj.StopPlay();

        _centerTimerPanel.SetActive(true); 
        _timeLeft = GameManager.Instance.Data.centerTime;
        MinusSecondCenter();
    }

    public void SetTimer()
    {
        if (_timerText.gameObject.activeSelf) return;

        source?.Cancel();
        source = new CancellationTokenSource();

        Time.timeScale = 1f;
        _endGamePanel.SetActive(false);
        if (GameManager.Instance.Data.level == 1)
        {
            _timerText.gameObject.SetActive(false);
            return;
        }

        _timerText.gameObject.SetActive(true);

        ttt = true;
        if (GameManager.Instance.Data.marathonScoreSum == 0 && GameManager.Instance.Data.isMarathonMode)
        {

            GameManager.Instance.Data.marathonScoreSum = GameManager.Instance.Data.marathonScore;
        }

        if (GameManager.Instance.Data.isTimeMode)
        {
            if (GameManager.Instance.Data.isMarathonMode)
            {
                if (GameManager.Instance.Data.marathonTimeSum == 0)
                {
                    _timeLeft = GameManager.Instance.Data.marathonTime;
                }
                else
                {
                    _timeLeft = GameManager.Instance.Data.marathonTimeSum;
                }
            }
            else
            {
                _timeLeft = GameManager.Instance.Data.timeValue;
            }
            
            MinusSecond();
        }
        else
        {
            if (GameManager.Instance.Data.marathonTimeSum == 0)
            {
                _timeLeft = 0;
            }
            else
            {
                _timeLeft = GameManager.Instance.Data.marathonTimeSum;
            }
            AddSecond();
        }
    }



    public string EndTimer()
    {
        string res = _timerText.text;
        source?.Cancel();
        source = new CancellationTokenSource();
        _timerText.text = $"{00:00} : {00:00}";

        return res;
    }

    private async void AddSecond()
    {
        if (!ttt) return;
        
        _timeLeft += 1;        
        UpdateTimeText();
        var result = await UniTask.Delay(1000, cancellationToken: source.Token).SuppressCancellationThrow();
        if (result) return;
        AddSecond();
    }

    private async void MinusSecondCenter()
    {
        if (_timeLeft <= 0) return;
        _timeLeft -= 1;
        UpdateTimeTextCenter();
        var result = await UniTask.Delay(1000, cancellationToken: source.Token).SuppressCancellationThrow();
        if (result) return;
        MinusSecondCenter();
    }
    private async void MinusSecond()
    {

        if (!ttt) return;
        _timeLeft -= 1;
        UpdateTimeText();
        var result = await UniTask.Delay(1000, cancellationToken: source.Token).SuppressCancellationThrow();
        if (result) return;
        MinusSecond();
    }

    private void UpdateTimeTextCenter()
    {
        if (_timeLeft <= 0)
        {
            _timeLeft = 0;
            
            _centerTimerPanel.SetActive(false);
            _centerTimerText.text = "00";
            _obj.Play(GameManager.Instance.Data.level);
            _coursorBrain.isCalibrate = false;
            source?.Cancel();
            source = new CancellationTokenSource();
            _coursorBrain.FinalCenter();
        }

        float seconds = Mathf.FloorToInt(_timeLeft % 60);

        _centerTimerText.text = $"{seconds:00}";
    }

    private void UpdateTimeText()
    {
        if (_timeLeft <= 0 && GameManager.Instance.Data.isTimeMode)
        {
            _timeLeft = 0;
            source?.Cancel();
            source = new CancellationTokenSource();

            if (!GameManager.Instance.Data.isMarathonMode)
            {
                _endGamePanel.SetActive(true);

                _gamePanel.SetActive(false) ;

                if (GameManager.Instance.Data.level != 7)
                    _nextLevelButton.gameObject.SetActive(GameManager.Instance.Data.isMarathonMode);
                else
                    _nextLevelButton.gameObject.SetActive(false);

                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = 0f;
                _endGamePanel.SetActive(true);

                _gamePanel.SetActive(false);
                //test += GameManager.Instance.Data.marathonScore;
                if (GameManager.Instance.Data.level != 7)
                    _nextLevelButton.gameObject.SetActive(GameManager.Instance.Data.isMarathonMode);
                else
                {
                    GameManager.Instance.Data.isMarathonModeEnd = true;
                    _nextLevelButton.gameObject.SetActive(false);

                }

                //NextLevelButton();
            }
        }

        else if ((Convert.ToInt32(_count.text) == GameManager.Instance.Data.score && GameManager.Instance.Data.isScoreMode &&
             !GameManager.Instance.Data.isMarathonMode) ||
            (Convert.ToInt32(_count.text) ==  GameManager.Instance.Data.marathonScoreSum &&
             GameManager.Instance.Data.isScoreMode && GameManager.Instance.Data.isMarathonMode))
        {
            source?.Cancel();
            source = new CancellationTokenSource();

            if (!GameManager.Instance.Data.isMarathonMode)
            {
                _endGamePanel.SetActive(true);

                _gamePanel.SetActive(false);

                if (GameManager.Instance.Data.level != 7)
                    _nextLevelButton.gameObject.SetActive(GameManager.Instance.Data.isMarathonMode);
                else
                {

                    GameManager.Instance.Data.isMarathonModeEnd = true;
                    _nextLevelButton.gameObject.SetActive(false);
                }

                Time.timeScale = 0f;
            }
            else
            {
               
                    GameManager.Instance.Data.marathonScoreSum += GameManager.Instance.Data.marathonScore;
                Debug.LogError($"___{GameManager.Instance.Data.marathonScoreSum}");
                Time.timeScale = 0f;
                _endGamePanel.SetActive(true);

                _gamePanel.SetActive(false);
                //test += GameManager.Instance.Data.marathonScore;
                if (GameManager.Instance.Data.level != 7)
                    _nextLevelButton.gameObject.SetActive(GameManager.Instance.Data.isMarathonMode);
                else
                {
                    GameManager.Instance.Data.isMarathonModeEnd = true;
                    _nextLevelButton.gameObject.SetActive(false);
                   
                }

                //NextLevelButton();
            }
        }
        GameManager.Instance.Data.marathonTimeSum = _timeLeft;

        float minutes = Mathf.FloorToInt(_timeLeft / 60);
        float seconds = Mathf.FloorToInt(_timeLeft % 60);
        
        _timerText.text = $"{minutes:00} : {seconds:00}";
    }
}
