using UnityEngine;

public class Comprar : MonoBehaviour, IInteractuable
{
    [Header("Configuración de compra")]
    public string nombreItem = "ManzanaVerde"; 

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
            string mensaje = player.BuyFromShop(nombreItem, 1);
            Debug.Log(mensaje);
        }
        else
        {
            Debug.LogWarning("No se encontró al PlayerController en la escena.");
        }

    }
}
