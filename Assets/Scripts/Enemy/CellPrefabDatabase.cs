using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class CellPrefabValue
{
    [SerializeField] private string cellId;
    [SerializeField] private TankCell cellPrefab;

    //get
    public string CellId => cellId;
    public TankCell CellPrefab => cellPrefab;

}

[CreateAssetMenu(
    fileName = "CellPrefab",
    menuName = "Cell Prefab"
    )]


public class CellPrefabDatabase : ScriptableObject
{
    [SerializeField] private List<CellPrefabValue> values = new List<CellPrefabValue>();
    private Dictionary<string, TankCell> prefabDictionary;

    private void OnEnable()
    {
        prefabDictionary = null;
    }

    private void Initialize()
    {
        if(prefabDictionary != null)
        {
            return;
        }
        prefabDictionary = new Dictionary<string, TankCell>();

        foreach (CellPrefabValue value in values)
        {
            if (value.CellId == null)
            {
                Debug.Log("empty CellId");
                continue;
            }
            if(value.CellPrefab == null)
            {
                Debug.Log("empty CellPrefab");
                continue;
            }
            if (prefabDictionary.ContainsKey(value.CellId) == true)
            {
                Debug.Log("중복 CellId");
                continue;
            }

            prefabDictionary.Add(value.CellId, value.CellPrefab);
        }
        

    }

    public bool TryGetPrefab(string cellId, out TankCell prefab)
    {
        Initialize();
        return prefabDictionary.TryGetValue(cellId, out prefab);
    }

}
