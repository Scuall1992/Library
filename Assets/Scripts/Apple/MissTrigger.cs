using System;
using DG.Tweening;
using UnityEngine;

public class MissTrigger : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private float _bounceTime = 2f;
    [SerializeField] private Sprite _pufImage;
    
    public async void MissMove(Transform tr, Transform tr1, float speed)
    {
        if (GameManager.Instance.Data.level == 2)
        {
            return;
        }

            Vector3 endPos = new Vector3(tr.position.x, tr.position.y - 2);
        StatesController.onMissAction?.Invoke();

        var distance = Vector3.Distance(tr.position, endPos);


        await tr.GetComponent<SpriteRenderer>().DOFade(0f, 0.1f).AsyncWaitForCompletion();
        
        if (GameManager.Instance.Data.playMusic)
        {
            _audioSource.Play();
        }

        await tr1.GetComponent<SpriteRenderer>().DOFade(255f, 0.3f).AsyncWaitForCompletion();
        await tr1.GetComponent<SpriteRenderer>().DOFade(0f, 0.3f).AsyncWaitForCompletion();


        //if (GameManager.Instance.Data.level == 7)
        //{
        //    await tr.DOMove(endPos, distance / speed).SetEase(Ease.Linear).AsyncWaitForCompletion();
        //}
        //else
        //{
        //    await tr.DOMove(endPos, _bounceTime).SetEase(Ease.Linear).AsyncWaitForCompletion();
        //}

        

        InstantiateBallsController.onSetBall?.Invoke();
    }
}