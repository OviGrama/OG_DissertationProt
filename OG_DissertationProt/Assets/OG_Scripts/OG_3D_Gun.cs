using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OG_3D_Gun : MonoBehaviour
{
    public float fl_damage = 10f;
    public float fl_range = 100f;
    public float fl_fireRate = 15f;
    public float fl_impactForce = 100f;

    public Camera fpsCam;

    public ParticleSystem MuzzleFlash;
    public GameObject ImpactEffect;

    private float fl_nextTimetoFire = 0f;
    private AudioSource mAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        mAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= fl_nextTimetoFire)
        {
            fl_nextTimetoFire = Time.time + 1f / fl_fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        MuzzleFlash.Play();
        mAudioSource.Play();

        RaycastHit hit;
        if(Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, fl_range))
        {
            Debug.Log(hit.transform.name);

            OG_3D_Enemy enemyScRef = hit.transform.GetComponent<OG_3D_Enemy>();
            if(enemyScRef != null)
            {
                enemyScRef.TakeDamage(fl_damage);
            }

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * fl_impactForce);
            }

            GameObject ImpactGO =  Instantiate(ImpactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(ImpactGO, 2f);
        }

    }

}
