using UnityEngine;
using System.Collections.Generic;

public enum CableColor
{
    Red,
    Green,
    Blue,
    Yellow
}

public class CableConnector3D : MonoBehaviour
{
    public bool isStartConnector = false;
    public bool isEndConnector = false;
    public CableColor lineColor;
    public Color color;

    private LineRenderer lineRenderer;
    private bool isDrawing = false;

    private static CableConnector3D activeStartConnector;
    private CableConnector3D connectedEndConnector;

    public bool sameColorConnection = false;

    // NUEVO: Lista para registrar todos los conectores
    private static List<CableConnector3D> allConnectors = new List<CableConnector3D>();

    void Start()
    {
        // Registrar esta instancia
        allConnectors.Add(this);

        color = GetUnityColor(lineColor);
        Renderer cubeRenderer = GetComponentInChildren<Renderer>();
        if (cubeRenderer != null)
        {
            cubeRenderer.material.color = GetUnityColor(lineColor);
        }

        lineRenderer = gameObject.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.startColor = GetUnityColor(lineColor);
        lineRenderer.endColor = GetUnityColor(lineColor);
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
    }

    private Color GetUnityColor(CableColor cableColor)
    {
        switch (cableColor)
        {
            case CableColor.Red: return Color.red;
            case CableColor.Green: return Color.green;
            case CableColor.Blue: return Color.blue;
            case CableColor.Yellow: return Color.yellow;
            default: return Color.white;
        }
    }

    void Update()
    {
        if (lineRenderer == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
            {
                if (isStartConnector)
                {
                    if (connectedEndConnector != null)
                    {
                        connectedEndConnector = null;
                        sameColorConnection = false;
                    }

                    activeStartConnector = this;
                    isDrawing = true;
                    lineRenderer.enabled = true;
                }
            }
        }

        if (isDrawing && activeStartConnector == this)
        {
            Vector3 worldMousePos = GetMouseWorldPosition();
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, worldMousePos);
        }

        if (Input.GetMouseButtonUp(0) && activeStartConnector == this)
        {
            isDrawing = false;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                CableConnector3D targetConnector = hit.collider.GetComponent<CableConnector3D>();

                if (targetConnector != null && targetConnector != this && targetConnector.isEndConnector)
                {
                    connectedEndConnector = targetConnector;
                    lineRenderer.SetPosition(1, connectedEndConnector.transform.position);
                    lineRenderer.enabled = true;

                    sameColorConnection = this.lineColor == connectedEndConnector.lineColor;
                    var componenteMinijuego = transform.parent?.parent?.GetComponent<MinijuegoController>();

                    if (componenteMinijuego != null)
                    {
                        componenteMinijuego.coneccion(GetUnityColor(lineColor), sameColorConnection);
                    }
                }
                else
                {
                    ResetCable();
                }
            }
            else
            {
                ResetCable();
            }

            activeStartConnector = null;
        }
    }

    private void ResetCable()
    {
        connectedEndConnector = null;
        sameColorConnection = false;
        lineRenderer.enabled = false;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.down, transform.position);

        float distance;
        if (plane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }
        return transform.position;
    }

    // NUEVO: Función pública para resetear todas las conexiones
    public static void ResetAllConnections()
    {
        foreach (var connector in allConnectors)
        {
            connector.ResetCable();
        }
        activeStartConnector = null;
    }
}
