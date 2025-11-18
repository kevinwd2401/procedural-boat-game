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

    private static readonly int maxSizeBulletPool = 100;
    private static readonly int defaultSizeBulletPool = 40;

    private static GameObject bulletPrefab = Resources.Load<GameObject>("Shell");
    public static IObjectPool<Projectile> bulletPool = new ObjectPool<Projectile>(CreateBullet, ActionOnGetBullet, ActionOnReleaseBullet, ActionOnDestroyBullet, true, defaultSizeBulletPool, maxSizeBulletPool);

    private static readonly int maxSizeSplashPool = 100;
    private static readonly int defaultSizeSplashPool = 40;

    private static GameObject splashPrefab = Resources.Load<GameObject>("Splash");
    public static IObjectPool<ParticleSystem> splashPool = new ObjectPool<ParticleSystem>(CreateSplash, ActionOnGetSplash, ActionOnReleaseSplash, ActionOnDestroySplash, true, defaultSizeSplashPool, maxSizeSplashPool);

    private static readonly int maxSizeSparkPool = 60;
    private static readonly int defaultSizeSparkPool = 20;

    private static GameObject sparkPrefab = Resources.Load<GameObject>("Spark");
    public static IObjectPool<ParticleSystem> sparkPool = new ObjectPool<ParticleSystem>(CreateSpark, ActionOnGetSpark, ActionOnReleaseSpark, ActionOnDestroySpark, true, defaultSizeSparkPool, maxSizeSparkPool);


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


    /// Splash
    private static ParticleSystem CreateSplash() {
        GameObject splash = Object.Instantiate(splashPrefab);
        splash.SetActive(false);
        return splash.GetComponent<ParticleSystem>();
    }

    private static void ActionOnGetSplash(ParticleSystem splash) {
        splash.gameObject.SetActive(true);
        splash.gameObject.GetComponent<ReturnToPool>().ReturnPS();
    }

    private static void ActionOnReleaseSplash(ParticleSystem splash) {

        splash.gameObject.SetActive(false);
    }

    private static void ActionOnDestroySplash(ParticleSystem splash) {
        GameObject.Destroy(splash.gameObject);
    }

    /// Spark
    private static ParticleSystem CreateSpark() {
        GameObject spark = Object.Instantiate(sparkPrefab);
        spark.SetActive(false);
        return spark.GetComponent<ParticleSystem>();
    }

    private static void ActionOnGetSpark(ParticleSystem spark) {
        spark.gameObject.SetActive(true);
        spark.gameObject.GetComponent<ReturnToPool>().ReturnPS();
    }

    private static void ActionOnReleaseSpark(ParticleSystem spark) {

        spark.gameObject.SetActive(false);
    }

    private static void ActionOnDestroySpark(ParticleSystem spark) {
        GameObject.Destroy(spark.gameObject);
    }

}
