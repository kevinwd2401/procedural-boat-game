using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duck : GroupBehavior, IDamagable, IBoid
{
    [SerializeField] Transform duckModel;
    [SerializeField] GameObject smokePrefab;
    [SerializeField] GameObject healPrefab;
    public int Health { get; set;} = 1000;

    private Vector3 velocity;
    private bool isDead;

    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = 1.8f;

        seekWeight = 0.9f;
        fleeWeight = 1.6f;
        cohesionWeight = 0.7f;
        alignmentWeight = 0.9f;
        separationWeight = 1.6f;

        enemyRadius = 16f;
        neighborRadius = 15f;
        separationRadius = 7f;

        velocity = Random.insideUnitSphere;
        velocity.y = 0f;
        DuckManager.Instance.Ducks.Add((IBoid) this);
        StartCoroutine(SpawnHealsCor());
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;
        Vector3 acceleration = Vector3.zero;

        acceleration += Seek(DuckManager.Instance.TargetPosition) * seekWeight;
        acceleration += Cohesion(DuckManager.Instance.Ducks) * cohesionWeight;
        acceleration += Alignment(DuckManager.Instance.Ducks) * alignmentWeight;
        acceleration += Separation(DuckManager.Instance.Ducks) * separationWeight;

        velocity += acceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, moveSpeed);

        if (velocity.magnitude > 0.001f)
        {
            transform.Translate((velocity + FleeEnemies(DuckManager.Instance.EnemyArray) * fleeWeight) * Time.deltaTime);
            duckModel.rotation = Quaternion.LookRotation(velocity.normalized, Vector3.up);
        }
    }

    public void InflictDamage(int dmg, Transform bulletTrans) {
        Health -= dmg;
        if (Health < 300) {
            GameObject smoke = Instantiate(smokePrefab, bulletTrans.position, Quaternion.identity);
            smoke.transform.parent = transform;
            smoke.transform.localPosition = 0.7f * smoke.transform.localPosition;
        }
        if (Health <= 0) {
            Destruction();
        }
    }

    private IEnumerator SpawnHealsCor() {
        while (!isDead) {
            yield return new WaitForSeconds(25 + 10 * Random.value);
            //spawn heal
            if (Health <= 300) Health = 300;
            else Health += 200;
            
            GameObject heal = Instantiate(healPrefab, transform.position, Quaternion.Euler(0, 360 * Random.value, 0));
        }
    }

    public Vector3 GetVelocity() {
        return velocity;
    }
    public Vector3 GetPosition() {
        return transform.position;
    }

    private void Destruction() {
        if (isDead) return;
        isDead = true;
        
        DuckManager.Instance.DuckDied((IBoid) this);
        GetComponent<Collider>().enabled = false;
        StartCoroutine(SinkCor(7));
        Debug.Log("dead duck");
    }

    IEnumerator SinkCor(float duration) {
        float originalY = transform.position.y;
        float sinkTimer = 0;
        while (sinkTimer < duration) {
            float vertical = Mathf.SmoothStep(0, -16, Mathf.Pow(sinkTimer / duration, 2f));
            transform.position = new Vector3(transform.position.x, originalY + vertical, transform.position.z);

            sinkTimer += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
