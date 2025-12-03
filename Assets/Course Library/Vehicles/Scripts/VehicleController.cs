using UnityEngine;

public class VehicleController : MonoBehaviour, IInteractuable
{
    [Header("Configuración del Vehículo")]
    public float speed = 15f;
    public float turnSpeed = 50f;
    public Transform seatPoint; // Punto donde se "sienta" el jugador (o se oculta)
    public Transform exitPoint; // Punto donde aparece el jugador al salir
    public GameObject vehicleCamera; // Cámara del vehículo (opcional, si tiene una propia)

    [Header("Referencias")]
    private PlayerController player;
    private bool isDriving = false;

    // Implementación de IInteractuable
    public string TextoInteraccion => "Conducir";

    void Start()
    {
        // Asegurarse de que la cámara del vehículo esté desactivada al inicio
        if (vehicleCamera != null)
            vehicleCamera.SetActive(false);
    }

    void Update()
    {
        if (isDriving)
        {
            HandleMovement();
            HandleExit();
        }
    }

    public void Interaccion()
    {
        if (!isDriving)
        {
            EnterVehicle();
        }
    }

    void EnterVehicle()
    {
        player = FindFirstObjectByType<PlayerController>();
        if (player == null) return;

        isDriving = true;
        
        // Actualizar GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetInCar(true);
        }

        // Desactivar control del jugador y ocultarlo
        player.controleActivo = false;
        player.GetComponent<Collider>().enabled = false; // Desactivar colisiones del jugador
        player.GetComponent<Rigidbody>().isKinematic = true; // Desactivar física del jugador
        
        // Mover jugador al asiento (o hacerlo hijo del coche para que se mueva con él)
        player.transform.SetParent(transform);
        if (seatPoint != null)
        {
            player.transform.position = seatPoint.position;
            player.transform.rotation = seatPoint.rotation;
        }
        else
        {
            player.transform.localPosition = Vector3.zero; // Fallback
        }
        
        // Ocultar visualmente al jugador (opcional, si el coche es cerrado)
        // player.gameObject.SetActive(false); // Cuidado: si desactivas el objeto, los scripts no corren. Mejor desactivar el render o moverlo.
        // En este caso, lo mantendremos activo pero "dentro" y sin control.
        
        // Cambiar cámaras
        if (vehicleCamera != null)
        {
            vehicleCamera.SetActive(true);
            player.cameraTransform.gameObject.SetActive(false); // Desactivar cámara del jugador
        }
        else
        {
            // Si no hay cámara de vehículo, podríamos mover la cámara del jugador
            // Pero por ahora asumimos que se asignará una cámara en el inspector
        }

        player.LimpiarInteractuable(); // Limpiar UI
    }

    void HandleExit()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ExitVehicle();
        }
    }

    void ExitVehicle()
    {
        isDriving = false;

        // Actualizar GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetInCar(false);
        }

        // Restaurar jugador
        player.transform.SetParent(null); // Desemparentar
        
        if (exitPoint != null)
        {
            player.transform.position = exitPoint.position;
            player.transform.rotation = exitPoint.rotation;
        }
        else
        {
            player.transform.position = transform.position + transform.right * 2f; // Salir a un lado
        }

        player.controleActivo = true;
        player.GetComponent<Collider>().enabled = true;
        player.GetComponent<Rigidbody>().isKinematic = false;

        // Restaurar cámaras
        if (vehicleCamera != null)
        {
            vehicleCamera.SetActive(false);
            player.cameraTransform.gameObject.SetActive(true);
        }

        player = null;
    }

    void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Mover hacia adelante/atrás
        transform.Translate(Vector3.forward * Time.deltaTime * speed * verticalInput);

        // Rotar (solo si se mueve)
        if (Mathf.Abs(verticalInput) > 0.1f)
        {
             // Invertir rotación si va marcha atrás para sensación más natural
            float direction = verticalInput > 0 ? 1 : -1;
            transform.Rotate(Vector3.up * Time.deltaTime * turnSpeed * horizontalInput * direction);
        }
    }
}
