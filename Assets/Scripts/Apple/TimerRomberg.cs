using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerRomberg : BaseBoot
{
    [SerializeField] private TextMeshProUGUI _centerTimerText;
    [SerializeField] private GameObject _centerTimerPanel;
    [SerializeField] private NewMonoBehaviourScript newMonoBehaviourScript;

    private CancellationTokenSource source;
    
    private float _timeLeft = 0f;

    public async override UniTask Boot()
    {
        await base.Boot();
        source?.Cancel();
        source = new CancellationTokenSource();

    }

    public void StartTimer()
    {
       

        _centerTimerPanel.SetActive(true);
        _timeLeft = 10f;
        MinusSecondCenter();
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

    private void UpdateTimeTextCenter()
    {
        if (_timeLeft <= 0)
        {
            _timeLeft = 0;
            
            _centerTimerPanel.SetActive(false);
            _centerTimerText.text = "00";

            source?.Cancel();
            source = new CancellationTokenSource();

            newMonoBehaviourScript.DrawGraph();
            newMonoBehaviourScript.DrawPoint(
                newMonoBehaviourScript.centerMass(), Color.yellow);
            newMonoBehaviourScript.trajectoryLength();
            newMonoBehaviourScript.RMS();
            newMonoBehaviourScript.countPointsByQuadrants();
        }

        float seconds = Mathf.FloorToInt(_timeLeft % 60);

        _centerTimerText.text = $"{seconds:00}";
    }

}
