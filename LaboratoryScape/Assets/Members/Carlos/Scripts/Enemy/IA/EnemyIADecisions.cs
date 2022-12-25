using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyIADecisions : MonoBehaviour
{
    [SerializeField] private EnemyScriptsStorage _enemyScriptsStorage;
    
    class Rule
    {
        public int priority; // Prioridad de la regla
        public Func<bool> condition; // Función que evalúa la condición de la regla
        public Action action; // Acción a realizar si se cumple la condición

        public Rule(int priority, Func<bool> condition, Action action)
        {
            this.priority = priority;
            this.condition = condition;
            this.action = action;
        }
    }

    List<Rule> rules = new List<Rule>(); // Lista de reglas
    
    [Header("--- DECISIONS EVALUATION CONTROL ---")]
    [Space(10)]
    float timeSinceLastEvaluation = 0f; // Tiempo transcurrido desde la última evaluación de reglas
    float evaluationInterval = 1f; // Intervalo de tiempo entre evaluaciones de reglas

    private void Awake()
    {
        _enemyScriptsStorage = GetComponent<EnemyScriptsStorage>();
    }

    private void Start()
    {
        // Añade una regla con alta prioridad que dice "si el enemigo está a menos de 5 metros de distancia, dispara"
        AddRule(10, () => {
            // Aquí va el código que evalúa si el enemigo está a menos de 5 metros de distancia
            float distance = Vector3.Distance(_enemyScriptsStorage.FieldOfView.playerRef.transform.position, transform.position);
            
            return distance < 5f; // Devuelve true si se cumple la condición
        }, () => {
            // Aquí va el código que hace que el enemigo dispare
        });
        
        // Añade una regla con baja prioridad que dice "si el enemigo está a más de 10 metros de distancia, busca una posición de cubierto"
        AddRule(1, () => {
            // Aquí va el código que evalúa si el enemigo está a más de 10 metros de distancia
            float distance = Vector3.Distance(_enemyScriptsStorage.FieldOfView.playerRef.transform.position, transform.position);
            
            return distance > 5f; // Devuelve true si se cumple la condición
        }, () => {
            // Obtiene una posición aleatoria dentro del navmesh
            NavMeshHit hit;
            NavMesh.SamplePosition(transform.position + Random.insideUnitSphere * 5, out hit, 5, NavMesh.AllAreas);

            // Hace que el enemigo se mueva hacia la posición de cubierto
            GetComponent<NavMeshAgent>().destination = hit.position;
        });
    }
    
    // Añade una regla al sistema
    public void AddRule(int priority, Func<bool> condition, Action action)
    {
        rules.Add(new Rule(priority, condition, action));
    }

    // Función que evalúa todas las reglas en orden de prioridad y realiza la acción correspondiente si se cumple la condición de la regla
    public void EvaluateRules()
    {
        // Ordena las reglas por prioridad
        rules.Sort((r1, r2) => r2.priority.CompareTo(r1.priority));

        // Evalúa cada regla en orden de prioridad
        foreach (var rule in rules)
        {
            // Si se cumple la condición de la regla, realiza la acción correspondiente y detiene la evaluación de las demás reglas
            if (rule.condition())
            {
                rule.action();
                break;
            }
        }
    }

    public void EvaluateRulesUpdate()
    {
        timeSinceLastEvaluation += Time.deltaTime;
        if (timeSinceLastEvaluation >= evaluationInterval)
        {
            timeSinceLastEvaluation = 0;
            EvaluateRules();
        }
    }
}
