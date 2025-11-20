using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckManager : MonoBehaviour
{
    [SerializeField] Player player;
    public static DuckManager Instance;
    public List<IBoid> Ducks = new List<IBoid>();
    public List<Transform> EnemyArray = new List<Transform>();
    public Vector3 TargetPosition = new Vector3(0, 0, 0);

    void Awake()
    {
        Instance = this;
    }

    void Start() {
        TargetPosition = player.gameObject.transform.position;
        StartCoroutine(SetTargetAssignEnemy());
    }

    private IEnumerator SetTargetAssignEnemy() {
        yield return new WaitForSeconds(1);
        while (true) {
            TargetPosition = player.gameObject.transform.position;

            EnemyArray.Clear();
            Collider[] hits = Physics.OverlapSphere(GetCenter(), 80, (1 << 6) | (1 << 11));

            foreach (Collider hit in hits)
            {
                EnemyArray.Add(hit.transform);
            }
            yield return new WaitForSeconds(8);
        }
    }

    public void DuckDied(IBoid d) {
        Ducks.Remove(d);
        UIManager.Instance.UpdateDucks(Ducks.Count);
    }

    public Vector3 GetCenter() {
        Vector3 center = Vector3.zero;
        int count = 0;
        foreach (IBoid other in Ducks)
        {
            center += other.GetPosition();
            count++;
        }

        if (count == 0) return player.transform.position;
        center /= count;
        return center;
    }
}
