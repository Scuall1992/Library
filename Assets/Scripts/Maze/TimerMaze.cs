using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;

public class TimerMaze : BaseBoot
{
    [SerializeField] private TextMeshProUGUI _timer;
    [SerializeField] private TextMeshProUGUI _fit;
    [SerializeField] private TextMeshProUGUI _type;
    public static int time = 0;

    public static Action onResetData;
    private CancellationTokenSource source;

    public async override UniTask Boot()
    {
        await base.Boot();

        onResetData += ResetData;

        source?.Cancel();
        source = new CancellationTokenSource();
        AddSecond();
    }

    private void OnDestroy()
    {
        onResetData -= ResetData;
    }

    public void ResetData()
    {
        GameManager.Instance.AddRecord(new PlayerRecord(DateTime.Now.Date.ToString("d"),
           _timer.text, _type.text, _fit.text));

        time = 0;

        source?.Cancel();
        source = new CancellationTokenSource();

        UpdateTimeText();
    }

    private async void AddSecond()
    {
        time += 1;
        UpdateTimeText();

        var result = await UniTask.Delay(1000, cancellationToken: source.Token)
            .SuppressCancellationThrow();

        if (result) return;

        AddSecond();
    }

    private void UpdateTimeText()
    {
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);

        _timer.text = $"{minutes:00}:{seconds:00}";
    }
}
