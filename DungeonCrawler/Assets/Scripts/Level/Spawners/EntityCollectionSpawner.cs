using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "EntityCollectionSpawner", menuName = "Scriptable Objects/Level/EntityCollectionSpawner")]
public class EntityCollectionSpawner : ScriptableObject
{
    [SerializeField] private List<Spawnable> spawnables;
    [SerializeField] private bool OverlapSpawns;
    private int Weights;

    public void Init()
    {
        Weights = 0;

        foreach(Spawnable spawnable in spawnables)
        {
            Weights += spawnable.weight;
        }
    }

    public void Spawn(Region region)
    {
        int spawncount = LevelManager.Instance.GetSpawnCount();

        if (spawnables.Count == 0)
        {
            Debug.LogError("No spawnables in list");
            return;
        }

        for(int i = 0; i < spawncount; i++)
        {
            Vector2Int pos = new(Random.Range(region.bounds.xMin, region.bounds.xMax), Random.Range(region.bounds.yMin, region.bounds.yMax));
            GameObject go = Instantiate(spawnables[Random.Range(0, spawnables.Count)].spawnable);
            go.transform.position = new Vector3(pos.x + 0.5f, pos.y + 0.5f);


        }
        Random.Range(0, spawncount);

    }


    [Serializable]
    public struct Spawnable
    {
        public GameObject spawnable;
        public int weight;
    }
}
