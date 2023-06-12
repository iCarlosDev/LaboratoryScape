using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }
    
     IEnumerator LoadSceneAsync ()
      {
            AsyncOperation op = SceneManager.LoadSceneAsync(0); 
            op.allowSceneActivation = false;
            while ( !op.isDone )
            {
                if (op.progress >= 0.99f)
                {
                    op.allowSceneActivation = true;
                    AudioManager.instance.Play("MainTheme");
                }
    
                yield return null;
            }
      }
}
