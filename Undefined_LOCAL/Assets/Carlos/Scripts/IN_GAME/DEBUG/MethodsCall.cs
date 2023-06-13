using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MethodsCall : MonoBehaviour
{
    private void Update()
    {
        
    }

    private void DestroyFusibles()
    {
        FindObjectOfType<FusiblesControl>().SendMessage("DestroyFusibles");
    }
}
