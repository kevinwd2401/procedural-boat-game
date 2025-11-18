using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToPool : MonoBehaviour
{
    [SerializeField] ParticleSystem ps;
    [SerializeField] bool isSpark;

    public void ReturnPS() {
        StartCoroutine(ReturnPSCor());
    }

    IEnumerator ReturnPSCor() {
        yield return new WaitForSeconds(3f);
        if (isSpark) {
            BulletPool.sparkPool.Release(ps);
        } else {
            BulletPool.splashPool.Release(ps);
        }
    }
}
