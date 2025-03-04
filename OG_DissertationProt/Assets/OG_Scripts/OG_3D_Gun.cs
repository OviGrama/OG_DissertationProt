﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class OG_3D_Gun : MonoBehaviour
{
    [Header ("Weapon Properties")]
    public int in_minDMG = 5;
    public int in_maxDMG = 10;
    public float fl_range = 100f;
    public float fl_fireRate = 15f;
    public float fl_impactForce = 100f;
    public float fl_soundRadius;
    public SphereCollider soundSphereColl;
    public float spread = 0.1f;
    public float recoil = 1f;
    public float fl_ADSSpeed = 8f;
    public Vector3 aimPos;
    private bool bl_isAiming = false;
    public bool bl_isShooting;
    public AudioClip shellFalling;
    public GameObject soundRadius;


    [Header("Reloading Properties")]
    public int in_bulletsPerMag = 30;
    public int in_bulletsLeft;
    public int in_currentBullets;
    public float fl_reloadTime = 1f;
    private bool bl_isReloading = false;
    public AudioSource reloadingSound;

    [Header("Weapon Req Components")]
    public Camera fpsCam;
    public Transform shootPoint;
    protected FirstPersonController FpsController;
    public Animator anim;

    [Header("Weapon Req Prefabs")]
    [SerializeField]
    private ParticleSystem MuzzleFlash;
    [SerializeField]
    private GameObject[] ImpactEffect;
    [SerializeField]
    private GameObject[] BulletHole;

    [Header("UI Properties and Req")]
    public Text AmmoTxt;
    public string Ammo;


    private float fl_nextTimetoFire = 0f;
    private AudioSource mAudioSource;
    private Vector3 originalPos;
    private OG_WeaponSway weaponSwRef;
    private OG_EnemyAi enemyAiRef;
    private OG_PlayerHealth playerHealth;

    

    // Start is called before the first frame update
    void Start()
    {
        FpsController = GameObject.Find("Player").GetComponent<FirstPersonController>();
        weaponSwRef = GameObject.Find("Weapon Holder").GetComponent<OG_WeaponSway>();
        playerHealth = GameObject.Find("Player").GetComponent<OG_PlayerHealth>();
        mAudioSource = GetComponent<AudioSource>();
        AmmoTxt = GameObject.Find("AmmoText").GetComponent<Text>();
        in_currentBullets = in_bulletsPerMag;
        originalPos = transform.localPosition;
        soundRadius.gameObject.SetActive(false);

        UpdateAmmoUI();
    }

    // Update is called once per frame
    void Update()
    {
        soundSphereColl.radius = fl_soundRadius;

        if (bl_isReloading)
            return;

        if(in_currentBullets <= 0)
        {
            StartCoroutine(Reload());
            return;
        }


        if (Input.GetButton("Fire1") && Time.time >= fl_nextTimetoFire && !playerHealth.bl_playerDead)
        {
            fl_nextTimetoFire = Time.time + 1f / fl_fireRate;
            Shoot();
            bl_isShooting = true;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (in_currentBullets < in_bulletsPerMag && in_bulletsLeft > 0)
            {
                StartCoroutine(Reload());
            }
        }

        ADS();
        UpdateAmmoUI();
    }

    IEnumerator SoundRadiuTimer()
    {
        soundRadius.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        soundRadius.gameObject.SetActive(false);
    }

    void UpdateAmmoUI()
    {
        Ammo = in_currentBullets + "/" + in_bulletsLeft;
        AmmoTxt.text = Ammo.ToString();
    }

    private void FixedUpdate()
    {
        anim.SetBool("Aim", bl_isAiming);
    }

    private void ADS()
    {
        if (Input.GetButton("Fire2") )//&& !bl_isReloading)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, aimPos, Time.deltaTime * fl_ADSSpeed);
            bl_isAiming = true;
            weaponSwRef.fl_swayAmount = 0f;
            recoil = .7f;
            spread = .5f;
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPos, Time.deltaTime * fl_ADSSpeed);
            bl_isAiming = false;
            weaponSwRef.fl_swayAmount = 0.1f;
            recoil = 1f;
            spread = .8f;
        }
    }

    Vector3 CalculateSpread(float spread, Transform shootPoint)
    {
        return Vector3.Lerp(shootPoint.TransformDirection(Vector3.forward * 100), Random.onUnitSphere, spread);
    }

    void Shoot() //(int vLayerMask)
    {
        MuzzleFlash.Play();
        mAudioSource.Play();
        StartCoroutine(SoundRadiuTimer());

        Recoil();

        RaycastHit hit;
        int layerMask = 1 << 10;

        layerMask = ~layerMask; // Collides with everything apart from layer 10.


        if (Physics.Raycast(fpsCam.transform.position, CalculateSpread(spread, shootPoint), out hit, fl_range, layerMask))
        {
            int int_randomDamage = Random.Range(in_minDMG, in_maxDMG);
            Debug.Log("Random damage is " + int_randomDamage);

            Debug.Log("LayerMask: " + hit.transform.gameObject.layer + ", Name: " + hit.transform.name);

            OG_EnemyAi enemyScRef = hit.transform.GetComponent<OG_EnemyAi>();
            if (enemyScRef != null)
            {
                enemyScRef.TakeDamage(int_randomDamage);
            }

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * fl_impactForce);
            }


            if (hit.collider.tag == "Metal")
            {
                Instantiate(BulletHole[Random.Range(0, 2)], hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                GameObject ImpactGO = Instantiate(ImpactEffect[0], hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(ImpactGO, 2f);

            }

            if (hit.collider.tag == "Ground")
            {
                Instantiate(BulletHole[Random.Range(3, 5)], hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                GameObject ImpactGO = Instantiate(ImpactEffect[1], hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(ImpactGO, 2f);

            }

            if (hit.collider.tag == "Enemy")
            {
                //Instantiate(BulletHole[Random.Range(6, 8)], hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                GameObject ImpactGO = Instantiate(ImpactEffect[2], hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(ImpactGO, 2f);
            }

            if (hit.collider.tag == "Wall")
            {
                GameObject HoleGO = Instantiate(BulletHole[Random.Range(0, 2)], hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                GameObject ImpactGO = Instantiate(ImpactEffect[3], hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(ImpactGO, 2f);
            }

            if (hit.collider.tag == "Wood")
            {
                GameObject HoleGO = Instantiate(BulletHole[Random.Range(0, 2)], hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                GameObject ImpactGO = Instantiate(ImpactEffect[4], hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(ImpactGO, 2f);
            }

            if (hit.collider.tag == "Glass")
            {
                GameObject HoleGO = Instantiate(BulletHole[Random.Range(0, 2)], hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                GameObject ImpactGO = Instantiate(ImpactEffect[5], hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(ImpactGO, 2f);
            }

            anim.CrossFadeInFixedTime("Fire", 0.01f);

            in_currentBullets--;
            UpdateAmmoUI();
            //Debug.Log(in_currentBullets + " bullets left in clip!");

        }
    }

    IEnumerator SheelSoundDelay()
    {
        float fl_randomDelay = Random.Range(1.5f, 3f);
        yield return new WaitForSeconds(fl_randomDelay);
        AudioSource.PlayClipAtPoint(shellFalling, transform.position);
    }

    IEnumerator Reload()
    {
        reloadingSound.Play();
        bl_isReloading = true;
        anim.SetBool("Reloading", true);
        yield return new WaitForSeconds(fl_reloadTime - .25f);
        anim.SetBool("Reloading", false);
        yield return new WaitForSeconds(.25f);

        if (in_bulletsLeft <= 0) yield break;

        int in_bulletsToLoad = in_bulletsPerMag - in_currentBullets;
        int in_bulletsToDeduct = (in_bulletsLeft >= in_bulletsToLoad) ? in_bulletsToLoad : in_bulletsLeft;

        in_bulletsLeft -= in_bulletsToDeduct;
        in_currentBullets += in_bulletsToDeduct;
        bl_isReloading = false;
        UpdateAmmoUI();
    }

    void Recoil()
    {
        FpsController.mouseLook.Recoil(recoil);
    }

}
