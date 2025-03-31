using UnityEngine;

public class Interactuable : MonoBehaviour
{
    [SerializeField] protected Vector3 liftedPositionOffset = new Vector3(0, 0.2f, 0); // Editable en Unity
    [SerializeField] protected Vector3 liftedRotation = new Vector3(0, 0, 90); // Editable en Unity

    protected Vector3 originalPosition;
    protected Quaternion originalRotation;
    protected bool isDragging = false;

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
        restartPosition();
    }

    public void restartPosition()
    {
        // Vuelve a su posición y rotación original
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        isDragging = false;
    }
}