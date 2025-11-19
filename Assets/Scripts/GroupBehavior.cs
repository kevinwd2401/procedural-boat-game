using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GroupBehavior : MonoBehaviour
{
    protected float moveSpeed;

    protected float seekWeight;
    protected float fleeWeight;
    protected float cohesionWeight;
    protected float alignmentWeight;
    protected float separationWeight;

    protected float enemyRadius;
    protected float neighborRadius;
    protected float separationRadius;

    protected Vector3 Seek(Vector3 target)
    {
        Vector3 desired = (target - transform.position);
        desired.y = 0f;
        return desired.normalized * Mathf.Clamp01(desired.magnitude);
    }

    protected Vector3 FleeEnemies(List<Transform> t)
    {
        Vector3 flee = Vector3.zero;

        foreach (Transform enemy in t)
        {
            if (enemy == null) continue;
            Vector3 diff = transform.position - enemy.position;
            diff.y = 0f;
            float dist = diff.magnitude;

            if (dist < enemyRadius)
                flee += diff.normalized / Mathf.Max(dist, 0.001f);
        }

        return flee.normalized;
    }

    protected Vector3 Cohesion(List<IBoid> l)
    {
        Vector3 center = Vector3.zero;
        int count = 0;

        foreach (IBoid other in l)
        {
            if (other == this) continue;
            Vector3 diff = transform.position - other.GetPosition();
            diff.y = 0f;

            float dist = diff.magnitude;
            if (dist < neighborRadius && dist > 0f)
            {
                center += other.GetPosition();
                count++;
            }
        }

        if (count == 0) return Vector3.zero;
        center /= count;

        return (center - transform.position).normalized;
    }

    protected Vector3 Alignment(List<IBoid> l)
    {
        Vector3 avgVelocity = Vector3.zero;
        int count = 0;

        foreach (IBoid other in l)
        {
            if (other == this) continue;
            Vector3 diff = transform.position - other.GetPosition();
            diff.y = 0f;

            float dist = diff.magnitude;
            if (dist < neighborRadius && dist > 0f)
            {
                avgVelocity += other.GetVelocity();
                count++;
            }
        }

        if (count == 0) return Vector3.zero;

        avgVelocity /= count;
        avgVelocity.y = 0f;

        return avgVelocity.normalized;
    }

    protected Vector3 Separation(List<IBoid> l)
    {
        Vector3 repel = Vector3.zero;
        int count = 0;

        foreach (IBoid other in l)
        {
            if (other == this) continue;
            Vector3 diff = transform.position - other.GetPosition();
            diff.y = 0f;

            float dist = diff.magnitude;
            if (dist < separationRadius && dist > 0f)
            {
                repel += diff.normalized / dist;
                count++;
            }
        }

        if (count == 0) return Vector3.zero;
        return repel.normalized;
    }
}
