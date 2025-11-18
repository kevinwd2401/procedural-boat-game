using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BulletPool
{
    private static readonly int maxSizeBallPool = 100;
    private static readonly int defaultSizeBallPool = 20;

    private static GameObject ballPrefab = Resources.Load<GameObject>("PlasticBall");
    public static IObjectPool<GameObject> ballPool = new ObjectPool<GameObject>(CreateBall, ActionOnGetBall, ActionOnReleaseBall, ActionOnDestroyBall, true, defaultSizeBallPool, maxSizeBallPool);

    private static readonly int maxSizeBulletPool = 120;
    private static readonly int defaultSizeBulletPool = 40;

    private static GameObject bulletPrefab = Resources.Load<GameObject>("Shell");
    public static IObjectPool<Projectile> bulletPool = new ObjectPool<Projectile>(CreateBullet, ActionOnGetBullet, ActionOnReleaseBullet, ActionOnDestroyBullet, true, defaultSizeBulletPool, maxSizeBulletPool);

    private static Projectile CreateBullet() {
        GameObject bullet = Object.Instantiate(bulletPrefab);
        bullet.SetActive(false);
        return bullet.GetComponent<Projectile>();
    }

    private static void ActionOnGetBullet(Projectile bullet) {
        bullet.gameObject.SetActive(true);
        bullet.ResetValues();

    }

    private static void ActionOnReleaseBullet(Projectile bullet) {

        bullet.gameObject.SetActive(false);
    }

    private static void ActionOnDestroyBullet(Projectile bullet) {
        GameObject.Destroy(bullet.gameObject);
    }


    
    ///// Balls
    private static GameObject CreateBall() {
        GameObject ball = Object.Instantiate(ballPrefab);
        ball.SetActive(false);
        return ball;
    }

    private static void ActionOnGetBall(GameObject ball) {
        ball.SetActive(true);
        ball.GetComponent<ObjectColor>().Reset();
    }

    private static void ActionOnReleaseBall(GameObject ball) {

        ball.SetActive(false);
    }

    private static void ActionOnDestroyBall(GameObject ball) {
        GameObject.Destroy(ball);
    }
}
