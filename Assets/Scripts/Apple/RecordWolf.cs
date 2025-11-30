
using TMPro;
using UnityEngine;

public class RecordWolf : MonoBehaviour, IRecord
{
    [SerializeField] private TextMeshProUGUI text1;
    [SerializeField] private TextMeshProUGUI text3;
    [SerializeField] private TextMeshProUGUI text5;
    [SerializeField] private TextMeshProUGUI text4;


    public void Instant(PlayerRecord playerRecord, Transform content)
    {

        if (playerRecord._type != 2) return;

        text1.text = playerRecord._date.Split(' ')[0];
        
        text3.text = playerRecord._scoreG.ToString();
        text3.text = playerRecord._scoreL.ToString();

        text4.text = $"{playerRecord._timeValue}";
        
        Instantiate(this, content);
    }
}

