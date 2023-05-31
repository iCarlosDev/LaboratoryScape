using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Level1Manager : MonoBehaviour
{
    public static Level1Manager instance;
    [SerializeField] private Transform player;

    [Header("--- ALL ROOMS ---")]
    [Space(10)]
    [SerializeField] private List<RoomControl> roomsList;

    [Header("--- ALL ENEMIES ---")] 
    [Space(10)] 
    [SerializeField] private List<Enemy_IA> enemiesList;
    
    [Header("--- ALL ENEMIES SPAWNS ---")]
    [Space(10)]
    [SerializeField] private List<EnemiesSpawn> enemiesSpawnsList;

    [Header("--- SAFE ROOM ---")]
    [Space(10)]
    [SerializeField] private List<Transform> safeRoomWaypointsList; 
    
    [Header("--- ALL DOORS ---")]
    [Space(10)]
    [SerializeField] private List<DoorControl> doorsList;
    
    [Header("--- ALL BUTTONS ---")]
    [Space(10)]
    [SerializeField] private List<ButtonUnlockElevator> buttonUnlockElevatorList;

    [Header("--- ALARM PARAMETERS ---")]
    [Space(10)]
    [SerializeField] private Transform alarmWaypoint;
    [SerializeField] private bool alarmActivated;

    [Header("--- ALL UI ---")] 
    [Space(10)] 
    [SerializeField] private GameObject interactCanvas;
    
    [Header("--- TUTORIAL PARAMETERS ---")] 
    [Space(10)] 
    [SerializeField] private Transform playerStartPosTutorial;
    [SerializeField] private Transform playerStartPosLobby;

    //GETTERS && SETTERS//
    public bool AlarmActivated
    {
        get => alarmActivated;
        set => alarmActivated = value;
    }
    public List<RoomControl> RoomsList => roomsList;
    public List<Enemy_IA> EnemiesList => enemiesList;
    public List<Transform> SafeRoomWaypointsList => safeRoomWaypointsList;
    public Transform AlarmWaypoint => alarmWaypoint;
    public List<DoorControl> DoorsList => doorsList;
    public List<ButtonUnlockElevator> ButtonUnlockElevatorList => buttonUnlockElevatorList;
    public List<EnemiesSpawn> EnemiesSpawnsList => enemiesSpawnsList;
    public GameObject InteractCanvas => interactCanvas;

    ///////////////////////
    
    private void Awake()
    {
        instance = this;
        player = FindObjectOfType<PlayerMovement>().transform;
    }

    private void Start()
    {
        PauseMenuManager.instance.PauseMenu.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        OptionsManager.instance.SetAllOptions();

        roomsList.AddRange(FindObjectsOfType<RoomControl>());
        roomsList.Add(GameObject.FindWithTag("SafeRoomCollider").GetComponent<RoomControl>());
        
        enemiesList.AddRange(FindObjectsOfType<Enemy_IA>());
        
        enemiesSpawnsList.AddRange(FindObjectsOfType<EnemiesSpawn>());

        if (GameObject.FindWithTag("SafeRoomCollider") != null)
        {
            safeRoomWaypointsList.AddRange(GameObject.FindWithTag("SafeRoomCollider").GetComponentsInChildren<Transform>());
            safeRoomWaypointsList.Remove(safeRoomWaypointsList[0]);
        }

        doorsList.AddRange(FindObjectsOfType<DoorControl>());
        
        buttonUnlockElevatorList.AddRange(FindObjectsOfType<ButtonUnlockElevator>());
        
        interactCanvas.gameObject.SetActive(false);
        
        SetPlayerPosition();
        SetLevelMusic();
    }

    private void SetLevelMusic()
    {
        AudioManager.instance.Stop("MainTheme");
        AudioManager.instance.Play("DarkTension");
    }

    private void SetPlayerPosition()
    {
        Transform player = this.player;
        
        if (CheckpointsManager.instance.TutorialCompleted)
        {
            player.position = playerStartPosLobby.position;
            player.rotation = playerStartPosLobby.rotation;
        }
        else
        {
            player.position = playerStartPosTutorial.position;
            player.rotation = playerStartPosTutorial.rotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerRoot"))
        {
            CheckpointsManager.instance.TutorialCompleted = true;
            CheckpointsManager.instance.SaveCheckpoint();
        }
    }
}
