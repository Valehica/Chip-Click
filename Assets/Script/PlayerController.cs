using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rigidBody;
    public float movementSpeed;
    public Vector2 sensivity;
    public Transform camera;

    private float verticalRotation = 0f;
    public float verticalLimit = 80f;
    
    [SerializeField] private gameManager gameManager; 
    [SerializeField] private MinijuegoController miniJuegoController; 
    
    private bool movementEnabled = true;

    public float interactionDistance = 3f; // Distancia máxima para interactuar

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
    }

    private void updateMovement()
    {
        float hor = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");

        Vector3 velocity = Vector3.zero;

        if (hor != 0 || vert != 0)
        {
            Vector3 direction = (transform.forward * vert + transform.right * hor).normalized;
            velocity = direction * movementSpeed;
        }

        velocity.y = rigidBody.linearVelocity.y;
        rigidBody.linearVelocity = velocity;
    }

    private void UpdateMouseLook()
    {
        float hor = Input.GetAxis("Mouse X");
        float vert = Input.GetAxis("Mouse Y");

        if (hor != 0)
        {
            transform.Rotate(0, hor * sensivity.x, 0);
        }

        if (vert != 0)
        {
            verticalRotation -= vert * sensivity.y;
            verticalRotation = Mathf.Clamp(verticalRotation, -verticalLimit, verticalLimit);
            camera.localEulerAngles = new Vector3(verticalRotation, 0, 0);
        }
    }

    private void CheckInteraction()
    {
        if (Input.GetMouseButtonDown(0)) // Clic izquierdo
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
            Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.red, 1f);

            if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
            {
                GameObject hitObject = hit.collider.gameObject;

                if (hitObject.CompareTag("NPC"))
                {
                    Debug.Log("A TOCADO UN NPC");
                    int dineroAparato = hitObject.GetComponent<NPCMovement>().valorAparato;
                    miniJuegoController.dineroAparato = dineroAparato;
                    miniJuegoController.calcularValorFinalAparato();
                    gameManager.miniJuegoActivo = true;
                    gameManager.ActualizarCamara();
                    gameManager.npcActual = hitObject;
                }
                else if (hitObject.CompareTag("Computador"))
                {
                    Debug.Log("A TOCADO UN COMPUTADOR");
                    gameManager.AbrirComputador();
                }
                else
                {
                    Debug.Log("Se tocó: " + hitObject.name);
                }
            }
        }
    }
    
    public void SetMovementEnabled(bool enabled)
    {
        movementEnabled = enabled;

        // Bloquear/desbloquear el cursor según el estado
        Cursor.lockState = enabled ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !enabled;
    }
    void Update()
    {
        if (movementEnabled)
        {
            updateMovement();
            UpdateMouseLook();
        }

        CheckInteraction();
    }
}
