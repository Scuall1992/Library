using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private int _level;
    [SerializeField] private int _showLevel;

    public int GetShowLevel()
    {
        return _showLevel;
    }

    public void LevelStart()
    {
        GameManager.Instance.Data.level = _level;
        GameManager.Instance.Data.showLevel = _showLevel;
        GameManager.Instance.SaveGameDataToJson();
    }
}
