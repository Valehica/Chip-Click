using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    public Transform[] queuePoints; // puntos en la fila (primer punto es la caja)
    private List<NPCMovement> currentNPCs = new List<NPCMovement>();
    public int CurrentCount => currentNPCs.Count;
    public int MaxCount => queuePoints.Length;

    public void AddNPC(NPCMovement npc)
    {
        currentNPCs.Add(npc);
        UpdateQueuePositions();
    }

    public void RemoveNPC(NPCMovement npc)
    {
        currentNPCs.Remove(npc);
        UpdateQueuePositions();
    }

    void UpdateQueuePositions()
    {
        for (int i = 0; i < currentNPCs.Count; i++)
        {
            if (i < queuePoints.Length)
            {
                currentNPCs[i].SetDestination(queuePoints[i].position);
            }
        }
    }
}