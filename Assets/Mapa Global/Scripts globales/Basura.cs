using UnityEngine;

public class Basura : MonoBehaviour, IInteractuable
{
    public string TextoInteraccion => "Recoger basura";
    
    private Limpiar misionLimpiar;

    void Start()
    {
        // Buscar la misión de limpieza activa
        misionLimpiar = FindObjectOfType<Limpiar>();
    }

    public void Interaccion()
    {
        Debug.Log("=== DIAGNÓSTICO BASURA ===");
        Debug.Log($"misionLimpiar encontrada: {misionLimpiar != null}");
        
        if (misionLimpiar != null)
        {
            Debug.Log($"Misión activa: {misionLimpiar.misionActiva}");
        }
        
        if (misionLimpiar != null && misionLimpiar.misionActiva)
        {
            // Notificar a la misión que se recogió basura
            misionLimpiar.RecogerBasura();
            
            Debug.Log("¡Basura recogida! Destruyendo objeto...");
            
            // Limpiar referencia del PlayerController
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.LimpiarInteractuable();
            }
            
            // Destruir el objeto de basura
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("❌ No hay ninguna misión de limpieza activa o no se encontró el script Limpiar.");
        }
    }
}