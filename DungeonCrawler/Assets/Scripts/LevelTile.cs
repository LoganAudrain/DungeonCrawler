using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Level Tile", menuName = "2D/Tiles/Level Tile")]
public class LevelTile : Tile
{
    public TileType Type;
}



[Serializable]
public enum TileType
{
    //Ground Tiles
    Stone = 0

    //Object Tiles


}