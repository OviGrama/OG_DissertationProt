using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class OG_EnemyAi : MonoBehaviour
{
    public float fl_health = 100;
    public GameObject DeathVFX;
    public Transform dVFXoffSet;

    GameObject Target;
    NavMeshAgent NavAgent;
    Animator anim;

    public enum State
    {
        PATROL,
        ATTACK,
        INVESTIGATE,
    }

    public State state;
    public bool bl_alive;

    // Patrolling Var
    public GameObject[] waypoints;
    private int waypointInd;

    // Investigating Var
    private Vector3 investigateSpot;
    private float fl_timer = 0;
    public float investigateWait = 10;

    //Sight Var
    public float fl_heightMultiplier;
    public float fl_sightDist = 10;
    [Range(1f, 15f)] public float fl_ViewOffsetAngle = 5f;
    [Range(1, 10)] public int fl_HalfRayAmount = 4;

    private void Start()
    {
        NavAgent = GetComponent<NavMeshAgent>();
        Target = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();

        waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        waypointInd = Random.Range(0, waypoints.Length);

        state = State.PATROL;

        bl_alive = true;

        fl_heightMultiplier = 1.1f;

        StartCoroutine("FSM");
    }

    // Add a timer for this coroutine.
    IEnumerator FSM()
    {
        while (bl_alive)
        {
            switch (state)
            {
                case State.PATROL:
                    print("Patrol");
                    Patrol();
                    break;

                case State.ATTACK:
                    print("Attack");
                    Attack();
                    break;

                case State.INVESTIGATE:
                    print("Investigate");
                    Investigate();
                    break;
            }

            yield return null;
        }
    }

    void Patrol()
    {
        NavAgent.stoppingDistance = 3;

        if (Vector3.Distance(this.transform.position, waypoints[waypointInd].transform.position) >= 2)
        {
            NavAgent.SetDestination(waypoints[waypointInd].transform.position);
            NavAgent.updatePosition = true;
            anim.SetBool("bl_Run", true);
            anim.SetBool("bl_Shoot", false);
        }

        else if (Vector3.Distance(this.transform.position, waypoints[waypointInd].transform.position) <= 2)
        {
            waypointInd = Random.Range(0, waypoints.Length);
        }

        else
        {
            NavAgent.updatePosition = false;
        }
    }

    void Attack()
    {
        NavAgent.stoppingDistance = 10;
        NavAgent.SetDestination(Target.transform.position);
        anim.SetBool("bl_Run", false);
        anim.SetBool("bl_Shoot", true);
    }

    void Investigate()
    {
        fl_timer += Time.deltaTime;

        NavAgent.SetDestination(transform.position);
        NavAgent.updatePosition = false;
        transform.LookAt(investigateSpot);
        anim.SetBool("bl_Run", false);
        anim.SetBool("bl_Shoot", false);

        if (fl_timer >= investigateWait)
        {
            state = State.PATROL;
            fl_timer = 0;
        }
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "Player")
        {
            state = State.INVESTIGATE;
            investigateSpot = coll.gameObject.transform.position;
            FacePlayer();
        }
    }

    private void FixedUpdate()
    {
        SwitchState();
    }

    private GameObject tDetected = null;

    void RaycastFieldOfView()
    {
        RaycastHit hit;

        for (int i = 0; i < fl_HalfRayAmount; i++)
        {
            if (i == 0) // Centre Ray.
            {
                Debug.DrawRay(transform.position + Vector3.up * fl_heightMultiplier, transform.forward * fl_sightDist, Color.red);

                if (Physics.Raycast(transform.position + Vector3.up * fl_heightMultiplier, transform.forward, out hit, fl_sightDist))
                {
                    if (hit.collider.gameObject.tag == "Player")
                    {
                        tDetected = hit.collider.gameObject;
                        print(tDetected.name);
                    }

                    else if (hit.collider.gameObject.tag != "Player")
                    {
                        tDetected = null;
                    }
                }
            }

            else
            {
                Debug.DrawRay(transform.position + Vector3.up * fl_heightMultiplier, Quaternion.AngleAxis(fl_ViewOffsetAngle * i, transform.up).normalized * transform.forward * fl_sightDist, Color.red);

                if (Physics.Raycast(transform.position + Vector3.up * fl_heightMultiplier, Quaternion.AngleAxis(fl_ViewOffsetAngle * i, transform.up).normalized * transform.forward, out hit, fl_sightDist))
                {
                    if (hit.collider.gameObject.tag == "Player")
                    {
                        tDetected = hit.collider.gameObject;
                        print(tDetected.name);
                    }

                    else if (hit.collider.gameObject.tag != "Player")
                    {
                        tDetected = null;
                    }
                }
            }
        }

        for (int i = -1; i > -fl_HalfRayAmount; i--)
        {
            Debug.DrawRay(transform.position + Vector3.up * fl_heightMultiplier, Quaternion.AngleAxis(fl_ViewOffsetAngle * i, transform.up).normalized * transform.forward * fl_sightDist, Color.red);

            if (Physics.Raycast(transform.position + Vector3.up * fl_heightMultiplier, Quaternion.AngleAxis(fl_ViewOffsetAngle * i, transform.up).normalized * transform.forward, out hit, fl_sightDist))
            {
                if (hit.collider.gameObject.tag == "Player")
                {
                    tDetected = hit.collider.gameObject;
                    print(tDetected.name);
                }

                else if (hit.collider.gameObject.tag != "Player")
                {
                    tDetected = null;
                }
            }
        }
    }

    // Make this a couroutine, make it happen 10 a second.
    void SwitchState()
    {
        RaycastFieldOfView();

        if (tDetected == null)
        {
            state = State.PATROL;
        }

        // If the NPC detects the player
        if (tDetected != null) 
        {
            if (tDetected.tag == "Player")
            {
                state = State.ATTACK;
                Target = tDetected;
                FacePlayer();
            }
        }

        if (state == State.ATTACK)
        {
            if (tDetected == null)
            {
                state = State.INVESTIGATE;
            }
        }
    }

    private void FacePlayer()
    {
        Vector3 lookPos = Target.transform.position - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 5);
    }

    public void TakeDamage(float amount)
    {
        fl_health -= amount;

        if (fl_health <= 0f)
        {
            Death();
        }
    }

    void Death()
    {
        GameObject deathParticle = Instantiate(DeathVFX, dVFXoffSet.transform.position, transform.rotation);
        Destroy(gameObject);
        Destroy(deathParticle, 3);
    }
}