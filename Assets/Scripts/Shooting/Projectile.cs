using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] Transform model;
    private Rigidbody rb;
    [SerializeField] private MeshRenderer mr;
    private MaterialPropertyBlock mpb;

    private int damage;
    private float lifetime;
    private bool dead;

    void Awake() {
        rb = GetComponent<Rigidbody>();
        mpb = new MaterialPropertyBlock();
    }
    // Update is called once per frame
    void Update()
    {
        model.forward = rb.velocity.normalized;

        lifetime += Time.deltaTime;
        if (lifetime > 10) {
            BulletPool.bulletPool.Release(this);
        }
    }

    void FixedUpdate() {
        rb.AddForce(-4f * Vector3.up, ForceMode.Acceleration);
    }

    void OnCollisionEnter(Collision c) {
        if (c.gameObject.TryGetComponent<IDamagable>(out IDamagable d)) {
            d.InflictDamage(damage, model);
        }

        if (c.gameObject.tag == "Water") {
            ParticleSystem splash = BulletPool.splashPool.Get();
            splash.transform.position = transform.position;
            splash.Play();
        } else {
            ParticleSystem spark = BulletPool.sparkPool.Get();
            spark.transform.position = transform.position;
            spark.Play();
        }

        Destruction();
    }

    private void Destruction() {
        if (dead) return;
        dead = true;
        BulletPool.bulletPool.Release(this);
    }

    public void SetValues(int dmg, Vector3 velocity) {
        damage = dmg;
        Color c;
        if (damage < 100) {
            c = new Color(1f, 0.6f, 0.2f);;
        } else if (damage < 160) {
            c = new Color(1f, 0.3f, 0f);
        } else {
            c = new Color(1f, 0.05f, 0f);;
        }

        mpb.SetColor("_EmissionColor", c * 2);
        mr.SetPropertyBlock(mpb);
        rb.velocity = velocity;
    }

    public void ResetValues() {
        lifetime = 0;
        rb.velocity = Vector3.zero;
        dead = false;
    }
}
