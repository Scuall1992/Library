using System.Runtime.InteropServices;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;

public class UiController : MonoBehaviour
{
    [SerializeField] private Button _backToMenu;
    [SerializeField] private PlayerController _playerController;

    void Start()
    {
        _backToMenu.onClick.AddListener(BackToMenu);
    }

    void BackToMenu()
    {
        //StatesControllerWolf.onResetData?.Invoke();
       

        _playerController.StopReading();
        SceneManager.LoadScene(0);

    }
}
