using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scientist_IA : Enemy_IA
{
    [SerializeField] private bool went1stSafeRoom;
    [SerializeField] private int randomSafeRoomWaypoint;

    public override void Start()
    {
        base.Start();
        
        //Seteamos un número Random que decidirá a que lugar de la sala segura irá el NPC;
        randomSafeRoomWaypoint = Random.Range(0, Level1Manager.instance.SafeRoomWaypointsList.Count);
    }

    public override void Update()
    {
        base.Update();
        
        //Si el player no ha sido detectado nunca hará la lógica restante;
        if (!IsPlayerDetected || isDead) return;
        
        //Se comprueba si tiene que huir del Player;
        if (IsPlayerDetected)
        {
            RunOfPlayer();
        }
    }

    //Método para huir del player;
    private void RunOfPlayer()
    {
        _navMeshAgent.speed = 3f;

        //Si la alarma está activada irá a la sala segura;
        if (Level1Manager.instance.AlarmActivated)
        {
            SearchSafeRoom();
        }
        //Si la alarma no está activada irá a activarla;
        else
        {
            GoActivateAlarm();
        }
    }

    //Método para ir a la sala segura;
    private void GoSafeRoom(int room, int waypoint)
    {
        Debug.Log("Going Safe Room");

        if (!went1stSafeRoom)
        {
            _navMeshAgent.SetDestination(Level1Manager.instance.SafeRoomWaypointsList[randomSafeRoomWaypoint].position);
            went1stSafeRoom = true;
        }

        if (_enemyScriptStorage.FieldOfView.canSeePlayer && went1stSafeRoom)
        {
            _navMeshAgent.SetDestination(Level1Manager.instance.RoomsList[room].transform.GetChild(waypoint).position);
        }

        //Si el NPC llega al waypoint se quedará quieto;
        if (Vector3.Distance(transform.position, _navMeshAgent.destination) < 0.1f)
        {
            _navMeshAgent.ResetPath();
        }
    }

    private void SearchSafeRoom()
    {
        int RandomRoom = Random.Range(1, Level1Manager.instance.RoomsList.Count);
        int RandomRoomWaypoint = Random.Range(1, Level1Manager.instance.RoomsList[RandomRoom].transform.childCount);
        
        GoSafeRoom(RandomRoom, RandomRoomWaypoint);
    }
}
