using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private bool finalActivado = false; // Para evitar que se active múltiples veces

    void OnTriggerEnter(Collider other)
    {
        if (finalActivado) return; // Ya se activó, ignorar
        
        Debug.Log($"[FinishLine] Objeto detectado: {other.gameObject.name}, Tag: {other.tag}");
        
        // Detectar si es el Player (a pie) o el vehículo
        bool esPlayer = other.CompareTag("Player");
        
        if (esPlayer)
        {
            finalActivado = true;
            Debug.Log($"[FinishLine] ¡{other.gameObject.name} cruzó la línea de meta! Activando final...");
            
            // Reproducir el final
            FinalManager finalManager = FindObjectOfType<FinalManager>();
            if (finalManager != null)
            {
                finalManager.ReproducirFinal();
            }
            else
            {
                Debug.LogError("[FinishLine] No se encontró FinalManager en la escena.");
            }
        }
    }
}
