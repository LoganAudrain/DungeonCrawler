using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelSpawnSettings", menuName = "Scriptable Objects/Level/LevelSpawnSettings")]
public class LevelSpawnSettings : ScriptableObject
{
    [Min(0)] public int Room_Spawn_Min = 3;
    public int Room_Spawn_Max = 7;

    
    public List<Spawner> Room_Spawns;
    public int Room_Spawn_Weight;


    [Min(0)]public int Hall_Spawn_Min = 0;
    public int Hall_Spawn_Max = 3;

    public List<Spawner> Hall_Spawns;
    public int Hall_Spawn_Weight;

    public void Init()
    {
        Room_Spawn_Weight = 0;

        foreach (Spawner s in Room_Spawns)
        {
            Room_Spawn_Weight += s.weight;
        }

        Hall_Spawn_Weight = 0;

        foreach (Spawner s in Hall_Spawns)
        {
            Hall_Spawn_Weight += s.weight;
        }
    }

    [Serializable]
    public struct Spawner
    {
        public EntityCollectionSpawner spawner;
        public int weight;
    }
}
