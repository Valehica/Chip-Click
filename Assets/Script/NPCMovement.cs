using System;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class NPCMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] TextMeshProUGUI texto;
    public int valorAparato;

    private void Start()
    {
        valorAparato = UnityEngine.Random.Range(10, 30);
        valorAparato = valorAparato * 10; 
        texto.text = "$"+valorAparato.ToString();

    }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void SetDestination(Vector3 targetPosition)
    {
        if (agent != null)
        {
            agent.SetDestination(targetPosition);
        }
    }
}