using UnityEngine;
using TMPro;

public class Confesarse : MonoBehaviour, IInteractuable
{
    [Header("Configuración de Confesión")]
    public int reputacionGanada = 1;
    
    [Header("UI")]
    public GameObject panelConfesion;
    public TextMeshProUGUI textoMensaje;
    
    private int ultimoDiaConfesion = -1;
    private PlayerController player;
    private bool panelAbierto = false;
    
    public string TextoInteraccion => "Confesarse";

    void Start()
    {
        // Ocultar panel al inicio
        if (panelConfesion != null)
            panelConfesion.SetActive(false);
    }

    void Update()
    {
        // Controles con teclado cuando el panel está abierto
        if (panelAbierto)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                RealizarConfesion();
            }
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                CerrarPanel();
            }
        }
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

        // Abrir panel y mostrar mensaje apropiado
        AbrirPanel();
        ActualizarMensaje();
    }

    void AbrirPanel()
    {
        if (panelConfesion != null)
        {
            panelConfesion.SetActive(true);
            panelAbierto = true;
            
            // Desactivar controles del jugador
            if (player != null)
                player.enabled = false;
        }
    }

    void CerrarPanel()
    {
        if (panelConfesion != null)
        {
            panelConfesion.SetActive(false);
            panelAbierto = false;
            
            // Reactivar controles del jugador
            if (player != null)
                player.enabled = true;
        }
    }

    void ActualizarMensaje()
    {
        GameManager gm = GameManager.Instance;
        string mensaje = "";

        // Verificar si ya se confesó hoy
        if (ultimoDiaConfesion == gm.dia)
        {
            mensaje = "Ya te has confesado hoy.\n\nVuelve mañana si necesitas purificar tu alma nuevamente.";
        }
        // Verificar si tiene reputación negativa
        else if (player.reputacion >= 0)
        {
            mensaje = $"Tu reputación es buena ({player.reputacion}).\n\nNo necesitas confesarte en este momento.";
        }
        else
        {
            mensaje = $"Tu reputación actual es: {player.reputacion}\n\n¿Deseas confesarte?\n\nReputación: +{reputacionGanada}";
        }

        if (textoMensaje != null)
            textoMensaje.text = mensaje;
    }

    void RealizarConfesion()
    {
        GameManager gm = GameManager.Instance;

        // Verificar si puede confesarse
        if (ultimoDiaConfesion == gm.dia)
        {
            Debug.Log("Ya te has confesado hoy.");
            ActualizarMensaje(); // Recargar el panel con el mensaje actualizado
            return;
        }

        if (player.reputacion >= 0)
        {
            Debug.Log("No necesitas confesarte.");
            ActualizarMensaje(); // Recargar el panel con el mensaje actualizado
            return;
        }

        // Realizar la confesión
        ultimoDiaConfesion = gm.dia;
        string resultado = player.AddReputation(reputacionGanada);
        
        Debug.Log("Te has confesado. " + resultado);

        // Actualizar mensaje
        if (textoMensaje != null)
        {
            textoMensaje.text = $"Has confesado tus pecados.\n\n{resultado}\n\nQue la paz esté contigo.";
        }
    }
}