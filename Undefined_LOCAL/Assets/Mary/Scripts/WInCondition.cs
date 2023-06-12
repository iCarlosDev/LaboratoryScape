using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WInCondition : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "PlayerRoot")
        {
            StartCoroutine(goToWinScene());
        }
    }

    IEnumerator goToWinScene()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(3);
    }
}
