using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearTriggerControl : MonoBehaviour
{
    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider col)
    {
        if (MissionManager.Instance.is_StadiumDef)
        {
            if (col.gameObject.tag == "Player")
                MissionManager.Instance.is_Clear = true;
        }
    }
}
