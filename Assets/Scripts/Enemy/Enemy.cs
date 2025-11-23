using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : GroupBehavior, IDamagable
{
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] GameObject smokePrefab;
    [SerializeField] GameObject firePrefab;

    [SerializeField] protected Transform targetTransform;
    [SerializeField] protected Transform torpTargetTransform;
    public List<EnemyTurret> TurretArray = new List<EnemyTurret>();
    public List<EnemyLauncher> LauncherArray = new List<EnemyLauncher>();

    public int Health { get; set; }

    protected GameObject player;
    protected Rigidbody playerRB;
    protected Duck duckTarget;
    protected Ship ship;

    [SerializeField] protected int FiresOnShip;
    [SerializeField] protected bool EngineDamage;

    protected Vector3 Destination;

    protected Vector3 aimOffset;
    protected float aimOffsetLinear;
    protected bool isDead;
    protected bool noIncap;
    
    protected float getDist(Duck duck = null) {
        if (duck != null) {
            return Vector3.Distance(transform.position, duck.transform.position);
        }
        return Vector3.Distance(transform.position, player.transform.position);
    }
    protected bool lineOfSightCheck() {
        if (player == null) {
            return false;
        }
        return !Physics.Raycast(player.transform.position, 
            transform.position - player.transform.position, getDist(), 1 << 7);
    }

    protected IEnumerator ChangeOffset(float offsetRadius) {
        yield return new WaitForSeconds(0.5f);
        while (true) {
            if (FiresOnShip > 0)
                InflictDamage(50 * FiresOnShip, this.transform);

            offsetRadius = Mathf.Lerp(1, offsetRadius, Mathf.Clamp(0, 50, getDist()) / 50);
            Vector2 o = offsetRadius * Random.insideUnitCircle;
            if (playerRB.velocity.magnitude < 2.5f)
                o /= 3;
            aimOffset = new Vector3(o.x, 0, o.y);

            aimOffsetLinear = 0.75f * Random.value + 0.4f;
            yield return new WaitForSeconds(3 + Random.value);
        }
    }

    protected void moveTarget(Rigidbody targetRB, float bulletSpeed) {
        float d = getDist();
        bulletSpeed *= 1.35f;
        float travelTime = (d / bulletSpeed) * (1f + (4 * d) / (2f * bulletSpeed * bulletSpeed));
        Vector3 targetDir = targetRB.velocity * travelTime;
        targetDir.y = 0;
        targetTransform.position = targetRB.position + targetDir + aimOffset;
    }
    protected void moveTargetDuck(float bulletSpeed) {
        float d = getDist(duckTarget);
        bulletSpeed *= 1.35f;
        float travelTime = (d / bulletSpeed) * (1f + (4 * d) / (2f * bulletSpeed * bulletSpeed));
        Vector3 targetDir = duckTarget.GetVelocity() * travelTime;
        targetDir.y = 0;
        targetTransform.position = duckTarget.transform.position + targetDir + aimOffset;
    }
    protected void moveTorpTarget(float torpSpeed) {
        float d = getDist();
        float travelTime = d / torpSpeed;
        Vector3 playerDir = playerRB.velocity * travelTime * aimOffsetLinear * aimOffsetLinear;
        playerDir.y = 0;
        torpTargetTransform.position = player.transform.position + playerDir + aimOffset;
    }

    public void InflictDamage(int dmg, Transform bulletTrans) {
        if (dmg > 300 && dmg < 2000) {
            dmg /= 4;
        }
        if (!noIncap) {
            if (dmg > 1000 || (dmg > 50 && Random.value < 0.03f))
            {
                EngineDamage = true;
                float damageR = Random.value;
                if (damageR > 0.6f) {
                    ship.LoseEngines();
                } else if (damageR > 0.4f) {
                    ship.LoseRudder();
                }

                if (damageR > 0.4f) {
                    FiresOnShip++;
                    //Spawn VFX
                    GameObject smoke = Instantiate(smokePrefab, bulletTrans.position, Quaternion.identity);
                    smoke.transform.parent = transform;
                    smoke.transform.localPosition = 0.7f * smoke.transform.localPosition;
                }
            }
            else if (Health < 2000 && Random.value < 0.08f)
            {
                FiresOnShip++;

                //Spawn VFX
                GameObject fire = Instantiate(firePrefab, bulletTrans.position, Quaternion.identity);
                fire.transform.parent = transform;
                fire.transform.localPosition = 0.7f * fire.transform.localPosition;
            }
        }

        Health -= dmg;
        //Debug.Log("Enemy took " + dmg + ", Remaining: " + Health);
        if (Health <= 0) {
            Destruction();
        }
    }

    private void Destruction() {
        if (isDead) return;
        isDead = true;

        foreach (EnemyTurret t in TurretArray) {
            t.Ready = false;
        }
        foreach (EnemyLauncher l in LauncherArray) {
            l.Ready = false;
        }

        // VFX
        GameObject expl = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(expl, 5);

        ship.LoseEngines();
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().enabled = false;
        StartCoroutine(SinkCor(9));
        
        Destruction2();

        Debug.Log("Enemy Sunk");
    }
    protected virtual void Destruction2() {

    }

    IEnumerator SinkCor(float duration) {
        yield return new WaitForSeconds(1 + Random.value);
        float originalY = transform.position.y;
        float sinkTimer = 0;
        while (sinkTimer < duration) {
            float vertical = Mathf.SmoothStep(0, -20, Mathf.Pow(sinkTimer / duration, 2f));
            transform.position = new Vector3(transform.position.x, originalY + vertical, transform.position.z);

            sinkTimer += Time.deltaTime;
            yield return null;
        }
        Destroy(transform.parent.gameObject);
        
    }
}
