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

    [Header("--- SAFE ROOM ---")]
    [Space(10)]
    [SerializeField] private List<Transform> safeRoomWaypointsList; 
    
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

    ///////////////////////
    
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        roomsList.AddRange(GameObject.FindGameObjectsWithTag("RoomCollider"));
        roomsList.Add(GameObject.FindWithTag("SafeRoomCollider"));
        
        enemiesList.AddRange(FindObjectsOfType<Enemy_IA>());

        safeRoomWaypointsList.AddRange(GameObject.FindWithTag("SafeRoomCollider").GetComponentsInChildren<Transform>());
        safeRoomWaypointsList.Remove(safeRoomWaypointsList[0]);
    }
}
