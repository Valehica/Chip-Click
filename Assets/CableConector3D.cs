using Unity.VisualScripting;
using UnityEngine;

public class CableConnector3D : MonoBehaviour
{
    public Color lineColor = Color.white; // Color del LineRenderer, configurable desde Unity.
    
    private LineRenderer lineRenderer;
    private bool isDrawing = false;

    void Start()
    {
        // Crear el LineRenderer
        lineRenderer = gameObject.GetComponent<LineRenderer>();

        if (lineRenderer == null)
        {
            Debug.LogError("No se pudo inicializar el LineRenderer.");
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            return;
        }

        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false; // Se oculta al inicio
    }

    void Update()
    {
        if (lineRenderer == null) return; // Prevención de errores

        if (Input.GetMouseButtonDown(0)) // Detectar clic
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == gameObject) // Verifica si el objeto fue clickeado
                {
                    isDrawing = true;
                    lineRenderer.enabled = true;
                }
            }
        }

        if (isDrawing)
        {
            Vector3 worldMousePos = GetMouseWorldPosition();
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, worldMousePos);
        }

        if (Input.GetMouseButtonUp(0)) // Detectar cuando se suelta el clic
        {
            isDrawing = false;
            lineRenderer.enabled = false;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        // Ajustar proyección considerando la rotación de la cámara (90, 90, 0)
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.down, transform.position); // Plano paralelo al eje Y

        float distance;
        if (plane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }
        return transform.position;
    }
}
