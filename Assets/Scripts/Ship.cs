using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public Transform[] forceTransforms;
    public Transform shipModel;

    [SerializeField] private float currentRudder;
    [SerializeField] private float currentForce;
    [Header("Adjustable Settings")]
    public float turnSpeed, maxTurn, turnResistance;
    public float maxSpeed, maxForce, accelSpeed;
    
    private Rigidbody rb;
    private bool loseEngines, loseRudder;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentForce = maxForce;
    }

    // Update is called once per frame
    void Update()
    {
        if (loseEngines) {
            currentForce = Mathf.Max(currentForce - 0.4f * Time.deltaTime, 4f);
        }

        if (loseRudder) return;
        if (currentRudder > 0.01f) {
            currentRudder -= turnSpeed * Time.deltaTime;
        } else if (currentRudder < -0.01f) {
            currentRudder += turnSpeed * Time.deltaTime;
        }
        shipModel.localEulerAngles = new Vector3(0, 0, currentRudder / 2 * Mathf.Clamp01(rb.velocity.magnitude));
        //Debug.Log(rb.velocity.magnitude);
    }

    void FixedUpdate()
    {
        forceTransforms[2].localEulerAngles = new Vector3(0, currentRudder, 0);
        forceTransforms[3].localEulerAngles = new Vector3(0, currentRudder, 0);

        for (int i = 0; i < 4; i++) {
            if (i < 2) {
                Vector3 worldVel = rb.GetPointVelocity(forceTransforms[i].position);
                CalcSteeringForce(forceTransforms[i], worldVel, turnResistance);
            } else {
                Vector3 worldVel = rb.GetPointVelocity(forceTransforms[i].position);
                CalcSteeringForce(forceTransforms[i], worldVel, turnResistance / 2f);
                CalcAccelForce(forceTransforms[i], worldVel);
            }
            //CalcGravityForce(forceTransforms[i]);
        }
        
    }

    public void Turn(bool left) {
        if (!left && currentRudder > -maxTurn) {
            currentRudder -= 2 * turnSpeed * Time.deltaTime;
        } else if (left && currentRudder < maxTurn) {
            currentRudder += 2 * turnSpeed * Time.deltaTime;
        }
    }

    public float Accelerate(bool increase) {
        if (increase) {
            currentForce = Mathf.Min(currentForce + accelSpeed * Time.deltaTime, maxForce);
        } else {
            currentForce = Mathf.Max(currentForce - accelSpeed * Time.deltaTime, -1.5f);
        }
        return currentForce;
    }

    public void LoseRudder() {
        loseRudder = true;
    }

    public void LoseEngines() {
        loseEngines = true;
    }

    private void CalcSteeringForce(Transform forceTransform, Vector3 worldVel, float resistance) {
        Vector3 steeringDir = forceTransform.right;
        float steerVel = Vector3.Dot(worldVel, steeringDir);
        float desiredVelChange = -steerVel * resistance;
        float desiredAccel = desiredVelChange / Time.fixedDeltaTime;

        rb.AddForceAtPosition(desiredAccel * steeringDir, forceTransform.position);
    }

    private void CalcAccelForce(Transform forceTransform, Vector3 worldVel) {
        Vector3 driveDir = forceTransform.forward;
        float shipVel = Vector3.Dot(transform.forward, rb.velocity);
        float normalizedShipVel = shipVel / maxSpeed;
        if (normalizedShipVel < 1) {
            rb.AddForceAtPosition((currentForce * Mathf.Lerp(1f, 0.7f, currentRudder / maxTurn)) 
                * driveDir, forceTransform.position);
        }
    }

    private void CalcGravityForce(Transform forceTransform) {
        rb.AddForceAtPosition(-Vector3.up * 9, forceTransform.position);
    }

    public Vector3 GetVelocity() {
        return rb.velocity;
    }
}
