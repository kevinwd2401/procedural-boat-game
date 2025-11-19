using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamagable
{
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] Camera mainCamera;
    [SerializeField] Transform targetTransform;
    Ship ship;
    [SerializeField] PlayerTurret[] turrets;
    [SerializeField] PlayerLauncher[] launchers;
    public int Health { get; set;} = 2000;

    float fireDelay, torpedoFireDelay;
    private Plane targetPlane;
    
    private Vector3 camOffset;
    private float camOffsetMultiplier;

    void Awake() {
        ship = GetComponent<Ship>();
        targetPlane = new Plane(Vector3.up, Vector3.zero);
    }

    // Start is called before the first frame update
    void Start()
    {
        camOffset = mainCamera.transform.localPosition;
        camOffsetMultiplier = 1;
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

        if (Input.GetKey(KeyCode.Q)) {
            Transform camtrans = mainCamera.transform;
            camtrans.localEulerAngles = new Vector3(
            Mathf.Clamp(camtrans.localEulerAngles.x + 26 * Time.deltaTime, 53, 70), 
            camtrans.localEulerAngles.y, 
            camtrans.localEulerAngles.z);
        } else if (Input.GetKey(KeyCode.E)) {
            Transform camtrans = mainCamera.transform;
            camtrans.localEulerAngles = new Vector3(
            Mathf.Clamp(camtrans.localEulerAngles.x - 26 * Time.deltaTime, 53, 70), 
            camtrans.localEulerAngles.y, 
            camtrans.localEulerAngles.z);
        }

        float scrollValue = Input.mouseScrollDelta.y;
        if (scrollValue != 0f)
        {
            float step = 0.05f;
            camOffsetMultiplier -= Mathf.Sign(scrollValue) * step;
            camOffsetMultiplier = Mathf.Clamp(camOffsetMultiplier, 0.7f, 1.2f);
            mainCamera.transform.localPosition = camOffset * camOffsetMultiplier;
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

    public void InflictDamage(int dmg, Transform bulletTrans) {
        Vector2 myDir = new Vector2 (transform.forward.x, transform.forward.z);
        Vector2 otherDir = new Vector2 (bulletTrans.forward.x, bulletTrans.forward.z);
        if (Mathf.Abs(Vector2.Dot(myDir.normalized, otherDir.normalized)) > 0.85f ) {
            dmg /= 2;
        }
        Health -= dmg;
        Debug.Log("HP Left: " + Health);
        if (Health <= 0) {
            Destruction();
        }
    }

    private void Destruction() {
        ship.LoseEngines();
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        EnemyManager.Instance.EndGame();

        Debug.Log("dead");
    }
}

