using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{
    public float sweepSpeed = 16f;
    public float dotThreshold = 0.97f;
    private Vector3 sweepDir;
    private HashSet<IBoid> hitThisHalf = new HashSet<IBoid>();

    private void Start()
    {
        StartCoroutine(RadarSweep());
    }

    private IEnumerator RadarSweep()
    {
        float accumulatedRotation = 0f;
        sweepDir = transform.forward;
        sweepDir.y = 0;

        while (true)
        {
            float delta = sweepSpeed;

            sweepDir = Quaternion.Euler(0f, delta, 0f) * sweepDir;
            sweepDir.Normalize();
            accumulatedRotation += delta;

            ScanList(EnemyManager.Instance.Enemies);
            ScanList(DuckManager.Instance.Ducks);

            if (accumulatedRotation >= 180f)
            {
                hitThisHalf.Clear();
                accumulatedRotation = 0f;
            }
            yield return new WaitForSeconds(0.25f);
        }
    }

    private void ScanList(List<IBoid> boids)
    {
        foreach (var b in boids)
        {
            if (b == null) continue;

            Vector3 toTarget = (b.GetPosition() - transform.position);
            toTarget.y = 0;

            float d = Vector3.Dot(sweepDir, toTarget.normalized);
            if (d >= dotThreshold && toTarget.magnitude < 520)
            {
                if (!hitThisHalf.Contains(b))
                {
                    hitThisHalf.Add(b);

                    MonoBehaviour mb = b as MonoBehaviour;
                    if (mb.gameObject.TryGetComponent(out BlipSpawner blip))
                    {
                        blip.SpawnBlip();
                    }
                }
            }
        }
    }
}

