using System;
using UnityEngine;

public class CheckpointsManager : MonoBehaviour
{
    public static CheckpointsManager instance;
    
    [SerializeField] private bool _tutorialCompleted;

    //GETTERS && SETTERS//
    public bool TutorialCompleted
    {
        get => _tutorialCompleted;
        set => _tutorialCompleted = value;
    }
    
    //////////////////////////////////////

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        LoadCheckpoint();
    }

    public void SaveCheckpoint()
    {
        PlayerPrefs.SetInt("TutorialCheckpoint", _tutorialCompleted ? 1 : 0);
    }

    public void LoadCheckpoint()
    {
        if (!PlayerPrefs.HasKey("TutorialCheckpoint")) return;
        _tutorialCompleted = PlayerPrefs.GetInt("TutorialCheckpoint") == 1;
    }
}
