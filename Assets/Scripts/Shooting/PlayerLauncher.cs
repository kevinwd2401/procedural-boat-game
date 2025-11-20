using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLauncher : Turret
{
    // Start is called before the first frame update
    void Start()
    {
        torpedosLoaded = firePoints.Length;
        reloadTimer = ReloadTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (torpedosLoaded < firePoints.Length) {
            reloadTimer -= Time.deltaTime;
            if (reloadTimer < 0) {
                torpedosLoaded++;
                reloadTimer = ReloadTime;
            }
            UIManager.Instance.UpdateTorps(torpedosLoaded, reloadTimer, ReloadTime);
        }

        TurnTurret();

        turret.localEulerAngles += cTurretTurn * Time.deltaTime * Vector3.up;
    }
    
    public bool FireTorpedos() {
        return PlayerFireTorpedo(24);
    }
}
