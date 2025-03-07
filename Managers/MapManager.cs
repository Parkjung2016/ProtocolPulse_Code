using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoSingleton<MapManager>
{
    //[SerializeField] private Tilemap _tilemap;
    //private Grid _grid;

    //private void Awake()
    //{
    //    _grid = _tilemap.transform.parent.GetComponent<Grid>();
    //}

    //public bool CanMoveTo(Vector2Int targetCell)
    //{
    //    TileBase targetTile = _tilemap.GetTile((Vector3Int)targetCell);
    //    return targetTile != null;
    //}

    //public Vector2Int GetCellPoint(Vector3 worldPosition)
    //{
    //    return (Vector2Int)_tilemap.WorldToCell(worldPosition);
    //}

    //public Vector3 GetCellCenterWorld(Vector2Int cellPosition)
    //{
    //    return _tilemap.GetCellCenterWorld((Vector3Int)cellPosition);
    //}

    //public Vector2 GetCellSize()
    //{
    //    return _grid.cellSize;
    //}
}