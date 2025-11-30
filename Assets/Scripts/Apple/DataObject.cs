using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Data/Save")]
public class DataObject : ScriptableObject
{
    [SerializeField] public int level;
    [SerializeField] public int showLevel;
    [SerializeField] public float timeValue;
    [SerializeField] public float centerTime;
    [SerializeField] public int score;
    [SerializeField] public int marathonScore;

    [SerializeField] public int marathonScoreSum;
    [SerializeField] public int marathonScoreSumMin;
    [SerializeField] public int marathonTime;
    [SerializeField] public float marathonTimeSum;
    [SerializeField] public float offset;
    [SerializeField] public bool isTimeMode;
    [SerializeField] public bool isScoreMode;
    [SerializeField] public bool isMarathonMode;
    [SerializeField] public bool isMarathonModeEnd;
    [SerializeField] public bool playMusic;
    [SerializeField] public float sensitive;

    public DataObject()
    {
        level = 1;
        showLevel = 0;
        timeValue = 120;
        centerTime = 14;
        score = 30;
        marathonScore = 100;
        marathonScoreSum = 0;
        marathonScoreSumMin = 0;
        marathonTime = 90;
        marathonTimeSum = 0;
        offset = 2;
        isTimeMode = true;
        isScoreMode = false;
        isMarathonMode = false;
        isMarathonModeEnd = false;
        playMusic = true;
        sensitive = 1;
    }

}
