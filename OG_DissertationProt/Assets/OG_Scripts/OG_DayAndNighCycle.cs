using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OG_DayAndNighCycle : MonoBehaviour {

    [SerializeField] private Light sun;
    [SerializeField] private float secondsInFullDay = 120;


    [Range(0, 1)] [SerializeField] private float currentTimeofDay = 0;

    private float timeMultiplier = 1f;

    private float sunInitialIntensity;

    public bool bl_isNight;

    private void Start()
    {
        sunInitialIntensity = sun.intensity;
    }

    private void Update()
    {
        UpdateSun();
        currentTimeofDay += (Time.deltaTime / secondsInFullDay) * timeMultiplier;

        if (currentTimeofDay >= 1)
        {
            currentTimeofDay = 0;
        }
    }

    public void UpdateSun()
    {
        sun.transform.localRotation = Quaternion.Euler((currentTimeofDay * 360f) - 90, 170, 0);

        float intesityMultiplier = 1;

        if(currentTimeofDay <= 0.23f || currentTimeofDay >= 0.75f)
        {
            intesityMultiplier = 0;
        }

        else if (currentTimeofDay <= 0.25f)
        {
            intesityMultiplier = Mathf.Clamp01((currentTimeofDay - 0.23f) * (1 / 0.02f));
        }

        else if (currentTimeofDay >= 0.73f)
        {
            intesityMultiplier = Mathf.Clamp01(1 - ((currentTimeofDay - 0.73f) * (1 / 0.02f)));
        }

        sun.intensity = sunInitialIntensity * intesityMultiplier;
    }


}
