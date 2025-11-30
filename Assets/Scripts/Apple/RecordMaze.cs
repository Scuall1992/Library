using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class RecordMaze : MonoBehaviour, IRecord
{
    [SerializeField] private TextMeshProUGUI text1;
    [SerializeField] private TextMeshProUGUI text4;
    [SerializeField] private TextMeshProUGUI text2;
    [SerializeField] private TextMeshProUGUI text3;


    public void Instant(PlayerRecord playerRecord, Transform content)
    {

        if (playerRecord._type != 1) return;

        text1.text = playerRecord._date.Split(' ')[0];

        text4.text = $"{playerRecord._timeValue}";
        text2.text = $"{playerRecord._mazeType}";
        text3.text = $"{playerRecord._fit}";
        
        Instantiate(this, content);
    }
}

