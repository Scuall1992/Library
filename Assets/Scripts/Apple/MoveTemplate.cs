using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class MoveTemplate : MonoBehaviour
{
    [SerializeField] public float _speed;
    [SerializeField] private int _pointCount = 7;
    [SerializeField] private Ease ease;
    [SerializeField] private List<Vector3> _generatedPositions = new List<Vector3>();

    private Queue<Vector3> _targets;
    private Transform _currentTransfomBody;
    private Vector3 _prevTransform;
    private bool _stopped;
    private bool _isLastLevel;

    private Vector2 _areaMin = new Vector2(-10f, 10f);
    private Vector2 _areaMax = new Vector2(10f, -10f);
    private int rightCount = 0;

    public void Command(Transform obj, bool isLastLevel)
    {
        _targets = new Queue<Vector3>();
        _currentTransfomBody = obj;
        _stopped = false;
        _isLastLevel = isLastLevel;

        GenerateRandomPath();
        UpdateQueue();
        SetMoveIter();
    }

    public void GenerateRandomPath()
    {
        _generatedPositions.Clear();

        if (_isLastLevel)
        {
            if (rightCount < StatesController.rightCount)
            {
                _speed +=  0.03f;
                rightCount = StatesController.rightCount;
            }

            _areaMin = new Vector2(-10f, 10f);
            _areaMax = new Vector2(10f, -10f);

            _generatedPositions.Add(new Vector3(
                UnityEngine.Random.Range(_areaMin.x, _areaMax.x),
                _areaMin.y,
                1.0f
            ));


            _generatedPositions.Add(new Vector3(
                    UnityEngine.Random.Range(_areaMin.x, _areaMax.x),
                    _areaMin.x,
                    1.0f
            ));
        }
        else
        {
            _areaMin = new Vector2(-10f, 10f);
            _areaMax = new Vector2(10f, -4f);

            for (int i = 0; i < _pointCount; i++)
            {
                _generatedPositions.Add(new Vector3(
                    UnityEngine.Random.Range(_areaMin.x, _areaMax.x),
                    UnityEngine.Random.Range(_areaMin.y, _areaMax.y),
                    1.0f
                ));

                if (i == _pointCount)
                {
                    _generatedPositions.Add(new Vector3(
                    _generatedPositions[0].x,
                    _generatedPositions[0].y, 1.0f
                ));
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (_generatedPositions == null || _generatedPositions.Count == 0) return;

        Gizmos.color = Color.green;
        for (int i = 1; i < _generatedPositions.Count; i++)
        {
            Gizmos.DrawLine(_generatedPositions[i - 1], _generatedPositions[i]);
        }

        Gizmos.color = Color.red;
        foreach (var pos in _generatedPositions)
        {
            Gizmos.DrawSphere(pos, 0.1f);
        }
    }

    public void Stop()
    {
        _stopped = true;

        _currentTransfomBody.DOKill();
    }

    private void UpdateQueue()
    {
        _generatedPositions.ForEach(position => _targets.Enqueue(position));
    }

    private async UniTask SetMoveIter()
    {
        if (_targets.Count == _generatedPositions.Count)
        {
            var startPos = _targets.Dequeue();
            _currentTransfomBody.position = startPos;
            _prevTransform = startPos;
            if (!_stopped) await SetMoveIter();
            return;
        }

        if (_targets.Count != 0)
        {
            var nextPos = _targets.Dequeue();
            float distance = Vector3.Distance(nextPos, _prevTransform);
            float time;
            time = distance / _speed;

            _prevTransform = nextPos;

            await _currentTransfomBody.DOMove(nextPos, time)
                .SetEase(ease)
                .OnUpdate(() =>
                {
                    if (_stopped)
                    {
                        _currentTransfomBody.DOKill();
                        _currentTransfomBody.localPosition = new(0, 0, 1);
                    }
                })
                .SetUpdate(false)
                .AsyncWaitForCompletion();

            if (!_stopped) await SetMoveIter();
        }
        else
        {
            UpdateQueue();
            if (!_stopped) await SetMoveIter();
        }
    }
}
