using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class BaseMenuUI : MonoBehaviour
{
    [SerializeField] private Button _backToMenu;
    [SerializeField] private PlayerController _playerController;

   /* public IObservable<Unit> OnClick => _backToMenu
       .OnClickAsObservable()
       .ThrottleFirst(TimeSpan.FromMilliseconds(300));*/
}
