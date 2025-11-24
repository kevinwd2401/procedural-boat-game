using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] FogVariables fv;
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
    private int[] randomNumbers;
    private int currentSpecialIndex;
    private bool currentlySpawning;


    void Awake() {
        Instance = this;
        WaveNumber = 5;
        Kills = 0;
        waveTimer = 10;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentSpecialIndex = 0;
        randomNumbers = Math.randomizeSpecialArray();
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
            StartCoroutine(SpawnWave(4));
        }
    }


    private IEnumerator SpawnWave(float startDelay) {
        if (currentlySpawning) yield break;

        currentlySpawning = true;
        waveTimer = 300;

        if (WaveNumber == 12) {
            yield return new WaitForSeconds(1);
            waveTimer = 10000;
            EndGame(false);

            yield break;
        }

        yield return new WaitForSeconds(startDelay);
        WaveNumber++;
        UIManager.Instance.UpdateWaveNum(WaveNumber);

        SpawnFlagship();

        yield return new WaitForSeconds(0.2f);

        bool switchPosition = false;
        bool spawnSpecial = WaveNumber == 10 || WaveNumber == 12 || (WaveNumber > 5 && Random.value > 0.25f);


        for (int i = 0; i < Mathf.Min(WaveNumber + 2, 5 + Random.Range(0, 5)); i++) {

            bool pFocus = false, dFocus = false;
            float r = Random.value;
            if (r > 0.9f) {
                dFocus = true;
            } else if (r > 0.01f) {
                pFocus = true;
            }

            if (WaveNumber > 3 && i > 1 && !switchPosition && Random.value > 0.9f) {
                switchPosition = true;
                //reset
                yield return new WaitForSeconds(Mathf.Min(i * 16, 35f));
                SelectTargetPoint();
            }

            SpawnRegularShip(Mathf.Min(Random.Range(1, WaveNumber + 1), shipPrefabs.Length - 1), switchPosition, pFocus, dFocus);

            yield return new WaitForSeconds(0.5f);
        }

        if (spawnSpecial) {
            yield return new WaitForSeconds(36 + 10 * Random.value);
            SpawnSpecialShip();
            if (WaveNumber == 10 || WaveNumber == 12) {
                SpawnSpecialShip();
            } else if (Random.value < 0.4f) {
                SpawnRegularShip(1, true, true, false);
                SpawnRegularShip(1, true, true, false);
            }
        }
        currentlySpawning = false;
    }

    private void SelectTargetPoint() {
        int r = Random.Range(0, possibleTargetPoints.Length);
        chosenTargetPoint = playerTransform.position + 480 * (possibleTargetPoints[r].position - playerTransform.position).normalized;
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
        ((DefaultEnemy)Flagship).InitializeEnemy(true, true, false);
        chosenTargetPoint.x += 10;
        Enemies.Add((IBoid) Flagship);
    }

    private void SpawnRegularShip(int index, bool switchedPosition, bool pFocus, bool dFocus) {
        ClearTargetPoint();
        GameObject ship = Instantiate(shipPrefabs[index]);
        ship.transform.GetChild(0).position = chosenTargetPoint;
        DefaultEnemy de = ship.transform.GetChild(0).gameObject.GetComponent<DefaultEnemy>();
        de.InitializeEnemy(switchedPosition || (Random.value < 0.08f), pFocus, dFocus);
        chosenTargetPoint.x += 16;
        Enemies.Add((IBoid) de);
    }

    private void SpawnSpecialShip() {
        currentSpecialIndex++; // get next in random sequence
        if (currentSpecialIndex == blackShipPrefabs.Length) currentSpecialIndex = 0;
        SelectTargetPoint();
        ClearTargetPoint();

        GameObject ship = Instantiate(blackShipPrefabs[randomNumbers[currentSpecialIndex]]);
        ship.transform.GetChild(0).position = chosenTargetPoint;
        DefaultEnemy de = ship.transform.GetChild(0).gameObject.GetComponent<DefaultEnemy>();
        de.InitializeEnemy(true, true, false);
        Enemies.Add((IBoid) de);
        
        chosenTargetPoint.x += 10;

    }


    public void EndGame(bool playerDead) {
        UIManager.Instance.UpdateEnd(!playerDead, WaveNumber);
    }

    public void CallThickenFog(float duration) {
        fv.ThickenFog(duration);
    }
}
