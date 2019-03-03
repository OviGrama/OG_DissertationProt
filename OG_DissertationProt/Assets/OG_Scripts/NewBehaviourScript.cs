using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private bool bl_isReloading = false;
    public Vector3 aimPos;
    public float fl_ADSSpeed = 8f;
    private bool bl_isAiming = false;
    private OG_WeaponSway weaponSwRef;
    public float recoil = 1f;
    private Vector3 originalPos;


    // Start is called before the first frame update
    void Start()
    {
        originalPos = transform.localPosition;
        weaponSwRef = GameObject.Find("Weapon Holder").GetComponent<OG_WeaponSway>();

    }

    // Update is called once per frame
    void Update()
    {
        ADS();
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
            recoil = 0.5f;
        }
    }

}
