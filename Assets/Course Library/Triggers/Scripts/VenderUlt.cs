using UnityEngine;

public class VenderUlt : MonoBehaviour, IInteractuable
{
    [Header("Configuración de venta")]
    public string TextoInteraccion => "Vender Ultimo Item"; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interaccion()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            string mensaje = player.SellLastItemAdded();
            Debug.Log(mensaje);
        }
        else
        {
            Debug.LogWarning("No se encontró al PlayerController en la escena.");
        }

    }
}
