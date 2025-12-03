using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Arrastra aquí el Transform del jugador")]
    public Transform player;
    
    [Header("Configuración de Detección")]
    [Tooltip("Distancia a la que el personaje detecta al jugador")]
    [Range(1f, 20f)]
    public float detectionDistance = 5f;
    
    [Tooltip("Velocidad de rotación de la cabeza")]
    [Range(1f, 10f)]
    public float lookSpeed = 3f;
    
    [Header("Configuración de Mirada Aleatoria")]
    [Tooltip("Tiempo mínimo antes de cambiar de dirección aleatoria")]
    [Range(1f, 5f)]
    public float minRandomLookTime = 2f;
    
    [Tooltip("Tiempo máximo antes de cambiar de dirección aleatoria")]
    [Range(3f, 10f)]
    public float maxRandomLookTime = 5f;
    
    [Tooltip("Rango de valores aleatorios para horizontal (-1 a 1)")]
    [Range(0.3f, 1f)]
    public float randomHorizontalRange = 0.8f;
    
    [Tooltip("Rango de valores aleatorios para vertical (-1 a 1)")]
    [Range(0.3f, 1f)]
    public float randomVerticalRange = 0.5f;
    
    private Animator animator;
    private Vector2 targetLookDirection; // x = horizontal, y = vertical
    private float nextRandomLookTime;
    private bool isLookingAtPlayer = false;
    
    void Start()
    {
        // Obtener el componente Animator
        animator = GetComponent<Animator>();
        
        if (animator == null)
        {
            Debug.LogError("No se encontró un Animator en este GameObject!");
            enabled = false;
            return;
        }
        // Buscar automáticamente el jugador por etiqueta
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogWarning("No se encontró ningún objeto con la etiqueta 'Player'.");
            }
        }

        // Inicializar con una dirección aleatoria
        SetRandomLookDirection();
    }
    
    void Update()
    {
        if (animator == null)
            return;
        
        // Determinar si el jugador está cerca
        bool playerIsNear = false;
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            playerIsNear = distanceToPlayer <= detectionDistance;
        }
        
        // Decidir hacia dónde mirar
        if (playerIsNear)
        {
            // Mirar al jugador
            isLookingAtPlayer = true;
            CalculateLookAtPlayer();
        }
        else
        {
            // Mirar a direcciones aleatorias
            isLookingAtPlayer = false;
            
            // Verificar si es tiempo de cambiar la dirección aleatoria
            if (Time.time >= nextRandomLookTime)
            {
                SetRandomLookDirection();
            }
        }
        
        // Obtener los valores actuales del animator
        float currentHorizontal = animator.GetFloat("Head_Horizontal_f");
        float currentVertical = animator.GetFloat("Head_Vertical_f");
        
        // Interpolar suavemente hacia los nuevos valores
        float newHorizontal = Mathf.Lerp(currentHorizontal, targetLookDirection.x, Time.deltaTime * lookSpeed);
        float newVertical = Mathf.Lerp(currentVertical, targetLookDirection.y, Time.deltaTime * lookSpeed);
        
        // Actualizar los parámetros del animator
        animator.SetFloat("Head_Horizontal_f", newHorizontal);
        animator.SetFloat("Head_Vertical_f", newVertical);
    }
    
    void CalculateLookAtPlayer()
    {
        // Calcular la dirección hacia el jugador
        Vector3 directionToPlayer = player.position - transform.position;
        
        // Calcular el ángulo horizontal (izquierda/derecha)
        Vector3 flatDirection = new Vector3(directionToPlayer.x, 0, directionToPlayer.z);
        float horizontalAngle = Vector3.SignedAngle(transform.forward, flatDirection, Vector3.up);
        
        // Normalizar a -1 a 1 (el animator maneja los límites de rotación)
        // Usamos 45 grados como referencia para normalizar
        float horizontalValue = Mathf.Clamp(horizontalAngle / 45f, -1f, 1f);
        
        // Calcular el ángulo vertical (arriba/abajo)
        float distance = flatDirection.magnitude;
        float verticalAngle = Mathf.Atan2(directionToPlayer.y, distance) * Mathf.Rad2Deg;
        
        // Normalizar a -1 a 1
        float verticalValue = Mathf.Clamp(verticalAngle / 45f, -1f, 1f);
        
        targetLookDirection = new Vector2(horizontalValue, verticalValue);
    }
    
    void SetRandomLookDirection()
    {
        // Generar valores aleatorios entre -1 y 1
        float randomHorizontal = Random.Range(-randomHorizontalRange, randomHorizontalRange);
        float randomVertical = Random.Range(-randomVerticalRange, randomVerticalRange);
        
        targetLookDirection = new Vector2(randomHorizontal, randomVertical);
        
        // Programar el próximo cambio de dirección
        float randomTime = Random.Range(minRandomLookTime, maxRandomLookTime);
        nextRandomLookTime = Time.time + randomTime;
    }
    
    // Método para visualizar la detección en el editor
    void OnDrawGizmos()
    {
        // Dibujar el rango de detección
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionDistance);
        
        // Dibujar línea hacia el jugador si está cerca
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance <= detectionDistance)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position + Vector3.up * 1.5f, player.position);
            }
            else
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(transform.position + Vector3.up * 1.5f, player.position);
            }
        }
    }
}
