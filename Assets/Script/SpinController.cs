using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI; // Necesario para manejar la imagen de la ruleta

public class SpinnerDetector : MonoBehaviour
{
    public Transform spinnerCenter; // Centro de la ruleta
    public Image spinnerImage; // Imagen de la ruleta

    private bool isSpinning = false;
    private float spinProgress = 0f;
    private Vector2 lastMousePos;

    private Transform destornillador;
    private Transform tornillo;
    [SerializeField] private float desplazamientoArriba;
    
    [SerializeField] private bool completoVueltas; 
    [SerializeField] private float cantidadAngulosCompletar;
    

    private void Start()
    {
        spinnerImage.gameObject.SetActive(false);
    }

    /// <summary>
    /// Función para iniciar el giro, la llamas desde otro script
    /// </summary>
    public void StartSpinning(Transform destornilladorReference = null, Transform tornilloReference = null)
    {
        destornillador = destornilladorReference;
        tornillo = tornilloReference;
        spinnerCenter = tornillo;
        spinnerImage.gameObject.SetActive(true);
        spinnerImage.rectTransform.position = new Vector3(tornillo.position.x, spinnerImage.rectTransform.position.y, spinnerCenter.position.z);
        isSpinning = true;
        lastMousePos = Input.mousePosition;
        
    }

    void Update()
    {
        if (isSpinning && Input.GetMouseButton(0)) // Mientras mantiene presionado el mouse
        {
            RotateSpinner();
        }

        if (Input.GetMouseButtonUp(0)) // Cuando suelta el clic
        {
            StopSpinning();
        }
    }

    private void RotateSpinner()
    {
        Vector2 currentMousePos = Input.mousePosition;

        // Vectores desde el centro hacia el mouse (antes y ahora)
        Vector2 prevDir = lastMousePos - (Vector2)spinnerCenter.position;
        Vector2 currDir = currentMousePos - (Vector2)spinnerCenter.position;

        // Calcula el ángulo entre los dos vectores
        float angle = Vector2.SignedAngle(prevDir, currDir);

        Debug.Log(math.abs(angle));
        // Aplica la rotación en el sentido correcto
        spinProgress += Mathf.Abs(angle);
        spinnerImage.rectTransform.localRotation = Quaternion.Euler(0, 0, -spinProgress * 6);

        if (destornillador.gameObject != null && tornillo.gameObject != null)
        {
            destornillador.localRotation = Quaternion.Euler(0, spinProgress * 6, -90);
            destornillador.localPosition += new Vector3(0, desplazamientoArriba, 0);
            
            tornillo.localRotation = Quaternion.Euler(0, 90, -spinProgress * 6);
            tornillo.localPosition += new Vector3(-desplazamientoArriba, 0, 0);
        }
        
        lastMousePos = currentMousePos;

        if (spinProgress >= cantidadAngulosCompletar)
        {
            completoTotalVueltas();
        }
    }
    
    
    private void completoTotalVueltas()
    {
        isSpinning = false;
        spinProgress = 0f; // Reiniciar el progreso
        completoVueltas = true;
        spinnerImage.gameObject.SetActive(false);
        destornillador.GetComponent<Interactuable>().restartPosition();
        
        GetComponentInParent<MinijuegoController>().sacoTornillos(tornillo);
        Debug.Log("YAAA LO SACOO");
    }
    private void StopSpinning()
    {
        Debug.Log("Progreso de giro: " + spinProgress);
        
    }
}