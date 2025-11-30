using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public enum GameType
{
    Apple,
    Maze,
    Wolf
}

public class LoadRecordsData : BaseBoot
{
    [SerializeField] private GameObject recordObject0;
    [SerializeField] private GameObject recordObject1;
    [SerializeField] private GameObject recordObject2;
    [SerializeField] private Transform content0;
    [SerializeField] private Transform content1;
    [SerializeField] private Transform content2;
    [SerializeField] private GameType type;
    
    private GameManager _manager;
    public async override UniTask Boot()
    {
        await base.Boot();

        if (_manager == null)
            _manager = GameManager.Instance;
    }

    public void UpdateList()
    {

        if (_manager.PlayerData.records == null || _manager.PlayerData.records.Count == 0)
            return;

        foreach (Transform record in content0)
        {
            Destroy(record.gameObject);
        }

        foreach (Transform record in content1)
        {
            Destroy(record.gameObject);
        }

        foreach (Transform record in content2)
        {
            Destroy(record.gameObject);
        }


        foreach (var record in _manager.PlayerData.records)
        {
            switch (record._type)
            {
                case 0:
                    recordObject0.GetComponent<Record>().Instant(record, content0);
                    break;
                case 1:
                    recordObject1.GetComponent<RecordMaze>().Instant(record, content1);
                    break; 
                case 2:
                    recordObject2.GetComponent<RecordWolf>().Instant(record, content2);
                    break;
            }
        }
    }
}
