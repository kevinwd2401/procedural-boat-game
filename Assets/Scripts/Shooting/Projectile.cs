using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] Transform model;
    private Rigidbody rb;

    private int damage;
    private float lifetime;
    private bool dead;

    void Awake() {
        rb = GetComponent<Rigidbody>();
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
            d.InflictDamage(damage);
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
        rb.velocity = velocity;
    }

    public void ResetValues() {
        lifetime = 0;
        rb.velocity = Vector3.zero;
        dead = false;
    }
}
