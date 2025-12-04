using UnityEngine;
using System.Collections.Generic;
using TMPro;

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

    [Header("Control")]
    public bool controleActivo = true; // Permite desactivar el control durante cinemáticas

    [Header("Inventario")]
    public List<PlayerItem> inventario = new List<PlayerItem>();
    public float dinero = 0f;
    public int reputacion = 0;

    [Header("UI Interacción")]
    public GameObject panelInteraccion;
    public TextMeshProUGUI textoInteraccion;

    [Header("UI Stats")]
    public TextMeshProUGUI textoStats; // Muestra dinero y reputación

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        AddToInv("ManzanaVerde", 4); 
        EarnMoney(50f);
        AddReputation(10);

        if (panelInteraccion != null)
            panelInteraccion.SetActive(false);

        ActualizarUIStats();
    }

    void Update()
    {
        if (controleActivo)
        {
            HandleMouseLook();
            HandleMovement();
            HandleJump();
        }
        HandleInteraction();
        ActualizarUIStats(); // Actualizar constantemente
    }

    void EjecutarAccionO()
    {
        
    }

    public string AddToInv(string nombre, int cantidad)
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
        return $"Recibiste {cantidad} {nombre}(s) en tu inventario.";
    }

    public string SellFromInv(string nombre, int cantidad)
    {
        PlayerItem item = inventario.Find(i => i.nombre == nombre);
        ItemData baseItem = GameManager.Instance.baseDeDatos.Find(i => i.nombre == nombre);

        if (item != null && item.cantidad >= cantidad && baseItem != null)
        {
            item.cantidad -= cantidad;

            float factor = 1f;
            switch (baseItem.rareza)
            {
                case 1:
                    factor = Random.Range(0.65f, 0.75f);
                    break;
                case 2:
                    factor = Random.Range(0.80f, 0.90f);
                    break;
                case 3:
                    factor = Random.Range(0.95f, 1.05f);
                    break;
            }

            float precioVenta = baseItem.precio * factor;
            float ganancia = precioVenta * cantidad;
            dinero += ganancia;

            // Notificar si vendió item de misión de entregas
            Entregas.OnItemVendido(nombre);

            ActualizarUIStats();
            return $"Vendiste {cantidad} {nombre}(s) por {ganancia:F1} monedas (rareza {baseItem.rareza}, factor {factor:P0}).";
        }
        else
        {
            return $"No tienes suficientes {nombre} para vender.";
        }
    }

    public string SellLastItemAdded()
    {
        if (inventario.Count == 0)
        {
            return "No tienes objetos en el inventario para vender.";
        }

        // Obtener el último item del inventario
        PlayerItem ultimoItem = inventario[inventario.Count - 1];
        ItemData baseItem = GameManager.Instance.baseDeDatos.Find(i => i.nombre == ultimoItem.nombre);

        if (baseItem != null)
        {
            string nombreItem = ultimoItem.nombre;
            int cantidadVendida = ultimoItem.cantidad;

            // Remover el item del inventario
            inventario.RemoveAt(inventario.Count - 1);

            float factor = 1f;
            switch (baseItem.rareza)
            {
                case 1:
                    factor = Random.Range(0.65f, 0.75f);
                    break;
                case 2:
                    factor = Random.Range(0.80f, 0.90f);
                    break;
                case 3:
                    factor = Random.Range(0.95f, 1.05f);
                    break;
            }

            float precioVenta = baseItem.precio * factor;
            float ganancia = precioVenta * cantidadVendida;
            dinero += ganancia;

            // Notificar si vendió item de misión de entregas
            Entregas.OnItemVendido(nombreItem);

            ActualizarUIStats();
            return $"Vendiste {cantidadVendida} {nombreItem}(s) por {ganancia:F1} monedas (rareza {baseItem.rareza}, factor {factor:P0}).";
        }
        else
        {
            return "Error: No se encontró la información del item en la base de datos.";
        }
    }
    public string BuyFromShop(string nombre, int cantidad)
    {
        ItemData baseItem = GameManager.Instance.baseDeDatos.Find(i => i.nombre == nombre);

        if (baseItem != null)
        {
            float costo = baseItem.precio * cantidad;
            if (dinero >= costo)
            {
                dinero -= costo;
                AddToInv(nombre, cantidad);
                ActualizarUIStats();
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

    public string RemoveFromInv(string nombre, int cantidad)
    {
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

    public string PayMoney(float cantidad)
    {
        if (dinero >= cantidad)
        {
            dinero -= cantidad;
            ActualizarUIStats();
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
        ActualizarUIStats();
        return $"Recibiste {cantidad} monedas. Dinero total: {dinero:F1}.";
    }

    public string AddReputation(int cantidad)
    {
        reputacion += cantidad;
        ActualizarUIStats();
        return $"Tu reputación aumentó en {cantidad}. Reputación actual: {reputacion}.";
    }

    public string LoseReputation(int cantidad)
    {
        reputacion -= cantidad;
        if (reputacion < 0) reputacion = 0;
        ActualizarUIStats();
        return $"Perdiste {cantidad} de reputación. Reputación actual: {reputacion}.";
    }

    void ActualizarUIStats()
    {
        textoStats.text = "";
        if (textoStats != null)
        {
            if(Input.GetKey(KeyCode.I))
            {
                textoStats.text += $"Dinero: ${dinero:F0}\t\tReputación: {reputacion} \n";
                foreach (var item in inventario)
                {
                    if (item.cantidad > 0) 
                    {
                        textoStats.text += item.nombre + " x" + item.cantidad + "\n";
                    }
                }
                
            }else{
                textoStats.text = $"Dinero: ${dinero:F0}\t\tReputación: {reputacion} \n Presiona I para ver el inventario";
            }
            
        }
    }

 

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        Vector3 velocity = move * walkSpeed;
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
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
        if (Input.GetKeyDown(KeyCode.O))
        {
            EjecutarAccionO();
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
            if (currentInteractable != null)
            {
                MostrarPanelInteraccion(currentInteractable.TextoInteraccion);
                Debug.Log("Presiona E para " + currentInteractable.TextoInteraccion);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Trigger"))
        {
            if (currentInteractable != null)
            {
                currentInteractable = null;
                OcultarPanelInteraccion();
                Debug.Log("Saliste del área de interacción.");
            }
        }
    }

    void MostrarPanelInteraccion(string texto)
    {
        if (panelInteraccion != null)
        {
            panelInteraccion.SetActive(true);
            
            if (textoInteraccion != null)
            {
                textoInteraccion.text = $"[E] {texto}";
            }
        }
    }

    void OcultarPanelInteraccion()
    {
        if (panelInteraccion != null)
        {
            panelInteraccion.SetActive(false);
        }
    }

    public void LimpiarInteractuable()
    {
        currentInteractable = null;
        OcultarPanelInteraccion();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}