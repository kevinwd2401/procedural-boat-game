using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{

    public float innerRadius = 300f;
    public float outerRadius = 600f;

    public int spawnCount = 40;

    [SerializeField] private bool playerInRange;

    void Start() {
        StartCoroutine(SpawnCor());
    }

    IEnumerator SpawnCor() {
        while (true) {
            yield return new WaitForSeconds(10 + 4 * Random.value);

            bool prevPlayerInRange = playerInRange;

            playerInRange = Vector3.Distance(EnemyManager.Instance.playerTransform.position, transform.position) < outerRadius;

            if (!playerInRange || prevPlayerInRange) continue;

            for (int i = 0; i < spawnCount; i++)
            {
                Vector2 offset;
                Vector3 spawnPos;


                do
                {
                    offset = Random.insideUnitCircle * outerRadius;
                    spawnPos = transform.position + new Vector3(offset.x, 0, offset.y);
                }
                while (Vector3.Distance(EnemyManager.Instance.playerTransform.position, spawnPos) < innerRadius);

                GameObject ball = BulletPool.ballPool.Get();
                ball.transform.position = spawnPos;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.8f);
        Gizmos.DrawWireSphere(transform.position, outerRadius);
    }

}
