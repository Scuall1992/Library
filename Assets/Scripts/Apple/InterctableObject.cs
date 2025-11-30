using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CircleCollider2D))]
public class InterctableObject : MonoBehaviour
{
    [SerializeField] private AudioSource _takeMusic;
    [Space]
    [SerializeField] private MoveTemplate moveTemplate;
    [SerializeField] private Color _currentColor;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private CircleCollider2D _circleCollider;
    [SerializeField] private Transform pufTransform;
    [SerializeField] private List<LvlTemp> lvlTemps = new List<LvlTemp>();
    [Space]
    [SerializeField] private Timer _timer;
    [SerializeField] private Vector3 min = new Vector3(-15f,15f);
    [SerializeField] private Vector3 max = new Vector3(4f,-1f);
    
    private bool timerStart = false;
    private HashSet<int> recentTriggers = new HashSet<int>();

    private void OnValidate()
    {
        if(_circleCollider == null)
            _circleCollider = GetComponent<CircleCollider2D>();
    }

    public void StopPlay()
    {
        if (moveTemplate != null)
        {
            moveTemplate.Stop();
        }
    }


    public void Play(int level)
    {
        timerStart = false;
        StopPlay();
        _circleCollider.enabled = true;
        
        if (level < 5) return;
        
        var newLevel = lvlTemps.Find(x => x.level == level);
        bool isLastLevel = level == 7;

        moveTemplate = newLevel.temp[0];
        moveTemplate.transform.position = new Vector3(
            Random.Range(min.x, max.x),
            Random.Range(min.y, max.y),
            55f);

        moveTemplate.Command(transform, isLastLevel);
    }

    public void SetColor(Color newColor)
    {
        _currentColor = newColor;
        _spriteRenderer.color = newColor;
    }


    private void TriggerGate(Collider2D other) {
        int id = other.GetInstanceID();

        if (recentTriggers.Count > 0) {
            return;
        }

        recentTriggers.Add(id);
        StartCoroutine(RemoveFromRecent());

        if (other.tag == "Side")
        {
            StopPlay();
            _circleCollider.enabled = false;
            other.GetComponentInParent<BinController>().SideMove(transform, _currentColor);
        }
        else if (other.tag == "Mid")
        {
            StopPlay();
            _circleCollider.enabled = false;
            other.GetComponentInParent<BinController>().MiddleMove(transform, _currentColor);
        }
        else if (other.tag == "Miss" && GameManager.Instance.Data.level != 2)
        {
            StopPlay();
            _circleCollider.enabled = false;
            other.GetComponent<MissTrigger>().MissMove(transform, pufTransform, moveTemplate._speed);
        }
    }

    private IEnumerator RemoveFromRecent() {
        yield return new WaitForSeconds(0.5f);
        recentTriggers.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Mouse")
        {
            if (!timerStart)
            {
                _timer.SetTimer();
            }
            transform.SetParent(collision.transform);
            
            if (moveTemplate != null)
                moveTemplate.Stop();
            
            if (GameManager.Instance.Data.playMusic) _takeMusic.Play();
            transform.localPosition = new (0,0,1);
        }
        
        TriggerGate(collision);
    }
}

[System.Serializable]
public struct LvlTemp
{
    public int level;
    public List<MoveTemplate> temp;

}