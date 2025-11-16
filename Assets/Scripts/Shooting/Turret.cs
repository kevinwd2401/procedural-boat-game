using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Turret : MonoBehaviour
{
    [SerializeField] GameObject shellPrefab;
    [SerializeField] protected Transform targetTransform;
    [SerializeField] protected Transform turretBase, turret;
    [SerializeField] protected Transform[] firePoints;
    [Header("Constraints")]
    public bool FrontBlocked;
    public bool BackBlocked;
    [Header("Reload")]
    public float ReloadTime;
    public float reloadTimer;
    [Header("Turning")]
    public float TurretSpeed;
    public float cTurretTurn;
    [Header("Projectile Settings")]
    public int damage;
    public float bulletSpeed;

    protected bool freezeRotation;
    protected int torpedosLoaded;
    

    protected bool CheckAngle(float angle) {
        float dot = Vector3.Dot(turretBase.forward, turret.forward);
        if (FrontBlocked && dot > angle) return false;
        if (BackBlocked && dot < -angle) return false;
        return true;
    }

    protected bool CheckAim() {
        Vector3 TargetDir = (targetTransform.position - turretBase.position);
        TargetDir.y = 0;
        Vector3 turretForward = turret.forward;
        turretForward.y = 0;
        return Vector3.Dot(TargetDir.normalized, turretForward.normalized) > 0.98f;
    }

    protected void TurnTurret() {
        Vector3 TargetDir = targetTransform.position - turretBase.position;
        TargetDir.y = 0;
        float dir = Vector3.Dot(-turret.right, TargetDir.normalized);
        if (dir > 0.02f){
            cTurretTurn = Mathf.Max(-TurretSpeed, cTurretTurn - 900 * Time.deltaTime);
        } else if (dir < -0.02f) {
            cTurretTurn = Mathf.Min(TurretSpeed, cTurretTurn + 900 * Time.deltaTime);
        } else {
            if (Mathf.Abs(cTurretTurn) > 0.001f){
                cTurretTurn /= 1.4f;
            } else {
                cTurretTurn = 0;
            }
        }
    }

    protected bool PlayerFireTorpedo(float lifespan) {
        if (torpedosLoaded < 1 || !CheckAngle(0.7f) || !CheckAim() || getDist() < 1f) return false;
        torpedosLoaded--;

        Vector3 dir = turret.forward;
        dir.y = 0;
        GameObject torpedo = Instantiate(shellPrefab, firePoints[torpedosLoaded].position, Quaternion.identity);
        torpedo.transform.forward = dir.normalized;
        torpedo.GetComponent<Torpedo>().SetValues(damage, bulletSpeed, lifespan);

        return true;
    }

    protected bool EnemyFireTorpedo(float lifespan, float spread) {
        if (reloadTimer > 0.01f || !CheckAngle(0.7f) || !CheckAim() || getDist() < 1f) return false;
        reloadTimer = 1000f;

        StartCoroutine(EnemyTorpSpreadCor(lifespan, spread));
        return true;
    }

    protected IEnumerator EnemyTorpSpreadCor(float lifespan, float spread) {
        freezeRotation = true;
        float increment = spread / (firePoints.Length - 1);

        for (int i = 0; i < firePoints.Length; i++) {
            
            Vector3 dir = turret.forward;
            dir.y = 0;
            Vector3 offsetDir = Quaternion.Euler(0, -spread / 2 + i * increment, 0) * dir;
            GameObject torpedo = Instantiate(shellPrefab, firePoints[i].position, Quaternion.identity);
            torpedo.transform.forward = offsetDir.normalized;
            torpedo.GetComponent<Torpedo>().SetValues(damage, bulletSpeed, lifespan);

            yield return new WaitForSeconds(0.2f);
        }
        
        reloadTimer = ReloadTime;
        freezeRotation = false;
    }

    protected bool Fire(bool offset, float offsetAmount = 0.05f) {
        if (reloadTimer > 0.01f || !CheckAngle(0.8f) || !CheckAim() || getDist() < 3f) return false;
        reloadTimer = ReloadTime;

        if (firePoints.Length > 1) offset = true;

        Vector3 targetPos = targetTransform.position;
        float dist = Vector3.Distance(targetPos, turret.position);

        for (int i = 0; i < firePoints.Length; i++) {
            Vector3 newPos = targetPos;
            if (offset) {
                Vector3 offsetPos = Random.insideUnitSphere * (2 + offsetAmount * Mathf.Max(0, dist - 40));
                offsetPos.y = 0;
                newPos += offsetPos;
            }

            Vector3 bulletDir = Math.CalculateElevation(firePoints[i].position, newPos, 4f, bulletSpeed);
            if (bulletDir.magnitude < 0.5f) return false;

            Projectile shell = BulletPool.bulletPool.Get();
            shell.gameObject.transform.position = firePoints[i].position;
            shell.SetValues(damage, bulletSpeed * bulletDir);
        }
        return true;
    }

    protected float getDist() {
        return Vector3.Distance(turret.position, targetTransform.position);
    }

}
