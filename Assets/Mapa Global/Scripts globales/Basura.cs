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
        if (misionLimpiar != null && misionLimpiar.misionActiva)
        {
            misionLimpiar.RecogerBasura();
            
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.LimpiarInteractuable();
            }
            
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("No hay ninguna misión de limpieza activa.");
        }
    }
}