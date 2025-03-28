using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public GameObject npcPrefab;
    public QueueManager queueManager;
    public float spawnInterval = 5f;

    void Start()
    {
        InvokeRepeating(nameof(SpawnNPC), 1f, spawnInterval);
    }

    void SpawnNPC()
    {
        // Solo spawnea si hay espacio en la fila
        if (queueManager.CurrentCount < queueManager.MaxCount)
        {
            GameObject newNPC = Instantiate(npcPrefab, transform.position, transform.rotation);
            NPCMovement npcScript = newNPC.GetComponent<NPCMovement>();
            queueManager.AddNPC(npcScript);
        }
    }
}