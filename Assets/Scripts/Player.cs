using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamagable
{
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] Camera mainCamera;
    [SerializeField] Camera secondaryCam;
    [SerializeField] Transform targetTransform;
    Ship ship;
    [SerializeField] PlayerTurret[] turrets;
    [SerializeField] PlayerLauncher[] launchers;
    private int health = 2400;
    public int Health
    {
        get => health;
        set {
            health = Mathf.Min(value, 2400);
            UIManager.Instance.UpdateHealth(Health, 2400);
        }
    }

    float fireDelay, torpedoFireDelay;
    private Plane targetPlane;
    private bool isDead;

    private Vector3 secondaryCamOffset;
    private float secondaryCamOffsetMultiplier;
    private bool usingSecondaryCam;
    
    private Vector3 camOffset;
    private float camOffsetMultiplier;

    void Awake() {
        ship = GetComponent<Ship>();
        targetPlane = new Plane(Vector3.up, Vector3.zero);
    }

    // Start is called before the first frame update
    void Start()
    {
        secondaryCamOffset = secondaryCam.transform.localPosition;
        camOffset = mainCamera.transform.localPosition;
        camOffsetMultiplier = 1;
        secondaryCamOffsetMultiplier = 1;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Cursor.visible = !Cursor.visible;
        }

        //movement
        if (Input.GetKey(KeyCode.A)) {
            ship.Turn(true);
        } else if (Input.GetKey(KeyCode.D)) {
            ship.Turn(false);
        }

        if (Input.GetKey(KeyCode.W)) {
            UIManager.Instance.UpdateSpeed(ship.Accelerate(true), 8);
            
        } else if (Input.GetKey(KeyCode.S)) {
            UIManager.Instance.UpdateSpeed(ship.Accelerate(false), 8);
        }

        if (Input.GetKeyDown(KeyCode.Tab)) {
            if (!usingSecondaryCam) {
                mainCamera.enabled = false;
                secondaryCam.enabled = true;
            } else {
                secondaryCam.enabled = false;
                mainCamera.enabled = true;
            }
            usingSecondaryCam = !usingSecondaryCam;
        }

        if (!usingSecondaryCam && Input.GetKey(KeyCode.Q)) {
            Transform camtrans = mainCamera.transform;
            camtrans.localEulerAngles = new Vector3(
            Mathf.Clamp(camtrans.localEulerAngles.x + 30 * Time.deltaTime, 56, 70), 
            camtrans.localEulerAngles.y, 
            camtrans.localEulerAngles.z);
        } else if (!usingSecondaryCam && Input.GetKey(KeyCode.E)) {
            Transform camtrans = mainCamera.transform;
            camtrans.localEulerAngles = new Vector3(
            Mathf.Clamp(camtrans.localEulerAngles.x - 30 * Time.deltaTime, 56, 70), 
            camtrans.localEulerAngles.y, 
            camtrans.localEulerAngles.z);
        }

        float scrollValue = Input.mouseScrollDelta.y;
        if (scrollValue != 0f)
        {
            if (!usingSecondaryCam) {
                float step = 0.05f;
                camOffsetMultiplier -= Mathf.Sign(scrollValue) * step;
                camOffsetMultiplier = Mathf.Clamp(camOffsetMultiplier, 0.7f, 1.3f);
                mainCamera.transform.localPosition = camOffset * camOffsetMultiplier;
            } else {
                float step = 0.05f;
                secondaryCamOffsetMultiplier -= Mathf.Sign(scrollValue) * step;
                secondaryCamOffsetMultiplier = Mathf.Clamp(secondaryCamOffsetMultiplier, 0.3f, 1f);
                secondaryCam.transform.localPosition = secondaryCamOffset * secondaryCamOffsetMultiplier;
            }
        }
        
        //target
        
        Ray ray = usingSecondaryCam ? secondaryCam.ScreenPointToRay(Input.mousePosition) : 
            mainCamera.ScreenPointToRay(Input.mousePosition);

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
        if (health <= 0) {
            Destruction();
        }
    }

    private void Destruction() {
        if (isDead) return;
        isDead = true;

        ship.LoseEngines();
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        EnemyManager.Instance.EndGame(true);
    }
}

