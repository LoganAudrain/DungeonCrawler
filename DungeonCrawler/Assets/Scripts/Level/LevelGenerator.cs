using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private Tilemap _groundTiles;
    [SerializeField] private Tilemap _wallTiles;

    [SerializeField] private int MapSizeX = 100, MapSizeY = 100;
    [SerializeField] private int RoomSizeX = 8, RoomSizeY = 8;
    [SerializeField] private int MinRoomSize = 4;



    [SerializeField] private List<TileBase> WallTiles;
    [SerializeField] private List<TileBase> FloorTiles;

    [SerializeField] private List<Room> rooms;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateLevel();
    }

    


    public void GenerateLevel()
    {
        rooms = SplitRooms(new Room { bounds = new RectInt(new Vector2Int(-MapSizeX / 2, -MapSizeY / 2), new Vector2Int(MapSizeX / 2 - 1, MapSizeY / 2 - 1)) });
        rooms.AddRange(SplitRooms(new Room { bounds = new RectInt(new Vector2Int(0, -MapSizeY / 2), new Vector2Int(MapSizeX / 2, MapSizeY / 2 - 1)) }));
        rooms.AddRange(SplitRooms(new Room { bounds = new RectInt(new Vector2Int(-MapSizeX / 2, 0), new Vector2Int(MapSizeX / 2 - 1, MapSizeY / 2)) }));
        rooms.AddRange(SplitRooms(new Room { bounds = new RectInt(new Vector2Int(0, 0), new Vector2Int(MapSizeX / 2, MapSizeY / 2)) }));
        //rooms.Add(room);

        //WallTiles.Clear();
        //FloorTiles.Clear();

        foreach (Room r  in rooms)
        {
            DrawRoom(r);
            GenerateBorder(r);
            GenerateFloor(r);
        }

        Debug.Log("Generated!");
    }
    
    private List<Room> SplitRooms(Room room)
    {
        return SplitRoomX(room);
    }

    private List<Room> SplitRoomX(Room room)
    {
        List<Room> result;


        if (room.bounds.width >= RoomSizeX)
        {
            //Horizontal Split
            int split = Random.Range(MinRoomSize, room.bounds.width - MinRoomSize);

            Room roomA = new Room { bounds = new RectInt(new Vector2Int(room.bounds.xMin, room.bounds.yMin), new Vector2Int(split - 1, room.bounds.height)) };
            Room roomB = new Room { bounds = new RectInt(new Vector2Int(room.bounds.xMin + split, room.bounds.yMin), new Vector2Int(room.bounds.width - split, room.bounds.height)) };

            result = SplitRoomY(roomA);
            result.AddRange(SplitRoomY(roomB));
        }
        else
        {
            if(room.bounds.height >= RoomSizeY)
            {
                result = SplitRoomY(room);
            }
            else
            {
                result = new List<Room>();
                result.Add(room);
            }
        }

        return result;
    }

    private List<Room> SplitRoomY(Room room)
    {
        List<Room> result;


        if (room.bounds.height >= RoomSizeY)
        {
            //Vertical Split
            int split = Random.Range(MinRoomSize, room.bounds.height - MinRoomSize);

            Room roomA = new Room { bounds = new RectInt(new Vector2Int(room.bounds.xMin, room.bounds.yMin), new Vector2Int(room.bounds.width, split - 1)) };
            Room roomB = new Room { bounds = new RectInt(new Vector2Int(room.bounds.xMin, room.bounds.yMin + split), new Vector2Int(room.bounds.width, room.bounds.height - split)) };

            result = SplitRoomX(roomA);
            result.AddRange(SplitRoomX(roomB));
        }
        else
        {
            if(room.bounds.width >= RoomSizeX)
            {
                result = SplitRoomX(room);
            }
            else
            {
                result = new List<Room>();
                result.Add(room);
            }
        }


        return result;
    }

    private void GenerateBorder(Room room)
    {
        for (int x = room.bounds.xMin - 1; x < room.bounds.xMax + 1; x++)
        {
            _wallTiles.SetTile(new Vector3Int(x, room.bounds.yMin - 1, 0), WallTiles[0]);
            _wallTiles.SetTile(new Vector3Int(x, room.bounds.yMax, 0), WallTiles[0]);
        }

        for (int y = room.bounds.yMin; y < room.bounds.yMax; y++)
        {
            _wallTiles.SetTile(new Vector3Int(room.bounds.xMin - 1, y, 0), WallTiles[0]);
            _wallTiles.SetTile(new Vector3Int(room.bounds.xMax, y, 0), WallTiles[0]);
        }
    }

    private void GenerateFloor(Room room)
    {
        foreach(var pos in room.bounds.allPositionsWithin)
        {
            _groundTiles.SetTile(new Vector3Int(pos.x, pos.y, 0), FloorTiles[0]);
        }
    }

#if UNITY_EDITOR
    private void DrawRoom(Room room)
    {
        Debug.DrawLine(new Vector3(room.bounds.xMin, room.bounds.yMin, 1), new Vector3(room.bounds.xMin, room.bounds.yMax, 1), Color.blue, 100000);
        Debug.DrawLine(new Vector3(room.bounds.xMax, room.bounds.yMin, 1), new Vector3(room.bounds.xMax, room.bounds.yMax, 1), Color.blue, 100000);
        Debug.DrawLine(new Vector3(room.bounds.xMin, room.bounds.yMin, 1), new Vector3(room.bounds.xMax, room.bounds.yMin, 1), Color.blue, 100000);
        Debug.DrawLine(new Vector3(room.bounds.xMin, room.bounds.yMax, 1), new Vector3(room.bounds.xMax, room.bounds.yMax, 1), Color.blue, 100000);
    }
#endif
}

[Serializable]
public class Room
{
    public RectInt bounds;
}
