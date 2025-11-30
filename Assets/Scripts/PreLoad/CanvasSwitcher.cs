using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CanvasSwitcher : MonoBehaviour
{
    [SerializeField] private RectTransform canvasA;
    [SerializeField] private RectTransform canvasB;
    [SerializeField] private Button one ;
    [SerializeField] private Button two ;
    [SerializeField] private float transitionDuration = 0.5f;
    private float screenWidth;

    private void Start()
    {
        screenWidth = Screen.width;
        one.onClick.AddListener(()=>SwitchCanvas(true));
        two.onClick.AddListener(() => SwitchCanvas(false));
    }

    public void SwitchCanvas(bool f)
    {
        if (f)
        {
            // A -> B
            canvasA.DOAnchorPos(new Vector2(screenWidth, 0), transitionDuration);
            canvasB.anchoredPosition = new Vector2(-screenWidth, 0);
            canvasB.DOAnchorPos(Vector2.zero, transitionDuration);
        }
        else
        {
            // B -> A
            canvasB.DOAnchorPos(new Vector2(-screenWidth, 0), transitionDuration);
            canvasA.anchoredPosition = new Vector2(screenWidth, 0);
            canvasA.DOAnchorPos(Vector2.zero, transitionDuration);
        }
    }
}
