using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurret : Turret
{
    float delayTimer;
    public float shotOffset = 0.1f;
    public bool Ready;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        reloadTimer -= Time.deltaTime;

        TurnTurret();

        turret.localEulerAngles += cTurretTurn * Time.deltaTime * Vector3.up;

        if (Ready) {
            delayTimer -= Time.deltaTime;
            if (delayTimer < 0) {
                FireGuns();
                delayTimer = 0.16f;
            }
        }
        
    }
    
    public bool FireGuns() {
        return Fire(true, shotOffset);
    }
}
