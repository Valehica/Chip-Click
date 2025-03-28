using UnityEngine;

public class Interactuable : MonoBehaviour
{
    public Vector3 liftedPositionOffset = new Vector3(0, 0.2f, 0); // subir 20 cm
    public Vector3 liftedRotation = new Vector3(0, 0, 90); // rotación a vertical

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private bool isDragging = false;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    void OnMouseDown()
    {
        // Eleva y rota la herramienta
        transform.position = originalPosition + liftedPositionOffset;
        transform.rotation = Quaternion.Euler(liftedRotation);
        isDragging = true;
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            // Mueve el objeto siguiendo la posición del mouse
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z; // mantener profundidad
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            transform.position = new Vector3(worldPos.x, transform.position.y, worldPos.z); // mantener altura
        }
    }

    void OnMouseUp()
    {
        // Vuelve a su posición y rotación original
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        isDragging = false;
    }
}