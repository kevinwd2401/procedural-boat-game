using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeEnemy : DefaultEnemy
{
    [SerializeField] MeshRenderer lightMR;
    [SerializeField] ParticleSystem smokePS;
    AudioSource audioSource;

    public override void InitializeEnemy(bool solo, bool playerFocus, bool duckFocus) {
        audioSource = GetComponent<AudioSource>();

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
            Health = 4000; engageRange = 140;
            avoidRange = 40f;
            TorpBoat = true;
            detectPlayerRange = 200;
            StartCoroutine(Checker1());
        } else if (EnemyType == -2) {
            Health = 5000; engageRange = 70;
            avoidRange = 20f;
            StartCoroutine(Checker2());
        } else if (EnemyType == -3) {
            Health = 7200; engageRange = 140;
            avoidRange = 40f;
            TorpBoat = true;
            detectPlayerRange = 200;
            StartCoroutine(Checker3());
        } else if (EnemyType == -4) {
            Health = 6000; engageRange = 100;
            avoidRange = 20f;
            StartCoroutine(Checker4());
        } else if (EnemyType == -5) {
            Health = 8000; engageRange = 160;
            avoidRange = 60f;
            detectPlayerRange = 200;
            StartCoroutine(Checker5());
        } else if (EnemyType == -6) {
            Health = 4000; engageRange = 120;
            avoidRange = 40;
            detectPlayerRange = 180;
            StartCoroutine(Checker6());
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
                    audioSource.Play();
                    StartCoroutine(TorpReloadBoostCor(14, 0.3f));
                    yield return new WaitForSeconds(24 + 4 * Random.value);
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
            if (Health < FullHealth - 400) {
                if (Random.value < 0.7f) {
                    StartCoroutine(ReloadBoostCor(15, 0.2f));
                    audioSource.Play();
                    yield return new WaitForSeconds(26 + 4 * Random.value);
                } else {
                    StartCoroutine(SpeedBoostCor(18, 1.5f));
                    yield return new WaitForSeconds(20);
                }
            }
        }
    }

    private IEnumerator Checker5() {
        while (true) {
            yield return new WaitForSeconds(2);
            if (getDist() < 50 || Health < FullHealth / 2) {
                StartCoroutine(ReloadBoostCor(12, 0.25f));
                audioSource.Play();
                yield return new WaitForSeconds(20 + 4 * Random.value);
            }
        }
    }

    private IEnumerator Checker6() {
        while (true) {
            yield return new WaitForSeconds(4);
            if (Health < FullHealth && Random.value < 0.25f) {
                StartCoroutine((SpeedBoostCor(12, 1.75f)));
                yield return new WaitForSeconds(20 + 4 * Random.value);
            }
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
            m.SetColor("_EmissionColor", c * (1 + 7 * Mathf.Sin(timer * 7)));
            timer += Time.deltaTime;
            yield return null;
        }
        m.SetColor("_EmissionColor", c);
    }
}
