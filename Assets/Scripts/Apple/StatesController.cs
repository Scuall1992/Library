using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;

public class StatesController : BaseBoot
{
    [SerializeField] private TextMeshProUGUI rightText;
    [SerializeField] private TextMeshProUGUI missText;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject timer;
    [SerializeField] private TextMeshProUGUI levelText;

    private static object _locker = new object();

    public static int level = 0;
    public static int rightCount = 0;
    public static int missCount = 0;

    public static Action onRightAction;
    public static Action onMissAction;
    public static Action onSetLevel;

    public int test = 0;
    public async override UniTask Boot()
    {
        await base.Boot();
        onRightAction += AddRightCount;
        onMissAction += AddMissCount;
        onSetLevel += SetLevel;
    }
    private void OnDestroy()
    {
        onRightAction -= AddRightCount;
        onMissAction -= AddMissCount;
        onSetLevel -= SetLevel;
    }

    public void SetRightCount(int value)
    {
        bool lockTaken = false;

        Monitor.TryEnter(_locker, 0, ref lockTaken);

        try
        {
            if (lockTaken)
            {
                rightCount = value;
            }
            else
            {
                Debug.LogError("Lock is taken");
            }
        }
        finally
        {
            if (lockTaken)
            {
                Monitor.Exit(_locker);
            }
        }
    }

    public void SetMissCount(int value)
    {
        bool lockTaken = false;

        Monitor.TryEnter(_locker, 0, ref lockTaken);

        try
        {
            if (lockTaken)
            {
                missCount = value;
            }
            else 
            {
                Debug.LogError("Lock is taken");
            }
        }
        finally {
            if (lockTaken) {
                Monitor.Exit(_locker);
            }
        }

    }

    public void SetLevel()
    {
        level = GameManager.Instance.Data.showLevel;
        if (level == 0) levelText.text = "Тренировка";
        else levelText.text = $"Уровень {level}";
    }

    

    public Tuple<int, int> ResetData()
    {
        gameOverScreen.SetActive(false);
        timer.SetActive(false);
        Tuple<int, int> res;
        if (!GameManager.Instance.Data.isMarathonMode || GameManager.Instance.Data.isMarathonModeEnd)
        {
            res = new Tuple<int, int>(rightCount, missCount);
            rightCount = 0;
            missCount = 0;
            level = 0;
        }
        else
        {
            res = new Tuple<int, int>(rightCount, missCount);
        }


        VisualUpdate();
        return res;
    }

    private void AddRightCount()
    {
        SetRightCount(rightCount + 1);
        VisualUpdate();
    }

    private void AddMissCount()
    {
        SetMissCount(missCount + 1);
        VisualUpdate();
    }

    private void VisualUpdate()
    {
        rightText.text = rightCount.ToString();
        missText.text = missCount.ToString();
    }
}