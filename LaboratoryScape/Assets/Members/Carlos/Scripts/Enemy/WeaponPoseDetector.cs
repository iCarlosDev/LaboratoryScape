using UnityEngine;

public class WeaponPoseDetector : MonoBehaviour
{
    //Variables
    [SerializeField] private bool hasEntered;
    [SerializeField] private bool isBlocked;

    public bool IsBlocked
    {
        get => isBlocked;
        set => isBlocked = value;
    }

    public bool HasEntered
    {
        get => hasEntered;
        set => hasEntered = value;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            isBlocked = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            isBlocked = false;
        }
    }
}
