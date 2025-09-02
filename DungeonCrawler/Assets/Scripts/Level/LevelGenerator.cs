using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
//using UnityEngine.WSA;
using static Unity.Collections.AllocatorManager;
using Random = UnityEngine.Random;


public class LevelGenerator : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private bool debug = false;
#endif
    [SerializeField] private bool autoGenerate = false;

    [SerializeField] private LevelSpawnSettings spawnSettings;
    [SerializeField] private LevelGenSettings genSettings;

    [SerializeField] private Tilemap _groundTiles;
    [SerializeField] private Tilemap _wallTiles;
    [SerializeField] private Tilemap _borderTiles;

    [SerializeField] private List<Region> rooms;
    [SerializeField] private List<Region> hallways;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(autoGenerate)
            GenerateLevel();
    }




    public void GenerateLevel()
    {
        ClearLevel();
        spawnSettings.Init();

        GenerateFill();

        Block block = new Block(new Vector2Int(-genSettings.MapSizeX / 2, -genSettings.MapSizeY / 2), new Vector2Int(genSettings.MapSizeX, genSettings.MapSizeY));
        block.depth = genSettings.Splits;
        SplitBlocks(ref block);

        Debug.Log("Generated!");

        BuildLevel();
        
    }

    public void MovePlayer(GameObject player)
    {
        player.transform.position = rooms[Random.Range(0, rooms.Count)].bounds.center;
    }

    private void BuildLevel()
    {
        foreach (Region r in rooms)
        {
            BuildRoom(r, genSettings.FloorTiles[0]);
        }
        foreach (Region r in hallways)
        {
            BuildHallway(r, genSettings.FloorTiles[0]);
        }
    }

    private void ClearLevel()
    {
        _borderTiles.ClearAllTiles();
        _groundTiles.ClearAllTiles();
        _wallTiles.ClearAllTiles();
    }

    #region GenerateLevel

    private void GenerateRoom(ref Block block)
    {
        Vector2Int pos = new Vector2Int(Random.Range(block.bounds.xMin, block.bounds.xMax - genSettings.MinRoomSize - 1), Random.Range(block.bounds.yMin, block.bounds.yMax - genSettings.MinRoomSize - 1));
        Vector2Int size = new Vector2Int(Random.Range(genSettings.MinRoomSize, block.bounds.xMax - pos.x - 1), Random.Range(genSettings.MinRoomSize, block.bounds.yMax - pos.y - 1));

        block.region = new Region(pos, size);

        rooms.Add(block.region);

#if UNITY_EDITOR
        if (debug)
        {
            DrawRect(block.region.bounds, Color.red, 10000);
            DrawBlocks(block, Color.blue, 10000);
        }
#endif
    }

    private Region CreateRegionBetween(Region a, Region b)
    {
        bool xMiss = a.bounds.xMin > b.bounds.xMax - genSettings.HallwaySize + 1 || b.bounds.xMin > a.bounds.xMax - genSettings.HallwaySize + 1;
        bool yMiss = a.bounds.yMin > b.bounds.yMax - genSettings.HallwaySize + 1 || b.bounds.yMin > a.bounds.yMax - genSettings.HallwaySize + 1;

        if (xMiss && yMiss)
        {
            return CreateComplexRegion(a, b);
        }

        int min, max;

        Vector2Int pos;
        Vector2Int size;

        if (xMiss)
        {
            min = Math.Max(a.bounds.yMin, b.bounds.yMin);
            max = Math.Min(a.bounds.yMax, b.bounds.yMax);

            pos = new Vector2Int(Math.Min(a.bounds.xMax, b.bounds.xMax), Random.Range(min, max - genSettings.HallwaySize + 1));
            size = new Vector2Int(Math.Max(a.bounds.xMin, b.bounds.xMin) - pos.x, genSettings.HallwaySize);

        }
        else
        {
            min = Math.Max(a.bounds.xMin, b.bounds.xMin);
            max = Math.Min(a.bounds.xMax, b.bounds.xMax);

            pos = new Vector2Int(Random.Range(min, max - genSettings.HallwaySize + 1), Math.Min(a.bounds.yMax, b.bounds.yMax));
            size = new Vector2Int(genSettings.HallwaySize, Math.Max(a.bounds.yMin, b.bounds.yMin) - pos.y);

        }
        
        return new Region(pos, size);
    }

    private Region CreateComplexRegion(Region a, Region b)
    {
        int xMin, yMin, xMax, yMax;
        bool xMiss = false;

        if (a.bounds.xMin < b.bounds.xMin)
            xMin = a.bounds.xMin;
        else
            xMin = b.bounds.xMin;

        if (a.bounds.yMin < b.bounds.yMin)
            yMin = a.bounds.yMin;
        else
            yMin = b.bounds.yMin;

        if (a.bounds.xMax > b.bounds.xMax)
            xMax = a.bounds.xMax;
        else
            xMax = b.bounds.xMax;

        if (a.bounds.yMax > b.bounds.yMax)
            yMax = a.bounds.yMax;
        else
            yMax = b.bounds.yMax;



        if (a.bounds.xMin > b.bounds.xMax)
        {
            xMax -= a.bounds.width;
            xMin += b.bounds.width;
        }
        else if (b.bounds.xMin > a.bounds.xMax)
        {
            xMax -= b.bounds.width;
            xMin += a.bounds.width;
        }
        else if (a.bounds.yMin > b.bounds.yMax)
        {
            yMax -= a.bounds.height;
            yMin += b.bounds.height;

            xMiss = true;
        }
        else if (b.bounds.yMin > a.bounds.yMax)
        {
            yMax -= b.bounds.height;
            yMin += a.bounds.height;

            xMiss = true;
        }

        Vector2Int pos = new Vector2Int(xMin, yMin);
        Vector2Int size = new Vector2Int(xMax - xMin, yMax - yMin);

        HallRegion hall = new HallRegion(pos, size);

        bool left;

        if (xMiss)
        {
            left = a.bounds.center.y > b.bounds.center.y;

            Vector2Int posA = new();
            Vector2Int sizeA = new();
            Vector2Int posB = new();
            Vector2Int sizeB = new();

            if (left)
            {
                posA.x = Random.Range(a.bounds.xMin, a.bounds.xMax - genSettings.HallwaySize + 1);
                posA.y = (int)hall.bounds.center.y - 1;

                sizeA.x = genSettings.HallwaySize;
                sizeA.y = Math.Abs(a.bounds.yMin - (int)hall.bounds.center.y) + 1;


                posB.x = Random.Range(b.bounds.xMin, b.bounds.xMax - genSettings.HallwaySize + 1);
                posB.y = b.bounds.yMax;

                sizeB.x = genSettings.HallwaySize;
                sizeB.y = Math.Abs(b.bounds.yMin - (int)hall.bounds.center.y) + 1;
            }
            else
            {
                posA.x = Random.Range(b.bounds.xMin, b.bounds.xMax - genSettings.HallwaySize + 1);
                posA.y = (int)hall.bounds.center.y - 1;

                sizeA.x = genSettings.HallwaySize;
                sizeA.y = Math.Abs(b.bounds.yMin - (int)hall.bounds.center.y) + 1;


                posB.x = Random.Range(a.bounds.xMin, a.bounds.xMax - genSettings.HallwaySize + 1);
                posB.y = a.bounds.yMax;

                sizeB.x = genSettings.HallwaySize;
                sizeB.y = Math.Abs(a.bounds.yMax - (int)hall.bounds.center.y) + 1;
            }

            hall.boundA = new RectInt(posA, sizeA);
            hall.boundB = new RectInt(posB, sizeB);

            if (hall.boundA.position.x > hall.boundB.position.x)
                hall.bounds = new RectInt(new Vector2Int(hall.boundB.xMax, (int)hall.bounds.center.y - 1),
                    new Vector2Int(hall.boundA.xMin - hall.boundB.xMax, genSettings.HallwaySize));
            else
                hall.bounds = new RectInt(new Vector2Int(hall.boundA.xMax, (int)hall.bounds.center.y - 1),
                    new Vector2Int(hall.boundB.xMin - hall.boundA.xMax, genSettings.HallwaySize));
        }
        else
        {
            left = a.bounds.center.x > b.bounds.center.x;

            Vector2Int posA = new();
            Vector2Int sizeA = new();
            Vector2Int posB = new();
            Vector2Int sizeB = new();

            if (left)
            {
                posA.x = (int)hall.bounds.center.x - 1;
                posA.y = Random.Range(a.bounds.yMin, a.bounds.yMax - genSettings.HallwaySize + 1);

                sizeA.x = Math.Abs(a.bounds.xMin - (int)hall.bounds.center.x) + 1;
                sizeA.y = genSettings.HallwaySize;


                posB.x = b.bounds.xMax;
                posB.y = Random.Range(b.bounds.yMin, b.bounds.yMax - genSettings.HallwaySize + 1);

                sizeB.x = Math.Abs(b.bounds.xMin - (int)hall.bounds.center.x) + 1;
                sizeB.y = genSettings.HallwaySize;
            }
            else
            {
                posA.x = (int)hall.bounds.center.x - 1;
                posA.y = Random.Range(b.bounds.yMin, b.bounds.yMax - genSettings.HallwaySize + 1);

                sizeA.x = Math.Abs(b.bounds.xMin - (int)hall.bounds.center.x) + 1;
                sizeA.y = genSettings.HallwaySize;


                posB.x = a.bounds.xMax;
                posB.y = Random.Range(a.bounds.yMin, a.bounds.yMax - genSettings.HallwaySize + 1);

                sizeB.x = Math.Abs(a.bounds.xMax - (int)hall.bounds.center.x) + 1;
                sizeB.y = genSettings.HallwaySize;
            }

            hall.boundA = new RectInt(posA, sizeA);
            hall.boundB = new RectInt(posB, sizeB);

            if(hall.boundA.position.y > hall.boundB.position.y)
                hall.bounds = new RectInt(new Vector2Int((int)hall.bounds.center.x - 1, hall.boundB.yMax), 
                    new Vector2Int(genSettings.HallwaySize, hall.boundA.yMin - hall.boundB.yMax));
            else
                hall.bounds = new RectInt(new Vector2Int((int)hall.bounds.center.x - 1, hall.boundA.yMax),
                    new Vector2Int(genSettings.HallwaySize, hall.boundB.yMin - hall.boundA.yMax));
        }

#if UNITY_EDITOR
        Debug.Log("Complex Called!");
        if(debug && left)
            DrawRegion(hall, Color.azure, 10000);
        else
            DrawRegion(hall, Color.purple, 10000);
#endif


        return hall;
    }

    private void GenerateHallway(ref Block block)
    {
        if (block.Children.Count < 2)
            return;
        if (block.depth == 0)
            return;


        (Region a, Region b) regions = FindClosest(block);
        block.region = CreateRegionBetween(regions.a, regions.b);

        hallways.Add(block.region);



#if UNITY_EDITOR
        if (debug)
        {
            DrawRect(block.region.bounds, Color.green, 10000);
        }
#endif
    }

    private (Region, Region) FindClosest(Block block)
    {
        (Region a, Region b) closest = (block.Children[0].region, block.Children[1].region);
        (int x, int y) distance = GetRegionDistance(closest.a, closest.b);

        foreach(Region a in block.Children[0].GetRegions())
        {
            foreach(Region b in block.Children[1].GetRegions())
            {
                (int x, int y) dis = GetRegionDistance(a, b);

                if(dis.x <= distance.x && dis.y < distance.y ||
                   dis.x < distance.x && dis.y <= distance.y ||
                   dis.x + dis.y < distance.x + distance.y ||
                   (dis.x == 0 && dis.y <= distance.x + distance.y + 2) || 
                   (dis.y == 0 && dis.x <= distance.x + distance.y + 2))
                {
                    distance = dis;
                    closest = (a, b);
                }
            }
        }


        return closest;
    }

    private (int x, int y) GetRegionDistance(Region a, Region b)
    {
        (int x, int y) distance = (Math.Abs(a.bounds.xMin - b.bounds.xMin), Math.Abs(a.bounds.yMin - b.bounds.yMin));


        if(a.bounds.xMin > b.bounds.xMin && a.bounds.xMin < b.bounds.xMax ||
           b.bounds.xMin > a.bounds.xMin && b.bounds.xMin < a.bounds.xMax)
        {
            distance.x = 0;
        }

        if(a.bounds.yMin > b.bounds.yMin && a.bounds.yMin < b.bounds.yMax ||
           b.bounds.yMin > a.bounds.yMin && b.bounds.yMin < a.bounds.yMax)
        {
            distance.y = 0;
        }

        if (Math.Abs(a.bounds.xMin - b.bounds.xMax + 1) < distance.x)
            distance.x = Math.Abs(a.bounds.xMin - b.bounds.xMax + 1);
        if (Math.Abs(a.bounds.xMax - b.bounds.xMin + 1) < distance.x)
            distance.x = Math.Abs(a.bounds.xMax - b.bounds.xMin + 1);

        if (Math.Abs(a.bounds.yMin - b.bounds.yMax + 1) < distance.y)
            distance.y = Math.Abs(a.bounds.yMin - b.bounds.yMax + 1);
        if (Math.Abs(a.bounds.yMax - b.bounds.yMin + 1) < distance.y)
            distance.y = Math.Abs(a.bounds.yMax - b.bounds.yMin + 1);

        return distance;
    }




    
    private void SplitBlocks(ref Block block)
    {
        if(block.depth == 0)
        {
            GenerateRoom(ref block);
            return;
        }
        else
        {
            if (Random.Range(0, 2) == 0)
            {
                SplitBlockX(ref block);
            }
            else
            {
                SplitBlockY(ref block);
            }
        }

        GenerateHallway(ref block);
    }

    private void SplitBlockX(ref Block block)
    {
        if (block.bounds.width >= genSettings.MinBlockSize * 2 + 1)
        {
            //Horizontal Split
            int split = Random.Range(genSettings.MinBlockSize + 1, block.bounds.width - genSettings.MinBlockSize);

            Block blockA = new Block(new Vector2Int(block.bounds.xMin, block.bounds.yMin), new Vector2Int(split - 1, block.bounds.height));
            Block blockB = new Block(new Vector2Int(block.bounds.xMin + split, block.bounds.yMin), new Vector2Int(block.bounds.width - split, block.bounds.height));

            blockA.depth = block.depth - 1;
            blockB.depth = block.depth - 1;

            SplitBlocks(ref blockA);
            SplitBlocks(ref blockB);

            block.Children.Add(blockA);
            block.Children.Add(blockB);

            List<Region> regions = blockA.GetRegions();

            if (regions.Count > 0)
                block.subRegions.AddRange(regions);

            regions = blockB.GetRegions();

            if (regions.Count > 0)
                block.subRegions.AddRange(regions);
        }
        else
        {
            if (block.bounds.height >= genSettings.MinBlockSize * 2 + 1)
            {
                SplitBlockY(ref block);
            }
            else
            {
                GenerateRoom(ref block);
            }
        }
    }

    private void SplitBlockY(ref Block block)
    {
        if (block.bounds.height >= genSettings.MinBlockSize * 2 + 1)
        {
            //Vertical Split
            int split = Random.Range(genSettings.MinBlockSize + 1, block.bounds.height - genSettings.MinBlockSize);

            Block blockA = new Block(new Vector2Int(block.bounds.xMin, block.bounds.yMin), new Vector2Int(block.bounds.width, split - 1));
            Block blockB = new Block(new Vector2Int(block.bounds.xMin, block.bounds.yMin + split), new Vector2Int(block.bounds.width, block.bounds.height - split));

            blockA.depth = block.depth - 1;
            blockB.depth = block.depth - 1;

            SplitBlocks(ref blockA);
            SplitBlocks(ref blockB);

            block.Children.Add(blockA);
            block.Children.Add(blockB);

            List<Region> regions = blockA.GetRegions();

            if (regions.Count > 0)
                block.subRegions.AddRange(regions);

            regions = blockB.GetRegions();

            if (regions.Count > 0)
                block.subRegions.AddRange(regions);
        }
        else
        {
            if(block.bounds.width >= genSettings.MinBlockSize * 2 + 1)
            {
                SplitBlockX(ref block);
            }
            else
            {
                GenerateRoom(ref block);
            }
        }
    }
    private void GenerateFill()
    {
        RectInt bounds = new RectInt((-genSettings.MapSizeX / 2) - 4, (-genSettings.MapSizeY / 2) - 4, genSettings.MapSizeX + 8, genSettings.MapSizeY + 8);


        foreach(Vector2Int pos in bounds.allPositionsWithin)
        {
            _wallTiles.SetTile(new Vector3Int(pos.x, pos.y), genSettings.WallTiles[0]);
        }

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            _borderTiles.SetTile(new Vector3Int(x, bounds.yMin, 0), genSettings.VoidTile);
            _borderTiles.SetTile(new Vector3Int(x, bounds.yMin + 1, 0), genSettings.VoidTile);
            _borderTiles.SetTile(new Vector3Int(x, bounds.yMax - 1, 0), genSettings.VoidTile);
        }

        for (int y = bounds.yMin; y < bounds.yMax; y++)
        {
            _borderTiles.SetTile(new Vector3Int(bounds.xMin, y, 0), genSettings.VoidTile);
            _borderTiles.SetTile(new Vector3Int(bounds.xMax - 1, y, 0), genSettings.VoidTile);
        }
    }
    #endregion

    #region BuildLevel
    private void BuildRoom(Region room, TileBase floortile)
    {
        foreach (Vector2Int pos in room.bounds.allPositionsWithin)
        {
            _wallTiles.SetTile((Vector3Int)pos, null);
            _groundTiles.SetTile((Vector3Int)pos, floortile);
        }

        if (spawnSettings == null || spawnSettings.Room_Spawns.Count == 0)
            return;

        int selector = Random.Range(0, spawnSettings.Room_Spawn_Weight + 1);
        int index = 0;

        while(selector > spawnSettings.Room_Spawns[index].weight)
        {
            selector -= spawnSettings.Room_Spawns[index].weight;
            index++;
        }

        spawnSettings.Room_Spawns[index].spawner.Spawn(room);
    }

    private void BuildHallway(Region hallway, TileBase floortile)
    {
        foreach (Vector2Int pos in hallway.bounds.allPositionsWithin)
        {
            _wallTiles.SetTile((Vector3Int)pos, null);
            _groundTiles.SetTile((Vector3Int)pos, floortile);
        }

        if(hallway.GetType() == typeof(HallRegion))
        {
            foreach (Vector2Int pos in ((HallRegion)hallway).boundA.allPositionsWithin)
            {
                _wallTiles.SetTile((Vector3Int)pos, null);
                _groundTiles.SetTile((Vector3Int)pos, floortile);
            }

            foreach (Vector2Int pos in ((HallRegion)hallway).boundB.allPositionsWithin)
            {
                _wallTiles.SetTile((Vector3Int)pos, null);
                _groundTiles.SetTile((Vector3Int)pos, floortile);
            }
        }


        if (spawnSettings == null || spawnSettings.Hall_Spawns.Count == 0)
            return;

        int selector = Random.Range(0, spawnSettings.Hall_Spawn_Weight + 1);
        int index = 0;

        while (selector > spawnSettings.Hall_Spawns[index].weight)
        {
            selector -= spawnSettings.Hall_Spawns[index].weight;
            index++;
        }

        spawnSettings.Hall_Spawns[index].spawner.Spawn(hallway);
    }

    #endregion

#if UNITY_EDITOR
    private void DrawRect(RectInt rect, Color color,  float duration)
    { 
        Debug.DrawLine(new Vector3(rect.xMin, rect.yMin, 1), new Vector3(rect.xMin, rect.yMax, 1), color, duration);
        Debug.DrawLine(new Vector3(rect.xMax, rect.yMin, 1), new Vector3(rect.xMax, rect.yMax, 1), color, duration);
        Debug.DrawLine(new Vector3(rect.xMin, rect.yMin, 1), new Vector3(rect.xMax, rect.yMin, 1), color, duration);
        Debug.DrawLine(new Vector3(rect.xMin, rect.yMax, 1), new Vector3(rect.xMax, rect.yMax, 1), color, duration);
    }

    private void DrawBlocks(Block block, Color color, float duration)
    {
        foreach(Block b in block.Children)
        {
            DrawBlocks(b, color, duration);
        }

        DrawBlock(block, color, duration);
    }

    private void DrawBlock(Block block, Color color, float duration)
    {
        DrawRect(block.bounds, color, duration);
    }

    private void DrawRegion(Region region, Color color, float duration)
    {
        DrawRect(region.bounds, color, duration);
        if (region.GetType() == typeof(HallRegion))
        {
            DrawRect(((HallRegion)region).boundA, color, duration);
            DrawRect(((HallRegion)region).boundB, Color.white, duration);
            
        }
    }
#endif



    private class Block
    {
        public RectInt bounds;
        public List<Block> Children = new List<Block>();
        public Region region;
        public List<Region> subRegions = new List<Region>();
        public int depth = 0;

        public Block(Vector2Int position, Vector2Int size)
        {
            bounds = new RectInt(position, size);
        }

        public Block(int xMin, int yMin, int width, int height)
        {
            bounds = new RectInt(xMin, yMin, width, height);
        }

        public List<Region> GetRegions()
        {
            List<Region> list = new List<Region>();
            if(region != null)
                list.Add(region);
            
            if(subRegions != null)
                list.AddRange(subRegions);

            return list;
        }
    }
}

[Serializable]
public class Region
{
    public RectInt bounds;

    public Region(Vector2Int pos, Vector2Int size)
    {
        bounds = new RectInt(pos, size);
    }
}


[Serializable]
public class HallRegion : Region
{

    public RectInt boundA;
    public RectInt boundB;

    public HallRegion(Vector2Int pos, Vector2Int size) : base(pos, size)
    {
        
    }
}