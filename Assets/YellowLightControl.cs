using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowLightControl : MonoBehaviour
{
    [SerializeField] GameObject light;
    [SerializeField] float secTime;
    [SerializeField] bool lightActive;
    void Start()
    {
        lightActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        secTime += Time.deltaTime;

        if (lightActive)
        {
            light.SetActive(true);
            if (secTime >= 1f)
            {
                lightActive = false;
            }
        }
        if (!lightActive)
        {
            light.SetActive(false);
            if (secTime > 2f)
            {
                secTime = 0;
                lightActive = true;
            }
        }
    }
}
