using UnityEngine;

public class FitCameraToObjects : MonoBehaviour
{
    public Camera cam;                   // Камера (можно задать через инспектор)
    public GameObject targetGroup;       // Родительский GameObject с дочерними объектами
    public float padding = 1.1f;         // Отступ вокруг объекта (например, 10%)


    public void FitCameraToRenderers()
    {
        Renderer[] renderers = targetGroup.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            Debug.LogWarning("Нет объектов с Renderer внутри targetGroup");
            return;
        }

        // Построить объединённые Bounds
        Bounds bounds = renderers[0].bounds;
        foreach (Renderer rend in renderers)
        {
            bounds.Encapsulate(rend.bounds);
        }

        // Вычисляем нужный orthographicSize
        float verticalSize = bounds.size.y / 2f;
        float horizontalSize = bounds.size.x / 2f / cam.aspect;
        cam.orthographicSize = Mathf.Max(verticalSize, horizontalSize) * padding;
    }
}
