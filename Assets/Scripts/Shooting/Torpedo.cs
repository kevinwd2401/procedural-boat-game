using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torpedo : MonoBehaviour
{
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] MeshRenderer mr;
    private Rigidbody rb;

    private int damage;
    private float lifespan, totalLife;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        lifespan -= Time.deltaTime;
        if (lifespan < 0) Destruction();
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
        rb.velocity = force * transform.forward;
        if (force > 9) {
            Material m = mr.materials[0];
            Color c = m.GetColor("_EmissionColor");
            m.SetColor("_EmissionColor", 3 * c);
        }
        damage = dmg;
        this.lifespan = lifespan + Random.value;
        totalLife = this.lifespan;
    }
}
