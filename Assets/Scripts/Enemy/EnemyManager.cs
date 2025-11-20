using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] Transform[] possibleTargetPoints;
    [SerializeField] int WaveNumber;
    [SerializeField] GameObject[] shipPrefabs;
    [SerializeField] GameObject[] blackShipPrefabs;
    public static EnemyManager Instance;
    public Transform playerTransform;
    public List<IBoid> Enemies = new List<IBoid>();

    public bool FlagShipDestroyed;
    public Enemy Flagship;
    private Vector3 chosenTargetPoint;


    public int Kills;

    private float waveTimer;
    private int currentSpecialIndex;
    private bool currentlySpawning;


    void Awake() {
        Instance = this;
        WaveNumber = 0;
        Kills = 0;
        waveTimer = 10;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnWave(0.1f));
    }

    void Update() {
        waveTimer -= Time.deltaTime;
        if (waveTimer < 0) {
            StartCoroutine(SpawnWave(1));
        }
    }

    public void EnemyDied(DefaultEnemy e) {
        Enemies.Remove((IBoid) e);
        Kills++;
        UIManager.Instance.UpdateKills(Kills);
        if (Enemies.Count <= 0) 
        {
            StartCoroutine(SpawnWave(0.1f));
        }
    }


    private IEnumerator SpawnWave(float startDelay) {
        if (currentlySpawning) yield break;
        currentlySpawning = true;
        waveTimer = 300;

        yield return new WaitForSeconds(startDelay);
        WaveNumber++;
        Debug.Log("Spawning Wave " + WaveNumber);

        SpawnFlagship();

        yield return new WaitForSeconds(0.2f);

        bool switchPosition = false;
        bool spawnSpecial = WaveNumber > 5 && Random.value > 0.7;


        for (int i = 0; i < Mathf.Min(WaveNumber + 2, 5 + Random.Range(0, 5) - (spawnSpecial ? 3 : 0)); i++) {
            //reset target point up to one time, duck focus is rare, solo is random

            bool pFocus = false, dFocus = false;
            float r = Random.value;
            if (r > 0.9f) {
                dFocus = true;
            } else if (r > 0.01f) {
                pFocus = true;
            }

            if (WaveNumber > 3 && !switchPosition && Random.value > 0.9f) {
                switchPosition = true;
                //reset
                SelectTargetPoint();
                yield return new WaitForSeconds(25f);
            }

            ClearTargetPoint();
            GameObject ship = Instantiate(shipPrefabs[Mathf.Min(Random.Range(1, WaveNumber + 1), shipPrefabs.Length - 1)]);
            ship.transform.GetChild(0).position = chosenTargetPoint;
            DefaultEnemy de = ship.transform.GetChild(0).gameObject.GetComponent<DefaultEnemy>();
            de.InitializeEnemy(switchPosition || (Random.value < 0.08f), pFocus, dFocus);
            chosenTargetPoint.x += 10;
            Enemies.Add((IBoid) de);

            yield return new WaitForSeconds(0.2f);
        }

        if (spawnSpecial) {
            //SpawnSpecialShip()
        }
        currentlySpawning = false;
    }

    private void SelectTargetPoint() {
        int r = Random.Range(0, possibleTargetPoints.Length);
        chosenTargetPoint = playerTransform.position + 380 * (possibleTargetPoints[r].position - playerTransform.position).normalized;
        chosenTargetPoint.y = 1;
    }
    private void ClearTargetPoint() {
        Collider[] hits = Physics.OverlapSphere(chosenTargetPoint, 8f, 1 << 11);

        foreach (Collider col in hits)
        {
            Transform t = col.transform;

            Vector3 pos = t.position;
            pos.z += 20f;
            t.position = pos;
        }
    }

    private void SpawnFlagship() {
        //flagship either focuses ducks or player
        SelectTargetPoint();
        ClearTargetPoint();

        FlagShipDestroyed = false;
        GameObject ship = Instantiate(shipPrefabs[0]);
        ship.transform.GetChild(0).position = chosenTargetPoint;
        Flagship = ship.transform.GetChild(0).gameObject.GetComponent<Enemy>();
        ((DefaultEnemy)Flagship).InitializeEnemy(true, Random.value > 0.05f, false);
        chosenTargetPoint.x += 10;
        Enemies.Add((IBoid) Flagship);
    }

    private void SpawnSpecialShip() {
        currentSpecialIndex++;
        if (currentSpecialIndex == blackShipPrefabs.Length) currentSpecialIndex = 0;
        SelectTargetPoint();
        ClearTargetPoint();

        GameObject ship = Instantiate(blackShipPrefabs[currentSpecialIndex]);
        ship.transform.GetChild(0).position = chosenTargetPoint;
        DefaultEnemy de = ship.transform.GetChild(0).gameObject.GetComponent<DefaultEnemy>();
        de.InitializeEnemy(true, true, false);
        Enemies.Add((IBoid) de);
    }

    public void EndGame() {
        Debug.Log("Game Ended");
    }
}
