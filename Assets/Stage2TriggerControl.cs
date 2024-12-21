using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage2TriggerControl : MonoBehaviour
{
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider col)
    {
        Debug.Log(col.gameObject.name);
        if (col.gameObject.tag == "Player")
        MissionManager.Instance.is_Stadium = true;
    }
}
