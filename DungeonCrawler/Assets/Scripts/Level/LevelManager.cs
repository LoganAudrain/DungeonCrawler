using UnityEngine;

public class LevelManager : MonoBehaviour
{

    public static LevelManager Instance;
    public LevelGenerator generator;

    public int LevelIndex;

    public GameObject Player;
    public GameObject PlayerPrefab;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        generator.GenerateLevel();

        if (Player == null)
            Player = Instantiate(PlayerPrefab);

        generator.MovePlayer(Player);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetSpawnCount()
    {
        return 1;
    }
}
