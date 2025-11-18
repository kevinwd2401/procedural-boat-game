using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] int WaveNumber;
    [SerializeField] GameObject[] shipPrefabs;
    [SerializeField] GameObject[] blackShipPrefabs;
    public static EnemyManager Instance;
    public Transform playerTransform;

    public bool FlagShipDestroyed;
    public Enemy Flagship;

    private int enemyCount;
    public int EnemyCount
    {
        get => enemyCount;
        set
        {
            enemyCount = value;
            Debug.Log("Enemies: " + enemyCount);
            if (enemyCount == 0) {
                StartCoroutine(SpawnWave());
            }
        }
    }
    public int Deaths;


    void Awake() {
        Instance = this;
        WaveNumber = 0;
        Deaths = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnWave());
    }


    private IEnumerator SpawnWave() {
        yield return new WaitForSeconds(3);
        WaveNumber++;

    }
}
