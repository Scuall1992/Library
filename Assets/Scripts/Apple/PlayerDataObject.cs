using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "PlayerData/Save")]
public class PlayerDataObject : ScriptableObject
{
    public List<PlayerRecord> records = new List<PlayerRecord>();
}
