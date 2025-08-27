using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "LevelGenSettings", menuName = "Scriptable Objects/Level/LevelGenSettings")]
public class LevelGenSettings : ScriptableObject
{
    public int MapSizeX = 100, MapSizeY = 100;
    public int MinRoomSize = 6;
    public int HallwaySize = 2;
    public int MinBlockSize = 12;
    public int Splits = 5;

    public List<TileBase> WallTiles;
    public List<TileBase> FloorTiles;
    public TileBase VoidTile;

}
