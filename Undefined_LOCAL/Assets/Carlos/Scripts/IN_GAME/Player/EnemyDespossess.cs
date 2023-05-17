using System;
using System.Collections;
using UnityEngine;

public class EnemyDespossess : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private PlayerScriptStorage _playerScriptStorage;
    [SerializeField] private Enemy_IA enemy;

    [Header("--- POSSESSION COOLDOWN ---")]
    [Space(10)]
    [SerializeField] private float possessionTime;
    private Coroutine possessionCooldown;

    public Enemy_IA Enemy => enemy;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Despossess();
        }
    }

    //Método para setear parametros necesarios cada vez que se active este objeto;
    public void StartUp(Enemy_IA enemy, GameObject player)
    {
        this.enemy = enemy;
        this.player = player;

        if (_playerScriptStorage == null)
        {
            _playerScriptStorage = this.player.GetComponent<PlayerScriptStorage>();
        }

        if (possessionCooldown != null)
        {
            StopCoroutine(possessionCooldown);
            possessionCooldown = null;
        }

        possessionCooldown = StartCoroutine(PossessionCooldown_Coroutine());
    }

    //Corrutina para desposeer al NPC cuando se agote el tiempo de posesión;
    private IEnumerator PossessionCooldown_Coroutine()
    {
        yield return new WaitForSeconds(possessionTime);
        Despossess();
    }
    
    //Método para desposeer al NPC;
    public void Despossess()
    {
        //Hacemos que el player y el enemigo q hemos poseido aparezcan en la posición y rotación en la q estemos;
        player.transform.position = transform.position;
        player.transform.rotation = transform.rotation;
        enemy.transform.position = transform.position;
        enemy.transform.rotation = transform.rotation;

        enemy.GetComponent<CapsuleCollider>().enabled = false;
        
        //Activamos el NPC poseido;
        enemy.gameObject.SetActive(true);
        
        //Matamos al NPC poseido;
        enemy.Die();
        
        //Activamos al player;
        player.SetActive(true);
        _playerScriptStorage.PlayerHealth.AddHealth(30);

        //Recorremos todos los enemigos en escena y cambiamos el player de referencia que tienen;
        foreach (Enemy_IA enemy in Level1Manager.instance.EnemiesList)
        {
            //Igualamos el player de referencia al EnemyFP;
            enemy.PlayerRef = player.transform.GetChild(4);
        }

        //Desactivamos el EnemyFP;
        gameObject.SetActive(false);
    }
}
