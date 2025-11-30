using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class InstantiateBallsController : BaseBoot
{
    [SerializeField] private Transform _pointer;
    [SerializeField] private InterctableObject _obj;
    [SerializeField] private Transform _startPointTransform;
    [SerializeField] private List<Color> _colorList;
    [SerializeField] private Sprite _appleSprite;

    [SerializeField] private List<BinController> _bins;
    
    public static Action onSetBall;
    
    [SerializeField] private Transform mintr;
    [SerializeField] private Transform maxtr;
    [SerializeField] private CanvasScaler scaler;

    [SerializeField] private float coeff;
    
    
    [SerializeField] private Vector2 min = new Vector2(-13f, 2);
    [SerializeField] private Vector2 max = new Vector2(13f, 5.5f);

    public async override UniTask Boot()
    {
        await base.Boot();
        PrepareBin();
        onSetBall += BallConfigure;
    }


    private void OnDestroy()
    {
        onSetBall -= BallConfigure;
    }

    public void PrepareBin()
    {
        var screenCoeff = (float)Screen.width / (float)Screen.height;

        foreach (var bin in _bins)
        {
            var pos = bin.transform.position;
            if (pos.x != 0)
            {
                bin.transform.position = new Vector3(pos.x * screenCoeff, pos.y, pos.z);
            }
        }
    }

    public void PlayLevel()
    {
        StatesController.onSetLevel?.Invoke();

        foreach (var bin in _bins)
        {
            bin.PrepareToGame(GameManager.Instance.Data.level);
        }

        SetApplePosition();
        BallConfigure();
    }

    public void ExitLevel()
    {
        _obj.StopPlay();
        _pointer.position = new(0, 0, 0);
    }


    private async void BallConfigure()
    {

        //_obj.GetComponent<SpriteRenderer>().sprite = _appleSprite;
        Debug.LogError(_startPointTransform.name);
        _startPointTransform.position = new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), 55f);


        _obj.SetColor(_colorList[Random.Range(0, _colorList.Count)]);

        _obj.transform.SetParent(_startPointTransform);
        _obj.transform.position = _startPointTransform.position;

        
        if (GameManager.Instance.Data.level == 5)
        {
            _obj.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        }
        else if (GameManager.Instance.Data.level == 7)
        {
            _obj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        }
        else
        {
            _obj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }

        if (_obj.GetComponent<SpriteRenderer>().color.a == 0)
            await _obj.GetComponent<SpriteRenderer>().DOFade(255f, 0.3f).AsyncWaitForCompletion();

        _obj.Play(GameManager.Instance.Data.level);
    }

    private void SetApplePosition()
    {
        //if(scaler.scaleFactor != 0.7f)
        //{
            min = new(-13f, 2f);
            max = new(13f, 5.5f);
            /*}
            else
            {*/
            //min = new(-8f, -5f);
            //max = new(8f, 5f);
            //}



        }
    }
