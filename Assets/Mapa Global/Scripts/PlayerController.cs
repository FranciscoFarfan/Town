using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    public float walkSpeed = 5f;
    public float crouchSpeed = 2.5f;
    public float jumpForce = 5f;
    public Transform cameraTransform;
    public float mouseSensitivity = 2f;
    public float crouchHeight = 0.5f;
    public float normalHeight = 1f;

    private Rigidbody rb;
    private float xRotation = 0f;
    private bool isGrounded = true;
    private bool isCrouching = false;
    private bool doorAvailable = false;
    private bool canSleep = false;

    public SunLightSim sunLightSim;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleMouseLook();
        HandleTeleportForward();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    [System.Obsolete]
    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        float speed = isCrouching ? crouchSpeed : walkSpeed;

        Vector3 velocity = move * speed;
        Vector3 rbVelocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
        rb.velocity = rbVelocity;
    }



    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleTeleportForward()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Dirección horizontal (sin componente vertical)
            Vector3 forward = cameraTransform.forward;
            forward.y = 0f;
            forward.Normalize();

            // Teletransporte 0.5 unidades hacia adelante
            transform.position += forward * 0.5f;
        }
    }

    void HandleSleep()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if(canSleep)
            {
                // Avanzar el tiempo al amanecer
                sunLightSim.timeOfDay = 0.25f; 
            }
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;

        if (collision.gameObject.CompareTag("Door"))
            doorAvailable = true;

        if (collision.gameObject.CompareTag("HourlyDoor"))
            if (sunLightSim.isDayTime)
                doorAvailable = true;

        if (collision.gameObject.CompareTag("Beed"))
            if (!sunLightSim.isDayTime)
                canSleep = true;
                
    }
}
