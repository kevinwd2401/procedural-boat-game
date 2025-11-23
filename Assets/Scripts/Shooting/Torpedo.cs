using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torpedo : MonoBehaviour
{
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] MeshRenderer mr;
    private Rigidbody rb;
    [SerializeField] bool magnetic = false;

    private int damage;
    private float lifespan, totalLife;
    private Transform playerTransform;
    private Rigidbody playerRB;
    private bool stopHoming;

    // Start is called before the first frame update
    void Start()
    {
        stopHoming = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (magnetic && !stopHoming) {

            Vector3 toTarget = EnemyManager.Instance.playerTransform.position - transform.position;
            if (toTarget.magnitude < 24f) {
                stopHoming = true;
                return;
            }
            toTarget = CalculateTarget(toTarget) - transform.position;
            toTarget.y = 0f;

            Quaternion targetRot = Quaternion.LookRotation(toTarget.normalized, Vector3.up);

            float maxDegrees = 18f * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                maxDegrees
            );

        }

        lifespan -= Time.deltaTime;
        if (lifespan < 0) Destruction();
    }
    void FixedUpdate() {
        if (magnetic) {
            Vector3 nextPos = rb.position + transform.forward * 10 * Time.fixedDeltaTime;
            rb.MovePosition(nextPos);
        }
    }

    void OnCollisionEnter(Collision c) {
        if (c.gameObject.tag == "Water" || totalLife - lifespan < 0.5f) return;
        if (c.gameObject.TryGetComponent<IDamagable>(out IDamagable d)) {
            d.InflictDamage(damage, this.transform);

            if (c.contactCount > 0)
            {
                ParticleSystem spark = BulletPool.sparkPool.Get();
                spark.transform.position = c.GetContact(0).point;
                spark.Play();
            }
            
        }

        Destruction();
    }

    private void Destruction() {
        GameObject vfx = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(vfx, 4);
        Destroy(gameObject);
    }

    public void SetValues(int dmg, float force, float lifespan) {
        rb = GetComponent<Rigidbody>();
        if (!magnetic) {
            rb.velocity = force * transform.forward;
        } else {
            playerRB = EnemyManager.Instance.playerTransform.gameObject.GetComponent<Rigidbody>();
        }
        if (!magnetic && force > 9) {
            Material m = mr.materials[0];
            Color c = m.GetColor("_EmissionColor");
            m.SetColor("_EmissionColor", 3 * c);
        }
        damage = dmg;
        this.lifespan = lifespan + Random.value;
        totalLife = this.lifespan;
    }

    private Vector3 CalculateTarget(Vector3 toTarget) {
        float timeToTarget = toTarget.magnitude / 10;
        return playerRB.position + timeToTarget * playerRB.velocity;
    }
}
