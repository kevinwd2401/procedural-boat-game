using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultEnemy : Enemy
{
    public float ShootingOffset;
    [SerializeField] int EnemyType;
    [HideInInspector] public int seed3, seed2;

    [Header("States")]
    [SerializeField] bool DuckFocus;

    private float torpSpeed, shellSpeed;

    void Awake() {
        seed3 = Random.Range(0, 3);
        seed2 = Random.Range(0, 2);
        shellSpeed = 20;
        torpSpeed = 20;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ChangeOffset(ShootingOffset));

        player = EnemyManager.Instance.playerTransform.gameObject;
        playerRB = player.GetComponent<Rigidbody>();
        ship = GetComponent<Ship>();

        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart() {
        yield return new WaitForSeconds(0.1f);
        foreach (EnemyTurret t in TurretArray) {
            t.Ready = true;
            shellSpeed = t.bulletSpeed;
        }
        foreach (EnemyLauncher l in LauncherArray) {
            l.Ready = true;
            torpSpeed = l.bulletSpeed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        moveTarget(playerRB, shellSpeed);
        if (torpTargetTransform != null) {
            moveTorpTarget(torpSpeed);
        }
    }

    public void InitializeEnemy() {
        
    }
}
