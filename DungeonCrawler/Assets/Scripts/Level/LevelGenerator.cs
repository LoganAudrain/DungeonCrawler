using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;
using static Unity.Collections.AllocatorManager;
using Random = UnityEngine.Random;


public class LevelGenerator : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private bool debug = false;
#endif

    [SerializeField] private Tilemap _groundTiles;
    [SerializeField] private Tilemap _wallTiles;
    [SerializeField] private Tilemap _borderTiles;

    [SerializeField] private int MapSizeX = 100, MapSizeY = 100;
    [SerializeField] private int MinRoomSize = 6;
    [SerializeField] private int HallwaySize = 2;
    [SerializeField] private int MinBlockSize = 12;
    [SerializeField] private int Splits = 5;



    [SerializeField] private List<TileBase> WallTiles;
    [SerializeField] private List<TileBase> FloorTiles;
    [SerializeField] private TileBase VoidTile;


    [SerializeField] private List<Region> rooms;
    [SerializeField] private List<Region> hallways;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateLevel();
    }




    public void GenerateLevel()
    {
        GenerateFill();

        Block block = new Block(new Vector2Int(-MapSizeX / 2, -MapSizeY / 2), new Vector2Int(MapSizeX, MapSizeY));
        block.depth = Splits;
        SplitBlocks(ref block);



        foreach (Region r in rooms)
        {
            BuildRoom(r, FloorTiles[0]);
        }
        foreach (Region r in hallways)
        {
            BuildHallway(r, FloorTiles[0]);
        }
        //DrawRoom(r);

        Debug.Log("Generated!");
    }

    private void GenerateRoom(ref Block block)
    {
        Vector2Int pos = new Vector2Int(Random.Range(block.bounds.xMin, block.bounds.xMax - MinRoomSize - 1), Random.Range(block.bounds.yMin, block.bounds.yMax - MinRoomSize - 1));
        Vector2Int size = new Vector2Int(Random.Range(MinRoomSize, block.bounds.xMax - pos.x - 1), Random.Range(MinRoomSize, block.bounds.yMax - pos.y - 1));

        block.region = new Region(pos, size);

        BuildRoom(block.region, FloorTiles[0]);

        rooms.Add(block.region);

#if UNITY_EDITOR
        if (debug)
        {
            DrawRect(block.region.bounds, Color.red, 10000);
            DrawBlocks(block, Color.blue, 10000);
        }
#endif
    }

    private void GenerateHallway(ref Block block)
    {
        if (block.Children.Count < 2)
            return;
        if (block.depth == 0)
            return;




        Vector2 Target = FindEdge(block);



        Region regionA = block.Children[0].region;
        Region regionB = block.Children[1].region;

        float distance = Vector2.Distance(Target, regionA.bounds.center);

        foreach(Region r in block.Children[0].subRegions)
        {
            if(Vector2.Distance(Target, regionA.bounds.center) < distance)
            {
                regionA = r;
            }
        }

        distance = Vector2.Distance(Target, regionA.bounds.center);

        foreach (Region r in block.Children[1].subRegions)
        {
            if (Vector2.Distance(Target, regionA.bounds.center) < distance)
            {
                regionB = r;
            }
        }

        block.region = new Region(new Vector2Int((int)Target.x - 1, (int)Target.y - 1), new Vector2Int(2, 2));

        Debug.DrawLine(regionA.bounds.center, regionB.bounds.center, Color.white, 10000);









#if UNITY_EDITOR
        if (debug)
        {
            DrawRect(block.region.bounds, Color.green, 10000);
        }
#endif
    }

    private Vector2 FindEdge(Block block)
    {
        if (block.Children.Count < 2)
            return new Vector2();

        Vector2 BlockACenter = block.Children[0].bounds.center;
        Vector2 BlockBCenter = block.Children[1].bounds.center;

        Vector2 Edge = BlockACenter;

        if(BlockACenter.x != BlockBCenter.x)
        {
            if(BlockACenter.x > BlockBCenter.x)
            {
                Edge.x = block.Children[0].bounds.xMin;
            }
            else
            {
                Edge.x = block.Children[1].bounds.xMin;
            }
        }
        else
        {
            if (BlockACenter.y > BlockBCenter.y)
            {
                Edge.y = block.Children[0].bounds.yMin;
            }
            else
            {
                Edge.y = block.Children[1].bounds.yMin;
            }
        }

        return Edge;
    }

    private void BuildRoom(Region room, TileBase floortile)
    {
        foreach (Vector2Int pos in room.bounds.allPositionsWithin)
        {
            _wallTiles.SetTile((Vector3Int)pos, null);
            _groundTiles.SetTile((Vector3Int)pos, floortile);
        }
    }

    private void BuildHallway(Region hallway, TileBase floortile)
    {

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
        if (block.bounds.width >= MinBlockSize * 2 + 1)
        {
            //Horizontal Split
            int split = Random.Range(MinBlockSize + 1, block.bounds.width - MinBlockSize);

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
            if (block.bounds.height >= MinBlockSize * 2 + 1)
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
        if (block.bounds.height >= MinBlockSize * 2 + 1)
        {
            //Vertical Split
            int split = Random.Range(MinBlockSize + 1, block.bounds.height - MinBlockSize);

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
            if(block.bounds.width >= MinBlockSize * 2 + 1)
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
        RectInt bounds = new RectInt((-MapSizeX / 2) - 4, (-MapSizeY / 2) - 4, MapSizeX + 8, MapSizeY + 8);


        foreach(Vector2Int pos in bounds.allPositionsWithin)
        {
            _wallTiles.SetTile(new Vector3Int(pos.x, pos.y), WallTiles[0]);
        }

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            _borderTiles.SetTile(new Vector3Int(x, bounds.yMin, 0), VoidTile);
            _borderTiles.SetTile(new Vector3Int(x, bounds.yMin + 1, 0), VoidTile);
            _borderTiles.SetTile(new Vector3Int(x, bounds.yMax - 1, 0), VoidTile);
        }

        for (int y = bounds.yMin; y < bounds.yMax; y++)
        {
            _borderTiles.SetTile(new Vector3Int(bounds.xMin, y, 0), VoidTile);
            _borderTiles.SetTile(new Vector3Int(bounds.xMax - 1, y, 0), VoidTile);
        }
    }

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
            DrawRect(((HallRegion)region).boundB, color, duration);
            
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