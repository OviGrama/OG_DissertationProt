using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OG_Hearing_AI : MonoBehaviour
{
    public Vector3 v_position = new Vector3(1000f, 1000f, 1000f);
    public Vector3 v_resetPosition = new Vector3(1000f, 1000f, 1000f);

    public bool bl_playerInSight;
    public Vector3 v_personalLastSighting;

    private NavMeshAgent nav;
    private SphereCollider col;
    private Animator anim;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
