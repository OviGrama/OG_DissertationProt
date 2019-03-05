using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class OG_EnemyAi : MonoBehaviour
{
    [Header("Health")]
    public float fl_maxHealth = 100;
    public float fl_currentHealth;

    [Header("Necesarry Prefabs")]
    public AudioClip[] DeathSounds;
    public GameObject DeathVFX;
    public Transform dVFXoffSet;
    public Slider healthBar;


    GameObject Target;
    NavMeshAgent NavAgent;
    Animator anim;
    OG_GameManager gameManager;

    [Header("Shooting Properties")]
    //Shooting Var
    public int in_maxDmg;
    public int in_minDmg;
    public float fl_fireRate;
    public float fl_heightMultiplier;
    private float fl_nextTimeToFire;
    public float fl_ShootingStoppingDistance;
    public float fl_sightDist;
    [Range(1f, 15f)] public float fl_ViewOffsetAngle = 4f;
    [Range(4, 50)]public int in_NumberOFRays = 4;
    public AudioClip shootingSound;
    private GameObject tDetected = null;
    private OG_PlayerHealth pcHealthRef;
    private SphereCollider sphCol;

    [Header("Patrolling Properties")]
    // Patrolling Var
    public float fl_patrollingStoppingDistance;
    public GameObject[] waypoints;
    private int waypointInd;

    public enum State
    {
        PATROL,
        ATTACK,
        SOUNDAWARE,
        CHASE
    }

    [Header("States")]
    public State state;
    public bool bl_alive;





    private void Start()
    {
        NavAgent = GetComponent<NavMeshAgent>();
        Target = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
        sphCol = GetComponent<SphereCollider>();
        pcHealthRef = GameObject.Find("Player").GetComponent<OG_PlayerHealth>();
        gameManager = GameObject.Find("GameManager").GetComponent<OG_GameManager>();

        waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        waypointInd = Random.Range(0, waypoints.Length);

        //state = State.PATROL;

        bl_alive = true;

        fl_heightMultiplier = 1.1f;

        StartCoroutine(FSM());


        sphCol.radius = fl_sightDist;

        fl_currentHealth = fl_maxHealth;

    }

    private void FixedUpdate()
    {
        SwitchState();
        //StartCoroutine(SwitchState());
        RaycastFieldOfView();
        fl_ShootingStoppingDistance = fl_sightDist;
        sphCol.radius = fl_sightDist + 2;
        healthBar.value = CalculateHealth();
    }

    float CalculateHealth()
    {
        return fl_currentHealth / fl_maxHealth;
    }

    void RaycastFieldOfView()
    {
        RaycastHit hit;

        for (int i = 0; i < in_NumberOFRays; i++)
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

        for (int i = -1; i > -in_NumberOFRays; i--)
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

                case State.SOUNDAWARE:
                    print("Soundaware");
                    SoundAware();
                    break;
            }

            yield return null;
        }
    }

    // Make this a couroutine, make it happen 10 a second.
    void SwitchState()
    {

        // If the NPC detects the player
        if (tDetected != null) 
        {
            if (tDetected.tag == "Player" && !pcHealthRef.bl_playerDead)
            {
                state = State.ATTACK;
                Target = tDetected;
                FacePlayer();
            }
            else
            {
                state = State.PATROL;
            }
        }

        if (state == State.ATTACK)
        {
            if (tDetected == null)
            {
                state = State.PATROL;
            }
        }
    }

    void Patrol()
    {
        NavAgent.stoppingDistance = fl_patrollingStoppingDistance;

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
        NavAgent.stoppingDistance = fl_ShootingStoppingDistance;
        NavAgent.SetDestination(Target.transform.position);
        anim.SetBool("bl_Run", false);
        anim.SetBool("bl_Shoot", true);
        if(Time.time >= fl_nextTimeToFire && !pcHealthRef.bl_playerDead)
        {
            fl_nextTimeToFire = Time.time + 1f / fl_fireRate;
            DamageThePlayer();
        }
    }

    void SoundAware()
    {
        //Target = GameObject.Find("Player");
        NavAgent.stoppingDistance = fl_ShootingStoppingDistance;
        NavAgent.SetDestination(Target.transform.position);
        NavAgent.updatePosition = true;
        anim.SetBool("bl_Run", true);
        anim.SetBool("bl_Shoot", false);
    }

    void DamageThePlayer()
    {
        gameManager.fl_difficulty -= 0.5f;
        int damageOutput = Random.Range(in_minDmg, in_maxDmg);
        AudioSource.PlayClipAtPoint(shootingSound, transform.position);
        pcHealthRef.TakeDamage(damageOutput);
    }


    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "SoundRadius")
        {
            //Target = sDetected;
            //print(sDetected.name);
            state = State.SOUNDAWARE;
            FacePlayer();
        }

        //if (coll.gameObject.tag == "Player")
        //{
        //    state = State.SOUNDAWARE;
        //}
    }



    //private void OnTriggerExit(Collider coll)
    //{
    //    if (coll.tag == "SoundAware")
    //    {
    //        state = State.SOUNDAWARE;
    //        FacePlayer();
    //    }
    //}






    private void FacePlayer()
    {
        Vector3 lookPos = Target.transform.position - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 5);
    }

    public void TakeDamage(int amount)
    {
        fl_currentHealth -= amount;
        gameManager.fl_difficulty += 0.5f;

        if (fl_currentHealth <= 0f)
        {
            Death();
        }
    }

    public void Death()
    {
        gameManager.fl_difficulty += 1f;
        AudioSource.PlayClipAtPoint(DeathSounds[Random.Range(0, DeathSounds.Length)], transform.position);
        GameObject deathParticle = Instantiate(DeathVFX, dVFXoffSet.transform.position, transform.rotation);
        bl_alive = false;
        Destroy(gameObject);
        Destroy(deathParticle, 3);
    }
}