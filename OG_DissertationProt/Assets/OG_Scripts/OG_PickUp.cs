using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OG_PickUp : MonoBehaviour
{
    public enum PickUpType
    {
        Ammo,
        Health
    }

    public PickUpType pickUpType;

    public int in_ammoAmount;
    public int in_healthAmount;

    private OG_3D_Gun gunRef;
    private OG_PlayerHealth healthRef;

    public AudioClip ammoPickUp;
    public AudioClip healthPickUp;

    // Start is called before the first frame update
    void Start()
    {
        GameObject playerGO = GameObject.Find("Player");
        GameObject fpcGO = playerGO.transform.GetChild(0).gameObject;
        GameObject weaponHolderGO = fpcGO.transform.GetChild(1).gameObject;
        GameObject akmGO = weaponHolderGO.transform.GetChild(0).gameObject;
        gunRef = playerGO.GetComponentInChildren<OG_3D_Gun>();
        healthRef = GameObject.Find("Player").GetComponent<OG_PlayerHealth>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (pickUpType == PickUpType.Ammo)
            {
                AudioSource.PlayClipAtPoint(ammoPickUp, transform.position);
                gunRef.in_bulletsLeft += in_ammoAmount;
                Destroy(gameObject);
            }

            if (pickUpType == PickUpType.Health)
            {
                if (healthRef.fl_currnetPcHealth == healthRef.fl_maxHealth)
                {
                    return;
                }
                else
                {
                    AudioSource.PlayClipAtPoint(healthPickUp, transform.position);
                    healthRef.fl_currnetPcHealth += in_healthAmount;
                    Destroy(gameObject);
                }
            }
        }
    }
}
