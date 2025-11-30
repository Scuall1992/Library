using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Record : MonoBehaviour, IRecord
{
    [SerializeField] private TextMeshProUGUI text1;
    [SerializeField] private TextMeshProUGUI timeModeText;
    [SerializeField] private TextMeshProUGUI scoreModeText;
    [SerializeField] private TextMeshProUGUI text3;
    [SerializeField] private TextMeshProUGUI text5;
    [SerializeField] private TextMeshProUGUI text4;
    [SerializeField] private TextMeshProUGUI level;


    public void Instant(PlayerRecord playerRecord, Transform content)
    {
        if (playerRecord._type != 0) return;

        level.text = playerRecord._level;

        text1.text = playerRecord._date.Split(' ')[0];
        
        if (playerRecord._isTimeMode)
        {
            timeModeText.gameObject.SetActive(true);
            scoreModeText.gameObject.SetActive(false);
        }
        else
        {
            scoreModeText.gameObject.SetActive(true);
            timeModeText.gameObject.SetActive(false);
        }
        
        text3.text = playerRecord._scoreG.ToString();
        text5.text = playerRecord._scoreL.ToString();

        text4.text = $"{playerRecord._timeValue}";
        
        Instantiate(this, content);
    }
}

