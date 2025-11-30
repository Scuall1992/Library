using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class PanelSwitcher : MonoBehaviour
{
    [SerializeField] private RectTransform canvasA;
    [SerializeField] private RectTransform canvasB;
    [SerializeField] private RectTransform canvasC;
    [SerializeField] private Button one;
    [SerializeField] private Button two1;
    [SerializeField] private Button two2;
    [SerializeField] private Button three;
    [SerializeField] private float transitionDuration = 0.5f;
    private float screenWidth;

    private void Start()
    {
        screenWidth = Screen.width;
        one.onClick.AddListener(() => SwitchCanvas(1,2));
        two1.onClick.AddListener(() => SwitchCanvas(2, 3));
        two2.onClick.AddListener(() => SwitchCanvas(2, 1));
        three.onClick.AddListener(() => SwitchCanvas(3, 2));
    }

    public void SwitchCanvas(int from, int to)
    {
        RectTransform fromCanvas = GetCanvasByIndex(from);
        RectTransform toCanvas = GetCanvasByIndex(to);

        if (fromCanvas == null || toCanvas == null || from == to)
        {
            Debug.LogWarning("Invalid canvas transition.");
            return;
        }

        // Определяем направление: вправо (to > from) или влево (to < from)
        int direction = to > from ? 1 : -1;

        // Убираем текущий экран в сторону
        fromCanvas.DOAnchorPos(new Vector2(-direction * screenWidth, 0), transitionDuration);

        // Сдвигаем целевой экран с другой стороны и двигаем его в центр
        toCanvas.anchoredPosition = new Vector2(direction * screenWidth, 0);
        toCanvas.DOAnchorPos(Vector2.zero, transitionDuration);
    }

    private RectTransform GetCanvasByIndex(int index)
    {
        switch (index)
        {
            case 1: return canvasA;
            case 2: return canvasB;
            case 3: return canvasC;
            default: return null;
        }
    }
}
