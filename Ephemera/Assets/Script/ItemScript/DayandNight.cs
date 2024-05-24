using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayandNight : MonoBehaviour
{
    [SerializeField] private float secondperRealtimeSecond;//게임 세계의 100초 = 현실 1초
    private bool isNight = false;
    [SerializeField] private float fogDensityCalc;
    [SerializeField] private float nightFogDensity;
    private float dayFogDensity;
    private float currentFogDensity;
    // Start is called before the first frame update
    void Start()
    {
        dayFogDensity = RenderSettings.fogDensity;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.right,0.1f*secondperRealtimeSecond*Time.deltaTime);
        if (transform.eulerAngles.x >= 170)
        {
            isNight = true;
        }
        else if (transform.eulerAngles.x >= 340)
        {
            isNight = false;
        }
        if(isNight)
        {
            if (currentFogDensity <= nightFogDensity)
            {
                currentFogDensity += 0.1f * fogDensityCalc * Time.deltaTime;
                RenderSettings.fogDensity = currentFogDensity;
            }
        }
        else
        {
            if (currentFogDensity > dayFogDensity)
            {
                currentFogDensity -= 0.1f * fogDensityCalc * Time.deltaTime;
                RenderSettings.fogDensity = currentFogDensity;
            }
        }
    }
}
