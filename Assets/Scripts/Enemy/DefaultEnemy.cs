using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultEnemy : Enemy, IBoid
{
    public float ShootingOffset;
    [SerializeField] protected int EnemyType;
    [HideInInspector] public int seed3, seed2;

    [Header("States")]
    [SerializeField] protected bool PlayerDetected;
    [SerializeField] protected bool DuckDetected;
    [SerializeField] protected bool DuckFocus;
    [SerializeField] protected bool PlayerFocus;
    [SerializeField] protected bool TorpBoat;
    [SerializeField] protected bool Solo;

    private float torpSpeed, shellSpeed;
    protected float avoidRange, engageRange;
    private Vector3 storedDest;
    protected int FullHealth;
    private List<Transform> obstacles;


    void Awake() {
        seed3 = Random.Range(0, 3);
        seed2 = Random.Range(0, 2);
        shellSpeed = 20;
        torpSpeed = 20;
        obstacles = new List<Transform>();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        StartCoroutine(ChangeOffset(ShootingOffset));

        player = EnemyManager.Instance.playerTransform.gameObject;
        playerRB = player.GetComponent<Rigidbody>();
        ship = GetComponent<Ship>();

        StartCoroutine(DelayedStart());
        StartCoroutine(StateUpdater());
    }

    protected IEnumerator DelayedStart() {
        yield return new WaitForSeconds(0.1f);
        foreach (EnemyTurret t in TurretArray) {
            shellSpeed = t.bulletSpeed;
        }
        foreach (EnemyLauncher l in LauncherArray) {
            torpSpeed = l.bulletSpeed;
        }
    }

    protected IEnumerator StateUpdater() {
        while (true) {
            if (isDead) yield break;

            //checks
            PlayerDetected = getDist() < (140 + 20 * Random.value);
            UpdateDuckDetected();

            // lock/unlock turrets
            MoveTargets();
            foreach (EnemyTurret t in TurretArray) {
                t.Ready = (!TorpBoat && (PlayerDetected || DuckDetected)) || 
                    (TorpBoat && (PlayerDetected || DuckDetected) && Health < FullHealth - 300);
            }
            foreach (EnemyLauncher l in LauncherArray) {
                l.Ready = PlayerDetected;
            }

            yield return null;
            //update boid variables, Destination
            PopulateObstacles();

            if (!Solo && (EnemyType == 0 || EnemyManager.Instance.FlagShipDestroyed)) {
                Solo = true;
                cohesionWeight = 0.2f;
                alignmentWeight = 0f;
            }

            if (Solo) {
                // use own target
                Destination = GetOwnDestination();

            } else {
                Destination = ((DefaultEnemy) EnemyManager.Instance.Flagship).Destination;
            }
            
            yield return new WaitForSeconds(1.5f + Random.value);
        }
    }

    private void UpdateDuckDetected()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 120, 1 << 8);

        if (hits.Length > 0)
        {
            DuckDetected = true;
            duckTarget = hits[0].gameObject.GetComponent<Duck>();
        }
        else
        {
            DuckDetected = false;
            duckTarget = null;
        }
    }

    private Vector3 GetOwnDestination() {
        if ( DuckFocus || (DuckDetected && !PlayerDetected)) {
            Vector3 v = DuckManager.Instance.GetCenter() + 20 * Random.insideUnitSphere;
            v.y = transform.position.y;
            return v;

        } else if (PlayerDetected || PlayerFocus) {

            float d = getDist();
            Vector3 v;
            if (d > engageRange) {
                v = playerRB.position + 20 * Random.insideUnitSphere;
            } else if (d < avoidRange) {
                v = playerRB.position + (avoidRange + 10) * (transform.position - playerRB.position).normalized;
            } else {
                // engangement zone
                
                //recheck stored Destination
                float storedDist = Vector3.Distance(storedDest, playerRB.position);
                if (storedDist > avoidRange && storedDist < engageRange && Vector3.Distance(storedDest, transform.position) > 16) {
                    v = storedDest;
                } else if (Random.value > 0.5f) {
                    // perpendicular
                    Vector3 dirToPlayer = (playerRB.position - transform.position).normalized;
                    float sign = (Random.value > 0.5f) ? 1 : -1;
                    Vector3 cross = Vector3.Cross(dirToPlayer, sign * Vector3.up);
                    v = transform.position + 20 * cross + 5 * dirToPlayer;
                } else {
                    //random
                    Vector2 r2 = (engageRange - 40) * Random.insideUnitCircle;
                    v = playerRB.position;
                    v.x += r2.x;
                    v.y += r2.y;
                }
            }

            v.y = transform.position.y;
            storedDest = v;
            return v;

        } else {
            return DuckManager.Instance.GetCenter();
        }
    }

    private void PopulateObstacles()
    {
        obstacles.Clear();

        Collider[] hits = Physics.OverlapSphere(transform.position, 25f, 1 << 11);
        foreach (Collider col in hits)
        {
            obstacles.Add(col.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;

        if (DuckDetected) {
            if (duckTarget == null) {
                DuckDetected = false;
            }
        }

        //aiming
        MoveTargets();

        //movement
        Vector3 Direction = Vector3.zero;
        Direction += Seek(Destination) * seekWeight;
        Direction += Cohesion(EnemyManager.Instance.Enemies) * cohesionWeight;
        Direction += Alignment(EnemyManager.Instance.Enemies) * alignmentWeight;
        Direction += Separation(EnemyManager.Instance.Enemies) * separationWeight;
        Direction += FleeEnemies(obstacles) * fleeWeight;

        Direction.y = 0;
        float dir = Vector3.Dot(-transform.right, Direction.normalized);
        if (dir > 0.02f){
            //left
            ship.Turn(true);
        } else if (dir < -0.02f) {
            //right
            ship.Turn(false);
        }
    }

    protected void MoveTargets() {
        if (DuckDetected && (DuckFocus || !PlayerDetected)) {
            moveTargetDuck(shellSpeed);
        } else if (PlayerDetected) {
            moveTarget(playerRB, shellSpeed);
            if (torpTargetTransform != null) {
                moveTorpTarget(torpSpeed);
            }
        }
    }

    public virtual void InitializeEnemy(bool solo, bool playerFocus, bool duckFocus) {
        seekWeight = 0.8f;
        cohesionWeight = 0.5f;
        alignmentWeight = 2f;
        separationWeight = 1.8f;
        fleeWeight = 0.7f;

        neighborRadius = 50f;
        separationRadius = 15f;
        avoidRange = 16f;

        if (EnemyType == 0) { Health = 2400; engageRange = 110;}
        else if (EnemyType == 1) { Health = 1200 + Random.Range(0, 601); engageRange = 80;}
        else if (EnemyType == 2) { Health = 1200 + Random.Range(0, 801); engageRange = 140; TorpBoat = true;}
        else if (EnemyType == 3) { Health = 2800 + Random.Range(0, 801); engageRange = 100;}
        else if (EnemyType == 4) { Health = 4200 + Random.Range(0, 1201); engageRange = 120;}
        FullHealth = Health;

        DuckFocus = duckFocus;
        PlayerFocus = playerFocus;
        Solo = solo;

    }
    protected override void Destruction2() {
        foreach (EnemyTurret t in TurretArray) {
            t.Ready = false;
        }
        foreach (EnemyLauncher l in LauncherArray) {
            l.Ready = false;
        }
        EnemyManager.Instance.EnemyDied(this);
        if (EnemyType == 0) {
            EnemyManager.Instance.FlagShipDestroyed = true;
        }
    }

    public Vector3 GetPosition() {
        return transform.position;
    }
    public Vector3 GetVelocity() {
        return ship.GetVelocity();
    }
}
