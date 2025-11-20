using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBox : MonoBehaviour
{
    [SerializeField] GameObject greenSparkPrefab;
    private float timer;
    Transform playerT;
    Rigidbody rb;
    bool used;

    void Start() {
        playerT = EnemyManager.Instance.playerTransform;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (used) return;
        
        Vector3 toPlayer = playerT.position - transform.position;
        float dist = toPlayer.magnitude;
        if (dist <= 25)
        {
            Vector3 dir = toPlayer.normalized;
            float speed = 50 * (1f - dist / 25); 
            rb.AddForce(dir * speed);
        }

        timer += Time.deltaTime;
        if (timer > 30) {
            timer = -10;
            StartCoroutine(SinkCor(8));
        }
    }

    void OnCollisionEnter(Collision c) {
        if (c.gameObject.TryGetComponent<Player>(out Player p)) {
            HealPlayer(p);
            GetComponent<Collider>().enabled = false;
            rb.velocity = Vector3.zero;
            used = true;
        }
    }

    void HealPlayer(Player p) {
        p.Health += 240;
        GameObject spark = Instantiate(greenSparkPrefab, p.transform.position, Quaternion.identity);
        spark.transform.parent = p.transform;
        Destroy(spark, 5);
        StartCoroutine(SinkCor(5));
    }

    private IEnumerator SinkCor(float duration) {
        float originalY = transform.position.y;
        float sinkTimer = 0;
        while (sinkTimer < duration) {
            float vertical = Mathf.SmoothStep(0, -18, Mathf.Pow(sinkTimer / duration, 2f));
            transform.position = new Vector3(transform.position.x, originalY + vertical, transform.position.z);

            sinkTimer += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
