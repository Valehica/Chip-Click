using UnityEngine;

public class Cable3D : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private bool isDragging = false;
    private Vector3 startPoint;
    private Transform connectedEnd; // Donde se conecta el cable
    public Transform startTransform; // Conector de inicio
    public Color cableColor; // Color del cable

    void Start()
    {
        // Asegurar que el objeto tenga un LineRenderer
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // Configuración del LineRenderer
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = 2;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = cableColor;
        lineRenderer.endColor = cableColor;
        lineRenderer.enabled = false; // 🔹 Desactivado al inicio

        // Verificar que el conector de inicio esté asignado
        if (startTransform == null)
        {
            Debug.LogError("⚠️ Error: Start Transform no está asignado en el inspector.", this);
            return;
        }

        startPoint = startTransform.position;
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, startPoint);
    }

    void Update()
    {
        if (isDragging)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                lineRenderer.SetPosition(1, hit.point);
            }
            else
            {
                lineRenderer.SetPosition(1, ray.GetPoint(10));
            }
        }
    }

    void DetectarInicioArrastre()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider != null && hit.collider.gameObject == startTransform.gameObject)
            {
                isDragging = true;
                lineRenderer.enabled = true; // 🔹 Activar la línea al arrastrar
                Debug.Log("🔄 Arrastrando cable...");
            }
        }
    }

    void TryConnectCable()
    {
        isDragging = false;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            CableConnector3D connector = hit.collider.GetComponent<CableConnector3D>();

            if (connector != null && connector.lineColor == cableColor)
            {
                lineRenderer.SetPosition(1, connector.transform.position);
                connectedEnd = connector.transform;
                Debug.Log("✅ Cable Conectado Correctamente");
                return;
            }
        }

        // Si no se conecta, volver a la posición inicial
        lineRenderer.SetPosition(1, startPoint);
        lineRenderer.enabled = false; // 🔹 Ocultar si no se conectó
    }

    void OnMouseDown()
    {
        DetectarInicioArrastre();
    }

    void OnMouseUp()
    {
        TryConnectCable();
    }
}
