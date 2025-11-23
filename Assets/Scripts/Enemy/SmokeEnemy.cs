using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeEnemy : DefaultEnemy
{
    [SerializeField] MeshRenderer lightMR;
    [SerializeField] ParticleSystem smokePS;

    public override void InitializeEnemy(bool solo, bool playerFocus, bool duckFocus) {
        noIncap = true;
        seekWeight = 0.8f;
        cohesionWeight = 0.5f;
        alignmentWeight = 2f;
        separationWeight = 1.8f;
        fleeWeight = 0.7f;
        

        neighborRadius = 50f;
        separationRadius = 15f;
        avoidRange = 16f;

        if (EnemyType == -1) {
            Health = 3600; engageRange = 140;
            avoidRange = 40f;
            TorpBoat = true;
            StartCoroutine(Checker1());
        } else if (EnemyType == -2) {
            Health = 5000; engageRange = 70;
            avoidRange = 20f;
            StartCoroutine(Checker2());
        } else if (EnemyType == -3) {
            Health = 5200; engageRange = 140;
            avoidRange = 40f;
            TorpBoat = true;
            StartCoroutine(Checker3());
        } else if (EnemyType == -4) {
            StartCoroutine(Checker4());
        } else if (EnemyType == -5) {
            StartCoroutine(Checker5());
        }

        FullHealth = Health;

        DuckFocus = duckFocus;
        PlayerFocus = playerFocus;
        Solo = solo;
    }

    private IEnumerator Checker1() {
        while (true) {
            yield return new WaitForSeconds(2);
            if (Health < 2500) {
                StartCoroutine(SmokeCor(30));
                yield break;
            }
        }
    }

    private IEnumerator Checker2() {
        while (true) {
            yield return new WaitForSeconds(2);
            if (Health < 3500) {
                StartCoroutine(SmokeCor(24));
                StartCoroutine(SpeedBoostCor(18, 0.4f));
                yield return new WaitForSeconds(36);
            }
        }
    }

    private IEnumerator Checker3() {
        bool smokeUsed = false;
        while (true) {
            yield return new WaitForSeconds(2);
            if (Health < FullHealth - 400) {
                if (smokeUsed || Random.value > 0.4) {
                    StartCoroutine(TorpReloadBoostCor(14, 0.3f));
                    yield return new WaitForSeconds(26);
                } else {
                    StartCoroutine(SmokeCor(20));
                    smokeUsed = true;
                    yield return new WaitForSeconds(24);
                }
            }
        }
    }

    private IEnumerator Checker4() {
        while (true) {
            yield return new WaitForSeconds(2);
            
        }
    }

    private IEnumerator Checker5() {
        while (true) {
            yield return new WaitForSeconds(2);
            
        }
    }

    protected override void Destruction2() {
        base.Destruction2();
        if (smokePS != null) {
            smokePS.gameObject.transform.parent = null;
            Destroy(smokePS.gameObject, 10);
        }
    }

    private IEnumerator ReloadBoostCor(float duration, float reloadMultiplier) {
        StartCoroutine(lightFlashCor(duration));
        foreach (EnemyTurret t in TurretArray) {
            t.ReloadTime *= reloadMultiplier;
            t.reloadTimer = t.ReloadTime;
        }
        yield return new WaitForSeconds(duration);
        foreach (EnemyTurret t in TurretArray) {
            t.ReloadTime /= reloadMultiplier;
        }
    }

    private IEnumerator TorpReloadBoostCor(float duration, float reloadMultiplier) {
        StartCoroutine(lightFlashCor(duration));
        foreach (EnemyLauncher l in LauncherArray) {
            l.ReloadTime *= reloadMultiplier;
            l.reloadTimer = l.ReloadTime;
        }
        yield return new WaitForSeconds(duration);
        foreach (EnemyLauncher l in LauncherArray) {
            l.ReloadTime /= reloadMultiplier;
        }
    }

    private IEnumerator SpeedBoostCor(float duration, float speedMultiplier) {
        ship.EnemyMultiplyCurrentForce(speedMultiplier);
        yield return new WaitForSeconds(duration);
        ship.EnemyMultiplyCurrentForce(1 / speedMultiplier);
    }

    private IEnumerator SmokeCor(float duration) {
        EnemyManager.Instance.CallThickenFog(duration);
        smokePS.Play();
        var emission = smokePS.emission;
        emission.enabled = true;
        yield return new WaitForSeconds(duration);
        emission.enabled = false;
    }

    private IEnumerator lightFlashCor(float duration) {
        float timer = 0;
        Material m = lightMR.material;
        Color c = m.GetColor("_EmissionColor");

        while (timer < duration) {
            m.SetColor("_EmissionColor", c * (1 + 4 * Mathf.Sin(timer * 5)));
            timer += Time.deltaTime;
            yield return null;
        }
        m.SetColor("_EmissionColor", c);
    }
}
