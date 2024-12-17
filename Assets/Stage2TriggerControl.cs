using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage2TriggerControl : MonoBehaviour
{
    void Start()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        MissionManager.Instance.is_Stadium = true;
    }
}
