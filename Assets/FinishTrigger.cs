using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        TimerMaze.onResetData();
        SceneManager.LoadScene(2);
    }
}
