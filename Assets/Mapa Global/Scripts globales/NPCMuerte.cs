using UnityEngine;

/// <summary>
/// Script opcional que se puede agregar al NPC objetivo
/// para manejar su animación de muerte de forma más limpia
/// </summary>
public class NPCMuerte : MonoBehaviour
{
    [Header("Configuración de Muerte")]
    public string parametroMuerte = "Muerto"; // Nombre del parámetro Bool en Animator
    public float tiempoDestruccion = 3f; // Tiempo antes de destruir el GameObject
    public bool desactivarCollider = true;
    public bool desactivarScripts = true;
    
    [Header("Efectos Opcionales")]
    public GameObject efectoMuerte; // Partículas, sangre, etc.
    public AudioClip sonidoMuerte;
    
    private Animator animator;
    private AudioSource audioSource;
    private bool estaMuerto = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        
        // Si no hay AudioSource pero hay sonido, agregar uno
        if (audioSource == null && sonidoMuerte != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    /// <summary>
    /// Método público para matar al NPC
    /// </summary>
    public void Morir()
    {
        if (estaMuerto) return; // Evitar llamar múltiples veces
        
        estaMuerto = true;
        
        Debug.Log($"{gameObject.name} ha sido eliminado.");
        
        // Activar animación de muerte
        if (animator != null)
        {
            animator.SetBool(parametroMuerte, true);
            Debug.Log($"Parámetro '{parametroMuerte}' activado en {gameObject.name}");
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} no tiene Animator. La animación no se reproducirá.");
        }
        
        // Reproducir sonido de muerte
        if (audioSource != null && sonidoMuerte != null)
        {
            audioSource.PlayOneShot(sonidoMuerte);
        }
        
        // Instanciar efecto de muerte (sangre, partículas, etc.)
        if (efectoMuerte != null)
        {
            Instantiate(efectoMuerte, transform.position, Quaternion.identity);
        }
        
        // Desactivar collider para que no se pueda interactuar
        if (desactivarCollider)
        {
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }
        }
        
        // Desactivar scripts de interacción
        if (desactivarScripts)
        {
            // Desactivar IInteractuable
            IInteractuable interactuable = GetComponent<IInteractuable>();
            if (interactuable != null)
            {
                MonoBehaviour mb = interactuable as MonoBehaviour;
                if (mb != null)
                {
                    mb.enabled = false;
                }
            }
            
            // Desactivar este script también
            this.enabled = false;
        }
        
        // Destruir el GameObject después del tiempo configurado
        Destroy(gameObject, tiempoDestruccion);
    }
    
    /// <summary>
    /// Método alternativo para ragdoll physics (si lo usas)
    /// </summary>
    public void MorirConRagdoll()
    {
        if (estaMuerto) return;
        
        estaMuerto = true;
        
        // Desactivar animator para usar physics
        if (animator != null)
        {
            animator.enabled = false;
        }
        
        // Activar rigidbody en todos los huesos
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = false;
        }
        
        // Aplicar fuerza (simular impacto)
        Rigidbody mainRb = GetComponent<Rigidbody>();
        if (mainRb != null)
        {
            Vector3 direccionImpacto = transform.forward;
            mainRb.AddForce(direccionImpacto * 500f);
        }
        
        // Resto de efectos
        if (audioSource != null && sonidoMuerte != null)
        {
            audioSource.PlayOneShot(sonidoMuerte);
        }
        
        if (efectoMuerte != null)
        {
            Instantiate(efectoMuerte, transform.position, Quaternion.identity);
        }
        
        if (desactivarCollider)
        {
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }
        }
        
        Destroy(gameObject, tiempoDestruccion);
    }
    
    /// <summary>
    /// Verificar si el NPC está muerto
    /// </summary>
    public bool EstaMuerto()
    {
        return estaMuerto;
    }
}