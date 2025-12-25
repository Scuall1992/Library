using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIControllerMaze : BaseBoot
{
    [SerializeField] private Button _backToMenu;
    [SerializeField] private FpsMovement _fpsMovement;
    [SerializeField] private BallController _ball;
    
    private BallController _ballController;

    public async override UniTask Boot()
    {
        await base.Boot();

        _backToMenu.onClick.AddListener(BackToMenu);

        _ballController = GameObject.FindWithTag("ball").GetComponent<BallController>();
        if (_ballController != null)
        {
            // Do something with the playerObject
            Debug.Log("Found player: " + _ballController.name);
        }
        else
        {
            Debug.LogWarning("Player object with tag 'Player' not found.");
        }
    }

    void BackToMenu()
    {
        TimerMaze.onResetData?.Invoke();

        _ball.StopReading();
        _fpsMovement.StopReading();
        SceneManager.LoadScene(0);

    }
}
