using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duck : GroupBehavior, IDamagable, IBoid
{
    [SerializeField] Transform duckModel;
    public int Health { get; set;} = 1000;

    private Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = 2.5f;

        seekWeight = 0.3f;
        fleeWeight = 3f;
        cohesionWeight = 1.2f;
        alignmentWeight = 0.4f;
        separationWeight = 1.4f;

        enemyRadius = 16f;
        neighborRadius = 15f;
        separationRadius = 7f;

        velocity = Random.insideUnitSphere;
        velocity.y = 0f;
        DuckManager.Instance.Ducks.Add((IBoid) this);
    }

    // Update is called once per frame
    void Update()
    {
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

    public void InflictDamage(int dmg) {
        Health -= dmg;
        if (Health <= 0) {
            Destruction();
        }
    }

    public Vector3 GetVelocity() {
        return velocity;
    }
    public Vector3 GetPosition() {
        return transform.position;
    }

    private void Destruction() {
        DuckManager.Instance.DuckDied((IBoid) this);
        Destroy(gameObject);
        Debug.Log("dead duck");
    }


}
