using UnityEngine;

public class Desatornillador : Interactuable
{
    private bool sobreTornillo = false;
    private Transform tornilloTransform;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("tornillo"))
        {
            Debug.Log("Desatornillador sobre tornillo");
            sobreTornillo = true;
            tornilloTransform = other.transform;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("tornillo"))
        {
            sobreTornillo = false;
            tornilloTransform = null;
        }
    }

    // 🔄 Sobrescribimos OnMouseDown para bloquear movimiento si está sobre el tornillo
    void OnMouseDown()
    {
        if (sobreTornillo)
        {
            // Evitar que se mueva
            Debug.Log("Está sobre el tornillo, no se puede mover");
            isDragging = false;
            return;
        }

        // Si no está sobre tornillo, se comporta normalmente
        transform.position = originalPosition + liftedPositionOffset;
        transform.rotation = Quaternion.Euler(liftedRotation);
        isDragging = true;
    }

    void OnMouseUp()
    {
        if (sobreTornillo && tornilloTransform != null)
        {
            // Fijar la posición encima del tornillo
            transform.position = new Vector3(tornilloTransform.position.x, transform.position.y, tornilloTransform.position.z);
            GameObject spin = transform.parent.Find("Spin").gameObject;
            spin.GetComponentInChildren<SpinnerDetector>().StartSpinning(this.transform, tornilloTransform);
        }
        else
        {
            // Volver a la posición original
            transform.position = originalPosition;
            transform.rotation = originalRotation;
        }

        isDragging = false;
    }
}