using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
public class OG_OnTriggerEnter : MonoBehaviour
{
    public GameObject FinishPanel;

    OG_3D_Gun gunref;
    FirstPersonController fpsctrl;

    private void Start()
    {
        GameObject player = GameObject.Find("Player");
        gunref = GetComponentInChildren<OG_3D_Gun>();

        fpsctrl = GameObject.Find("Player").GetComponent<FirstPersonController>();
    }


    private void OnTriggerEnter(Collider coll)
    {
        if(coll.gameObject.tag == "Player")
        {
            FinishPanel.SetActive(true);
            Time.timeScale = 0f;
            fpsctrl.enabled = false;
            gunref.enabled = false;
        }
    }
}
