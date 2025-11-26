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
    public int reputacion = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        AddToInv("ManzanaVerde", 4); 
        EarnMoney(50f);
        AddReputation(10);
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
        HandleJump();
        HandleInteraction();
        

    }

    // ---------------- INVENTARIO ----------------

    void EjecutarAccionO(){
        Debug.Log("Se presionó la tecla O, ejecutando acción...");
        
        Debug.Log(AddReputation(5));
        Debug.Log(EarnMoney(50f));
        Debug.Log(LoseReputation(2));
        Debug.Log(PayMoney(20f));
    }

    public string AddToInv(string nombre, int cantidad){
        PlayerItem item = inventario.Find(i => i.nombre == nombre);
        if (item != null)
        {
            item.cantidad += cantidad;
        }
        else
        {
            inventario.Add(new PlayerItem { nombre = nombre, cantidad = cantidad });
        }

        return $"Recibiste {cantidad} {nombre}(s) en tu inventario.";
    }

    public string SellFromInv(string nombre, int cantidad){
        PlayerItem item = inventario.Find(i => i.nombre == nombre);
        ItemData baseItem = GameManager.Instance.baseDeDatos.Find(i => i.nombre == nombre);

        if (item != null && item.cantidad >= cantidad && baseItem != null)
        {
            item.cantidad -= cantidad;

            // 🔹 Calcular factor de venta según rareza
            float factor = 1f;
            switch (baseItem.rareza)
            {
                case 1: // común
                    factor = Random.Range(0.65f, 0.75f); // alrededor del 70%
                    break;
                case 2: // poco común
                    factor = Random.Range(0.80f, 0.90f); // entre 80 y 90%
                    break;
                case 3: // raro
                    factor = Random.Range(0.95f, 1.05f); // 100% con ligera variación
                    break;
            }

            float precioVenta = baseItem.precio * factor;
            float ganancia = precioVenta * cantidad;
            dinero += ganancia;

            return $"Vendiste {cantidad} {nombre}(s) por {ganancia:F1} monedas (rareza {baseItem.rareza}, factor {factor:P0}).";
        }
        else
        {
            return $"No tienes suficientes {nombre} para vender.";
        }
    }


    public string BuyFromShop(string nombre, int cantidad){
        ItemData baseItem = GameManager.Instance.baseDeDatos.Find(i => i.nombre == nombre);

        if (baseItem != null)
        {
            float costo = baseItem.precio * cantidad;
            if (dinero >= costo)
            {
                dinero -= costo;
                AddToInv(nombre, cantidad);
                return $"Compraste {cantidad} {nombre}(s) por {costo} monedas.";
            }
            else
            {
                return $"No tienes suficiente dinero para comprar {cantidad} {nombre}(s).";
            }
        }
        else
        {
            return $"El objeto {nombre} no existe en la tienda.";
        }
    }

    public string RemoveFromInv(string nombre, int cantidad){
        PlayerItem item = inventario.Find(i => i.nombre == nombre);

        if (item != null && item.cantidad >= cantidad)
        {
            item.cantidad -= cantidad;
            return $"Se removieron {cantidad} {nombre}(s) de tu inventario.";
        }
        else
        {
            return $"No tienes suficientes {nombre} para remover.";
        }
    }

    // ---------------- DINERO ----------------
    public string PayMoney(float cantidad)
    {
        if (dinero >= cantidad)
        {
            dinero -= cantidad;
            return $"Pagaste {cantidad} monedas. Dinero restante: {dinero:F1}.";
        }
        else
        {
            return $"No tienes suficiente dinero para pagar {cantidad}.";
        }
    }

    public string EarnMoney(float cantidad)
    {
        dinero += cantidad;
        return $"Recibiste {cantidad} monedas. Dinero total: {dinero:F1}.";
    }

    // ---------------- REPUTACIÓN ----------------
    public string AddReputation(int cantidad)
    {
        reputacion += cantidad;
        return $"Tu reputación aumentó en {cantidad}. Reputación actual: {reputacion}.";
    }

    public string LoseReputation(int cantidad)
    {
        reputacion -= cantidad;
        if (reputacion < 0) reputacion = 0; // opcional: no permitir reputación negativa
        return $"Perdiste {cantidad} de reputación. Reputación actual: {reputacion}.";
    }




    public void MostrarInventario(){
        foreach (var item in inventario)
        {
            if (item.cantidad > 0) 
            {
                Debug.Log(item.nombre + " x" + item.cantidad);
            }
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
        if (Input.GetKeyDown(KeyCode.O)){
            EjecutarAccionO();
        }
        if (Input.GetKeyDown(KeyCode.I)){
            MostrarInventario();
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
