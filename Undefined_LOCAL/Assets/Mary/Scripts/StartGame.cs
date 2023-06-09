using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadSceneAsync());
        SceneManager.LoadScene(2);
    }
    
     IEnumerator LoadSceneAsync ()
      {
            AsyncOperation op = SceneManager.LoadSceneAsync(2);
            op.allowSceneActivation = false;
            while ( !op.isDone )
            {
                if (op.progress >= 0.99f)
                {
                    op.allowSceneActivation = true;   
                }
    
                yield return null;
            }
      }
}
