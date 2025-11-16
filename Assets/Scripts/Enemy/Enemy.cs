using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : GroupBehavior, IDamagable
{

    [SerializeField] protected Transform targetTransform;
    [SerializeField] protected Transform torpTargetTransform;
    public List<EnemyTurret> TurretArray = new List<EnemyTurret>();
    public List<EnemyLauncher> LauncherArray = new List<EnemyLauncher>();

    public int Health { get; set; }

    protected GameObject player;
    protected Rigidbody playerRB;
    protected Ship ship;

    protected float shellSpeed;
    protected Vector3 aimOffset;
    protected float aimOffsetLinear;
    
    protected float getDist() {
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
            offsetRadius = Mathf.Lerp(1, offsetRadius, Mathf.Clamp(0, 50, getDist()) / 50);
            Vector2 o = offsetRadius * Random.insideUnitCircle;
            aimOffset = new Vector3(o.x, 0, o.y);
            aimOffsetLinear = 0.3f * Random.value + 0.7f;
            yield return new WaitForSeconds(3 + Random.value);
        }
    }

    protected void moveTarget(float bulletSpeed) {
        float d = getDist();
        bulletSpeed *= 1.35f;
        float travelTime = (d / bulletSpeed) * (1f + (4 * d) / (2f * bulletSpeed * bulletSpeed));
        Vector3 playerDir = playerRB.velocity * travelTime;
        playerDir.y = 0;
        targetTransform.position = player.transform.position + playerDir + aimOffset;
    }
    protected void moveTorpTarget(float torpSpeed) {
        float d = getDist();
        float travelTime = d / torpSpeed;
        Vector3 playerDir = playerRB.velocity * travelTime * aimOffsetLinear * aimOffsetLinear;
        playerDir.y = 0;
        targetTransform.position = player.transform.position + playerDir + aimOffset;
    }

    public void InflictDamage(int dmg) {
        Health -= dmg;
        if (Health <= 0) {
            Destruction();
        }
    }

    private void Destruction() {
        Debug.Log("OOF");
    }
}
