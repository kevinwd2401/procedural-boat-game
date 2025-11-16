using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamagable
{
    [SerializeField] Camera mainCamera;
    [SerializeField] Transform targetTransform;
    Ship ship;
    [SerializeField] PlayerTurret[] turrets;
    [SerializeField] PlayerLauncher[] launchers;
    public int Health { get; set;} = 1600;

    float fireDelay, torpedoFireDelay;
    private Plane targetPlane;

    // Start is called before the first frame update
    void Start()
    {
        ship = GetComponent<Ship>();
        targetPlane = new Plane(Vector3.up, Vector3.zero);
    }

    // Update is called once per frame
    void Update()
    {
        //movement
        if (Input.GetKey(KeyCode.A)) {
            ship.Turn(true);
        } else if (Input.GetKey(KeyCode.D)) {
            ship.Turn(false);
        }

        if (Input.GetKey(KeyCode.W)) {
            ship.Accelerate(true);
        } else if (Input.GetKey(KeyCode.S)) {
            ship.Accelerate(false);
        }
        
        //target
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (targetPlane.Raycast(ray, out float distance))
        {
            Vector3 worldPoint = ray.GetPoint(distance);

            worldPoint.y = targetTransform.position.y;
            targetTransform.position = worldPoint;
        }

        //firing
        fireDelay -= Time.deltaTime;
        torpedoFireDelay -= Time.deltaTime;
        if (Input.GetMouseButton(0) && fireDelay < 0.001f) {
            fireDelay = 0.1f;
            for (int i = 0; i < turrets.Length; i++) {
                if (turrets[i].FireGuns()) break;
            }
        }
        if (Input.GetMouseButton(1) && torpedoFireDelay < 0.001f) {
            torpedoFireDelay = 0.25f;
            for (int i = 0; i < launchers.Length; i++) {
                if (launchers[i].FireTorpedos()) break;
            }
        }
    }

    public void InflictDamage(int dmg) {
        Health -= dmg;
        Debug.Log("ouch");
        if (Health <= 0) {
            Destruction();
        }
    }

    private void Destruction() {
        Debug.Log("dead");
    }
}

