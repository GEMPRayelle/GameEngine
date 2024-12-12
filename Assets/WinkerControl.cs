using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WinkerControl : MonoBehaviour
{
    [SerializeField] GameObject[] winker;
    [SerializeField] float secTime;
    [SerializeField] bool winkerActive;
    void Start()
    {
        winkerActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        secTime += Time.deltaTime;

        if (winkerActive)
        {
            for (int j = 0; j < 4; j++)
            {
                winker[j].SetActive(true);
            }
            if (secTime >= 0.5f)
            {
                winkerActive = false;
            }
        }
        if (!winkerActive)
        {
            for (int j = 0; j < 4; j++)
            {
                winker[j].SetActive(false);
            }
            if (secTime > 1f)
            {
                secTime = 0;
                winkerActive = true;
            }
        }
    }
}
