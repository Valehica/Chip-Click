using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rigidBody;
    
    public float movementSpeed;
    public Vector2 sensivity;
    public new Transform camera; 
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();  
        Cursor.lockState = CursorLockMode.Locked;
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
            camera.Rotate(-vert * sensivity.y, 0, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        updateMovement();
        UpdateMouseLook();
    }
}
