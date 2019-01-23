using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OG_3D_Gun : MonoBehaviour
{
    public float fl_damage = 10f;
    public float fl_range = 100f;

    public Camera fpsCam;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        RaycastHit hit;
        if(Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, fl_range))
        {
            Debug.Log(hit.transform.name);
        }
    }

}
