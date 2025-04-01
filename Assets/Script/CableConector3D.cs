using UnityEngine;

public class CableConnector3D : MonoBehaviour
{
    public bool isStartConnector = false;
    public bool isEndConnector = false;
    public Color lineColor = Color.white;

    private LineRenderer lineRenderer;
    private bool isDrawing = false;

    private static CableConnector3D activeStartConnector;
    private CableConnector3D connectedEndConnector;

    public bool sameColorConnection = false;

    void Start()
    {
        // Asignar color al material del cubo hijo
        Renderer cubeRenderer = GetComponentInChildren<Renderer>();
        if (cubeRenderer != null)
        {
            cubeRenderer.material.color = lineColor;
        }

        // Inicializar o agregar LineRenderer
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
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
                    // Si ya había una conexión, se reinicia
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

                    // Evaluar si los colores coinciden
                    sameColorConnection = this.lineColor == connectedEndConnector.lineColor;
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
}
