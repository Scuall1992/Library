using UnityEngine;
using UnityEngine.UI;

public class Exit : MonoBehaviour
{

    [SerializeField] private Button exit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        exit.onClick.AddListener(Exitbutton);
    }

    private void Exitbutton()
    {
        Application.Quit(); 
    }
}
