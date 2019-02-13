using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OG_WeaponSway : MonoBehaviour
{
    public float fl_swayAmount;
    public float fl_maxAmount;
    public float fl_smoothAmount;

    private Vector3 initialPos;


    void Start()
    {
        initialPos = transform.localPosition;
    }


    // Update is called once per frame
    void Update()
    {
        float movementX = -Input.GetAxis("Mouse X") * fl_swayAmount;
        float movementY = -Input.GetAxis("Mouse Y") * fl_swayAmount;

        movementX = Mathf.Clamp(movementX, -fl_maxAmount, fl_maxAmount);
        movementY = Mathf.Clamp(movementY, -fl_maxAmount, fl_maxAmount);

        Vector3 finalPos = new Vector3(movementX, movementY, 0);
        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPos + initialPos, Time.deltaTime * fl_smoothAmount);
    }
}
