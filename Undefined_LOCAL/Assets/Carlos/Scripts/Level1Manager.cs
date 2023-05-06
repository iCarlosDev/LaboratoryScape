using System;
using System.Collections.Generic;
using UnityEngine;

public class Level1Manager : MonoBehaviour
{
    public static Level1Manager instance;

    [Header("--- ALL ROOMS ---")]
    [Space(10)]
    [SerializeField] private List<GameObject> roomsList;

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

    //GETTERS && SETTERS//
    public bool AlarmActivated
    {
        get => alarmActivated;
        set => alarmActivated = value;
    }
    public List<GameObject> RoomsList => roomsList;
    public List<Enemy_IA> EnemiesList => enemiesList;
    public List<Transform> SafeRoomWaypointsList => safeRoomWaypointsList;
    public Transform AlarmWaypoint => alarmWaypoint;
    public List<DoorControl> DoorsList => doorsList;
    public List<ButtonUnlockElevator> ButtonUnlockElevatorList => buttonUnlockElevatorList;
    public List<EnemiesSpawn> EnemiesSpawnsList => enemiesSpawnsList;

    ///////////////////////
    
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        PauseMenuManager.instance.PauseMenu.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        roomsList.AddRange(GameObject.FindGameObjectsWithTag("RoomCollider"));
        roomsList.Add(GameObject.FindWithTag("SafeRoomCollider"));
        
        enemiesList.AddRange(FindObjectsOfType<Enemy_IA>());
        
        enemiesSpawnsList.AddRange(FindObjectsOfType<EnemiesSpawn>());

        if (GameObject.FindWithTag("SafeRoomCollider") != null)
        {
            safeRoomWaypointsList.AddRange(GameObject.FindWithTag("SafeRoomCollider").GetComponentsInChildren<Transform>());
            safeRoomWaypointsList.Remove(safeRoomWaypointsList[0]);
        }

        doorsList.AddRange(FindObjectsOfType<DoorControl>());
        
        buttonUnlockElevatorList.AddRange(FindObjectsOfType<ButtonUnlockElevator>());
    }
}
