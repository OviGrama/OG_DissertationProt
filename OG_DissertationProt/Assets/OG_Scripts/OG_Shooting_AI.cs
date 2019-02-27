using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OG_Shooting_AI : MonoBehaviour
{

    public float fl_maximumDmg = 120f;
    public float minimuDmg = 45f;
    public AudioClip shotClip;

    private Animator anim;
    private SphereCollider sphCol;
    private Transform player;
    private OG_PlayerHealth pcHealthRef;
    private OG_EnemyAi aiMainRef;
    private bool bl_shooting;
    private float fl_scaledDamage;
    private bool bl_pcDetected = false;

    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();
        sphCol = GetComponent<SphereCollider>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        pcHealthRef = player.gameObject.GetComponent<OG_PlayerHealth>();
        aiMainRef = GameObject.FindGameObjectWithTag("Enemy").GetComponent<OG_EnemyAi>();

        fl_scaledDamage = fl_maximumDmg - minimuDmg;
    }

    // Update is called once per frame
    void Update()
    {
        if (aiMainRef.tDetected != null)
        {
            if (aiMainRef.tDetected.tag == "Player")
            {
                bl_pcDetected = true;
            }
            else
            {
                bl_pcDetected = false;
            }

            if (!bl_shooting && bl_pcDetected)      //aiMainRef.tDetected.tag == "Player")
            {
                Shoot();
            }

            //if (aiMainRef.tDetected == null)
            //{
            //    bl_shooting = false;
            //}
        }
    }

    void Shoot()
    {
        bl_shooting = true;
        float fl_fractionalDistance = (sphCol.radius - Vector3.Distance(transform.position, player.position)) / sphCol.radius;
        float fl_damage = fl_scaledDamage * fl_fractionalDistance + minimuDmg;
        pcHealthRef.TakeDamage(fl_damage);
    }


}
