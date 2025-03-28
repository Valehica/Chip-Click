using UnityEngine;

public class gameManager : MonoBehaviour
{
    public Camera mainCamera;
    public Camera miniGameCamera;
    public bool miniJuegoActivo = false;

    void Start()
    {
        ActualizarCamara();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Para testear
        {
            miniJuegoActivo = !miniJuegoActivo;
            ActualizarCamara();
        }
    }

    void ActualizarCamara()
    {
        if (miniJuegoActivo)
        {
            mainCamera.gameObject.SetActive(false);
            miniGameCamera.gameObject.SetActive(true);

            Cursor.lockState = CursorLockMode.Confined; // Cursor queda dentro del juego
            Cursor.visible = true;
        }
        else
        {
            mainCamera.gameObject.SetActive(true);
            miniGameCamera.gameObject.SetActive(false);

            Cursor.lockState = CursorLockMode.Locked;   // Cursor oculto y centrado
            Cursor.visible = false;
        }
    }
}