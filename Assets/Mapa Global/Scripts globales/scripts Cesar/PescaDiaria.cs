using UnityEngine;
using TMPro;
using System.Collections;

public class PescaDiaria : MonoBehaviour, IInteractuable
{
    [Header("Configuración de Pesca")]
    public string nombrePez = "Pez";
    
    [Header("UI")]
    public GameObject panelPesca;
    public TextMeshProUGUI textoMensaje;
    
    private int ultimoDiaPesca = -1;
    private PlayerController player;
    private bool pescando = false;
    
    public string TextoInteraccion => "Pescar";

    void Start()
    {
        if (panelPesca != null)
            panelPesca.SetActive(false);
    }

    public void Interaccion()
    {
        player = FindObjectOfType<PlayerController>();
        GameManager gm = GameManager.Instance;

        if (player == null || gm == null)
        {
            Debug.LogWarning("No se encontró PlayerController o GameManager.");
            return;
        }

        // Verificar si ya pescó hoy
        if (ultimoDiaPesca == gm.dia)
        {
            StartCoroutine(MostrarMensaje("Ya has pescado hoy.\n\nVuelve mañana para pescar de nuevo.", 2f));
            return;
        }

        // Si no ha pescado hoy, comenzar a pescar
        if (!pescando)
        {
            StartCoroutine(RealizarPesca());
        }
    }

    IEnumerator RealizarPesca()
    {
        GameManager gm = GameManager.Instance;
        pescando = true;

        // Desactivar controles del jugador
        if (player != null)
            player.enabled = false;

        // Mostrar panel
        if (panelPesca != null)
            panelPesca.SetActive(true);

        // Animación de pesca
        if (textoMensaje != null)
        {
            yield return MostrarTextoConEspera("Lanzando la caña", 1f);
            yield return MostrarTextoConEspera("Lanzando la caña.", 1f);
            yield return MostrarTextoConEspera("Lanzando la caña..", 1f);
            yield return MostrarTextoConEspera("Lanzando la caña...", 1f);
            yield return MostrarTextoConEspera("¡Algo picó!", 0.5f);
        }

        // Registrar el día de pesca
        ultimoDiaPesca = gm.dia;

        // Buscar el pez en la base de datos y spawnearlo
        ItemData pezData = gm.baseDeDatos.Find(item => item.nombre == nombrePez);
        
        if (pezData != null)
        {
            
            
            if (textoMensaje != null)
            {
                textoMensaje.text = $"¡Has pescado un {nombrePez}.";
            }
            
            Debug.Log($"Pescaste un {nombrePez}");
        }
        else
        {
            Debug.LogWarning($"No se encontró '{nombrePez}' en la base de datos del GameManager.");
            if (textoMensaje != null)
            {
                textoMensaje.text = "Error: Pez no encontrado.";
            }
        }

        // Esperar un poco antes de cerrar
        yield return new WaitForSeconds(2f);

        // Cerrar panel y reactivar controles
        if (panelPesca != null)
            panelPesca.SetActive(false);

        if (player != null)
            player.enabled = true;

        pescando = false;
    }

    IEnumerator MostrarTextoConEspera(string texto, float tiempo)
    {
        if (textoMensaje != null)
            textoMensaje.text = texto;
        
        yield return new WaitForSeconds(tiempo);
    }

    IEnumerator MostrarMensaje(string mensaje, float duracion)
    {
        if (panelPesca != null && textoMensaje != null)
        {
            if (player != null)
                player.enabled = false;

            panelPesca.SetActive(true);
            textoMensaje.text = mensaje;
            
            yield return new WaitForSeconds(duracion);
            
            panelPesca.SetActive(false);

            if (player != null)
                player.enabled = true;
        }
    }
}