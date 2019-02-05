using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class OG_3D_Gun : MonoBehaviour
{
    public float fl_damage = 10f;
    public float fl_range = 100f;
    public float fl_fireRate = 15f;
    public float fl_impactForce = 100f;
    public float spread = 0.1f;
    public float recoil = 1f;

    public Camera fpsCam;
    public Transform shootPoint;

    protected FirstPersonController FpsController;

    [SerializeField]
    private ParticleSystem MuzzleFlash;
    [SerializeField]
    private GameObject[] ImpactEffect;
    [SerializeField]
    private GameObject[] BulletHole;

    private float fl_nextTimetoFire = 0f;
    private AudioSource mAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        FpsController = GameObject.Find("Player").GetComponent<FirstPersonController>();
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

    Vector3 CalculateSpread(float spread, Transform shootPoint)
    {
        return Vector3.Lerp(shootPoint.TransformDirection(Vector3.forward * 100), Random.onUnitSphere, spread);
    }

    void Shoot()
    {
        MuzzleFlash.Play();
        mAudioSource.Play();

        Recoil();

        RaycastHit hit;
        if(Physics.Raycast(fpsCam.transform.position, CalculateSpread(spread, shootPoint), out hit, fl_range))
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

            if(hit.collider.tag == "Wall")
            {
                Transform HitObject = hit.collider.transform;
                GameObject HoleGO = Instantiate(BulletHole[Random.Range(0,2)], hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                HoleGO.transform.SetParent(HitObject);
                GameObject ImpactGO = Instantiate(ImpactEffect[0], hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(ImpactGO, 2f);
            }

            if (hit.collider.tag == "Ground")
            {
                Instantiate(BulletHole[Random.Range(3, 5)], hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                GameObject ImpactGO = Instantiate(ImpactEffect[1], hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(ImpactGO, 2f);

            }

        }

    }

    void Recoil()
    {
        FpsController.mouseLook.Recoil(recoil);
    }

}
