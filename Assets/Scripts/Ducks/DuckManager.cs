using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckManager : MonoBehaviour
{
    [SerializeField] Player player;
    public static DuckManager Instance;
    public List<IBoid> Ducks = new List<IBoid>();
    public List<Transform> EnemyArray = new List<Transform>();
    [SerializeField] Transform[] possibleTargetPoints;
    public Vector3 TargetPosition = new Vector3(0, 0, 0);

    private float healTimer = 10;

    void Awake()
    {
        Instance = this;
    }

    void Start() {
        int randomIndex = (int) (Mathf.Clamp01(Random.value - 0.0001f) * possibleTargetPoints.Length);
        TargetPosition = possibleTargetPoints[randomIndex].position;
        StartCoroutine(SetTargetAssignEnemy());
    }

    void Update()
    {
        healTimer -= Time.deltaTime;
        if (healTimer < 0) {
            healTimer = 10;
            if (player.Health <= 1900)
                player.Health += Ducks.Count * 20;
        }
    }

    private IEnumerator SetTargetAssignEnemy() {
        yield return new WaitForSeconds(1);
        while (true) {
            Vector3 center = GetCenter();
            if (Vector3.Distance(TargetPosition, center) < 40) {
                int randomIndex = (int) (Mathf.Clamp01(Random.value - 0.0001f) * possibleTargetPoints.Length);
                TargetPosition = possibleTargetPoints[randomIndex].position;
            }

            EnemyArray.Clear();
            Collider[] hits = Physics.OverlapSphere(center, 80, (1 << 6) | (1 << 11));

            foreach (Collider hit in hits)
            {
                EnemyArray.Add(hit.transform);
            }
            yield return new WaitForSeconds(8);
        }
    }

    public void DuckDied(IBoid d) {
        Ducks.Remove(d);
        if (Ducks.Count <= 0) 
        {
            EnemyManager.Instance.EndGame();
        }
    }

    public Vector3 GetCenter() {
        Vector3 center = Vector3.zero;
        int count = 0;
        foreach (IBoid other in Ducks)
        {
            center += other.GetPosition();
            count++;
        }

        if (count == 0) return Vector3.zero;
        center /= count;
        return center;
    }
}
