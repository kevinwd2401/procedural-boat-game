using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultEnemy : Enemy
{
    public float ShootingOffset;
    [SerializeField] int EnemyType;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ChangeOffset(ShootingOffset));

        player = EnemyManager.Instance.playerTransform.gameObject;
        playerRB = player.GetComponent<Rigidbody>();
        //ship = GetComponent<Ship>();

        foreach (EnemyTurret t in TurretArray) {
            t.Ready = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        moveTarget(18);
        if (torpTargetTransform != null) {
            moveTorpTarget(6);
        }
    }

    public void InitializeEnemy() {

    }
}
