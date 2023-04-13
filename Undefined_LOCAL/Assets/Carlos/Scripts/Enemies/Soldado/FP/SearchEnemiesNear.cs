using System.Collections;
using UnityEngine;

public class SearchEnemiesNear : MonoBehaviour
{
    [SerializeField] private LayerMask layerToDetect;
    
    void Start()
    {
        StartCoroutine(CallNearSoldiers());
    }
    
    private IEnumerator CallNearSoldiers()
    {
        yield return new WaitForSeconds(0.2f);
        
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, 3, layerToDetect);
        
        foreach (Collider collider in colliderArray)
        {
            if (collider.GetComponent<Soldier_IA>())
            {
                collider.GetComponent<Soldier_IA>().IsPlayerDetected = true;
                collider.GetComponent<Soldier_IA>().StartCoroutine(collider.GetComponent<Soldier_IA>().DetectPlayer());
            }
            else
            {
                collider.GetComponent<Scientist_IA>().IsPlayerDetected = true;
                collider.GetComponent<Scientist_IA>().StartCoroutine(collider.GetComponent<Scientist_IA>().DetectPlayer());
            }
        }
        
        Destroy(gameObject, 1f);
    }
}
