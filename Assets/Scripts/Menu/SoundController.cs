using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SoundController : BaseBoot
{
    [SerializeField] private AudioSource _music;
    [SerializeField] private Image _soundImage;
    [SerializeField] private List<Sprite> _sprite;
    [SerializeField] private Slider _slider;
    
    private Button _soundButton;
    private GameManager _manager;

    public async override UniTask Boot()
    {
        await base.Boot();
        StartSound();
    }

    private void ChangeSlider()
    {
        AudioListener.volume = _slider.value;
    }

    void StartSound()
    {
        if (_manager == null)
            _manager = GameManager.Instance;
        
        _soundButton = GetComponent<Button>();
        _soundButton.onClick.AddListener(ChangeState);
        _slider.onValueChanged.AddListener(delegate {ChangeSlider();});
        
        UpdateState();
    }

    private void ChangeState()
    {
        _manager.Data.playMusic = !_manager.Data.playMusic;
        UpdateState();
    }

    private void UpdateState()
    {
        bool state = _manager.Data.playMusic;
        Action onCall;
        
        onCall = state ? _music.Play : _music.Pause;
        _soundImage.sprite = state ? _sprite[0] : _sprite[1];
        onCall?.Invoke();
    }
}
