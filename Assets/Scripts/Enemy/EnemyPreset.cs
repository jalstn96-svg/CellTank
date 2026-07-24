using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class EnemyCellPlace
{
    [SerializeField] private string cellPrefab;
    [SerializeField] private Vector2Int position;

    public string CellPrefab => cellPrefab;
    public Vector2Int Position => position;

}

[CreateAssetMenu(
    fileName = "EnemyCellPreset",
    menuName = "Enemy Cell Preset"
    )]
public class EnemyPreset : ScriptableObject
{
    [SerializeField]
    private List<EnemyCellPlace> cells = new List<EnemyCellPlace>();

    public IReadOnlyList<EnemyCellPlace> Cells => cells;

}
