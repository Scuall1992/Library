using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class BinController : MonoBehaviour
{
    [SerializeField] private Transform leftBin;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip goodMusic;
    [SerializeField] private AudioClip badMusic;
    [Space]
    [SerializeField] private Color _currentColor;
    [SerializeField] private SpriteRenderer _colorTag;
    [SerializeField] private Transform _targetPose;
    [SerializeField] private float _bouncePower = 5f;
    [SerializeField] private float _bounceTime = 1f;
    [SerializeField] private List<Zone> _zones;

    private bool isActive = false;

    public Color Color
    {
        get => _currentColor;
        set
        {
            _currentColor = value;
            _colorTag.color = value;
        }
    }

    public void PrepareToGame(int currentLevel)
    {
        SetLevel(currentLevel);        
    }

    public async void SideMove(Transform tr, Color color)
    {
        await tr.DOJump(_targetPose.position, _bouncePower, 1, _bounceTime).SetEase(Ease.Linear).AsyncWaitForCompletion();
        if (color == _currentColor)
        {
            StatesController.onRightAction?.Invoke();
            _audioSource.clip = goodMusic;
        }
        else
        {
            StatesController.onMissAction?.Invoke();
            _audioSource.clip = badMusic;
        }

        if (GameManager.Instance.Data.playMusic)
        {
            _audioSource.Play();
        }

        InstantiateBallsController.onSetBall?.Invoke(); 
    }

    public async void MiddleMove(Transform tr, Color color)
    {
        await tr.DOMove(_targetPose.position, _bounceTime).SetEase(Ease.Linear).AsyncWaitForCompletion();
        if (color == _currentColor)
        {
            StatesController.onRightAction?.Invoke();
            _audioSource.clip = goodMusic;
        }
        else
        {
            StatesController.onMissAction?.Invoke();
            _audioSource.clip = badMusic;
        }

        if (GameManager.Instance.Data.playMusic)
        {
            _audioSource.Play();
        }
        
        InstantiateBallsController.onSetBall?.Invoke(); 
    }

    public void SetLevel(int level)
    {
        switch (level)
        {
            case <= 2:
                SetAllZoneWide();
                break;
            case >= 6:
                SetOneZone();
                break;

            default:
                SetAllZone();
                break;
        }
    }

    private void SetAllZoneWide()
    {
        var currentZone = _zones[0];
        
        
        var width = Screen.width;
        var height = Screen.height;

        if (width == 1920 && height == 1080)
        {
            currentZone = _zones[5];
        }
        else if (width == 1920 && height == 1200)
        {
            currentZone = _zones[5];
        }
        else if (width == 1280 && height == 1024)
        {
            currentZone = _zones[4];
        }
        else if (width == 1024 && height == 1024)
        {
            currentZone = _zones[2];
        }
        else if (width == 1024 && height == 768)
        {
            currentZone = _zones[3];
        }

        foreach(var zone in _zones)
        {
            if (currentZone == zone)
            {
                zone.CurrentZone.SetActive(true);
            } 
            else
            {
                zone.CurrentZone.SetActive(false);
            }
        }
        currentZone.CurrentZone.SetActive(true);
    }

    private void SetAllZone()
    {
        for (int i = 0; i <_zones.Count; i++)
        {
            _zones[i].CurrentZone.SetActive(false);
        }
        _zones[0].CurrentZone.SetActive(true);
    }

    private void SetOneZone()
    {
        for (int i = 0; i < _zones.Count; i++)
        {
            _zones[i].CurrentZone.SetActive(false);
        }
        _zones[1].CurrentZone.SetActive(true);
    }
}

[System.Serializable]
public class Zone
{
    public GameObject CurrentZone;

}

