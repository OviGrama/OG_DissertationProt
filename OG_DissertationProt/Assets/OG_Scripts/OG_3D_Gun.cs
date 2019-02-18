using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class OG_3D_Gun : MonoBehaviour
{
    [Header ("Weapon Properties")]
    public float fl_damage = 10f;
    public float fl_range = 100f;
    public float fl_fireRate = 15f;
    public float fl_impactForce = 100f;
    public float spread = 0.1f;
    public float recoil = 1f;
    public float fl_ADSSpeed = 8f;
    public Vector3 aimPos;
    private bool bl_isAiming = false;


[Header("Reloading Properties")]
    public int in_bulletsPerMag = 30;
    public int in_bulletsLeft;
    public int in_currentBullets;
    public float fl_reloadTime = 1f;
    private bool bl_isReloading = false;

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

    

    // Start is called before the first frame update
    void Start()
    {
        FpsController = GameObject.Find("Player").GetComponent<FirstPersonController>();
        weaponSwRef = GameObject.Find("Weapon Holder").GetComponent<OG_WeaponSway>();
        mAudioSource = GetComponent<AudioSource>();
        AmmoTxt = GameObject.Find("AmmoText").GetComponent<Text>();
        in_currentBullets = in_bulletsPerMag;
        originalPos = transform.localPosition;

        UpdateAmmoUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (bl_isReloading)
            return;

        if(in_currentBullets <= 0)
        {
            StartCoroutine(Reload());
            return;
        }


        if (Input.GetButton("Fire1") && Time.time >= fl_nextTimetoFire)
        {
            fl_nextTimetoFire = Time.time + 1f / fl_fireRate;
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (in_currentBullets < in_bulletsPerMag && in_bulletsLeft > 0)
            {
                StartCoroutine(Reload());
            }
        }

        ADS();
    }

    void UpdateAmmoUI()
    {
        Ammo = in_currentBullets + "/" + in_bulletsLeft;
        AmmoTxt.text = Ammo.ToString();
    }

    private void FixedUpdate()
    {
        //AnimatorStateInfo animInfo = anim.GetCurrentAnimatorStateInfo(0);
        anim.SetBool("Aim", bl_isAiming);
    }

    private void ADS()
    {
        if (Input.GetButton("Fire2") && !bl_isReloading)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, aimPos, Time.deltaTime * fl_ADSSpeed);
            bl_isAiming = true;
            weaponSwRef.fl_swayAmount = 0f;
            recoil = 0.3f;
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPos, Time.deltaTime * fl_ADSSpeed);
            bl_isAiming = false;
            weaponSwRef.fl_swayAmount = 0.1f;
            recoil = 0.45f;
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
            //if(hit.transform.tag == "Player")
            //{
            //    Physics.IgnoreCollision
            //}

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
                GameObject HoleGO = Instantiate(BulletHole[Random.Range(0,2)], hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
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

        }

        anim.CrossFadeInFixedTime("Fire", 0.01f);        

        in_currentBullets--;
        UpdateAmmoUI();
        Debug.Log(in_currentBullets + " bullets left in clip!");
        
    }

    IEnumerator Reload()
    {
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
