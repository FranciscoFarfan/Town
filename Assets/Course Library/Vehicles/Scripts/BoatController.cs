using UnityEngine;

public class BoatController : MonoBehaviour, IInteractuable
{
    [Header("Configuración del Barco")]
    public float speed = 10f;
    public float turnSpeed = 25f;
    public Transform seatPoint; // Punto donde se "sienta" el jugador (o se oculta)
    public Transform exitPoint; // Punto donde aparece el jugador al salir
    public Transform cameraMountPoint; // Punto donde se colocará la cámara del jugador

    [Header("Referencias")]
    private PlayerController player;
    private bool isDriving = false;

    // Variables para restaurar la cámara
    private Transform originalCameraParent;
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;

    // Implementación de IInteractuable
    public string TextoInteraccion => "Conducir";

    void Start()
    {
        
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
            GameManager.Instance.SetInBoat(true);
        }

        // Desactivar control del jugador y ocultarlo
        player.controleActivo = false;
        player.GetComponent<Collider>().enabled = false; // Desactivar colisiones del jugador
        player.GetComponent<Rigidbody>().isKinematic = true; // Desactivar física del jugador
        
        // Mover jugador al asiento
        player.transform.SetParent(transform);
        if (seatPoint != null)
        {
            player.transform.position = seatPoint.position;
            player.transform.rotation = seatPoint.rotation;
        }
        else
        {
            player.transform.localPosition = Vector3.zero;
        }
        
        // Lógica de Cámara: Reparentar la cámara del jugador al vehículo
        if (player.cameraTransform != null && cameraMountPoint != null)
        {
            // Guardar estado original
            originalCameraParent = player.cameraTransform.parent;
            originalCameraPosition = player.cameraTransform.localPosition;
            originalCameraRotation = player.cameraTransform.localRotation;

            // Mover cámara al punto de montaje del vehículo
            player.cameraTransform.SetParent(cameraMountPoint);
            player.cameraTransform.localPosition = Vector3.zero;
            player.cameraTransform.localRotation = Quaternion.identity;
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
            GameManager.Instance.SetInBoat(false);
        }

        // Restaurar cámara
        if (player.cameraTransform != null && originalCameraParent != null)
        {
            player.cameraTransform.SetParent(originalCameraParent);
            player.cameraTransform.localPosition = originalCameraPosition;
            player.cameraTransform.localRotation = originalCameraRotation;
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
