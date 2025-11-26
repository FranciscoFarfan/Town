using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class PlayerItem
{
    public string nombre;
    public int cantidad;
}


public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float walkSpeed = 5f;
    public Transform cameraTransform;
    public float mouseSensitivity = 2f;
    public float jumpForce = 5f;

    private Rigidbody rb;
    private float xRotation = 0f;
    private bool isGrounded = true;
    private IInteractuable currentInteractable;

    [Header("Inventario")]
    public List<PlayerItem> inventario = new List<PlayerItem>();
    public float dinero = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
        HandleJump();
        HandleInteraction();
    }

    // ---------------- INVENTARIO ----------------
    public void AddToInv(string nombre, int cantidad)
    {
        PlayerItem item = inventario.Find(i => i.nombre == nombre);
        if (item != null)
        {
            item.cantidad += cantidad;
        }
        else
        {
            inventario.Add(new PlayerItem { nombre = nombre, cantidad = cantidad });
        }
    }

    public void SellFromInv(string nombre, int cantidad)
    {
        PlayerItem item = inventario.Find(i => i.nombre == nombre);
        ItemData baseItem = GameManager.Instance.baseDeDatos.Find(i => i.nombre == nombre);

        if (item != null && item.cantidad >= cantidad && baseItem != null)
        {
            item.cantidad -= cantidad;
            dinero += baseItem.precio * cantidad;
        }
    }

    // ---------------- MOVIMIENTO ----------------
    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        Vector3 velocity = move * walkSpeed;
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z); // ⚠️ corregí "linearVelocity" → es "velocity"
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

    void HandleInteraction()
    {
        if (currentInteractable != null && Input.GetKeyDown(KeyCode.E))
        {
            currentInteractable.Interaccion();
        }
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trigger"))
        {
            currentInteractable = other.GetComponent<IInteractuable>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Trigger"))
        {
            if (currentInteractable != null)
                currentInteractable = null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
