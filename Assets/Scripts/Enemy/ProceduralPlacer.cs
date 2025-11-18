using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralPlacer : MonoBehaviour
{
    [SerializeField] Enemy enemy;
    [SerializeField] bool isTurret;
    // Start is called before the first frame update
    void Start()
    {
        int r = Random.Range(0, transform.childCount);
        Transform turretType = transform.GetChild(r);
        if (!isTurret) {
            turretType.gameObject.SetActive(true);
            return;
        }

        GameObject turret;
        if (turretType.childCount == 1) {
            turret = turretType.GetChild(0).gameObject;
        } else if (turretType.childCount == 2) {
            turret = turretType.GetChild(((DefaultEnemy) enemy).seed2).gameObject;
        } else {
            turret = turretType.GetChild(((DefaultEnemy) enemy).seed3).gameObject;
        }
        turret.SetActive(true);
        turret.GetComponent<Turret>().AddSelfToTurrets(enemy);
    }
}
