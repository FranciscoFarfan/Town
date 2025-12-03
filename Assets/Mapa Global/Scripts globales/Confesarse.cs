using UnityEngine;
using TMPro;

public class Confesarse : MonoBehaviour, IInteractuable
{
    [Header("Configuración de Confesión")]
    public int reputacionGanada = 1;
    
    [Header("UI")]
    public GameObject panelConfesion;
    public TextMeshProUGUI textoMensaje;
    
    [Header("Animación")]
    public Animator npcAnimator; // Animator del NPC (cura/sacerdote)
    public string nombreAnimacionPersinar = "Persinar"; // Nombre del estado/trigger
    
    private PlayerController player;
    private bool panelAbierto = false;
    
    public string TextoInteraccion => "Confesarse";

    void Start()
    {
        if (panelConfesion != null)
            panelConfesion.SetActive(false);
            
        // Buscar el Animator automáticamente si no está asignado
        if (npcAnimator == null)
            npcAnimator = GetComponent<Animator>();
    }

    void Update()
    {
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

        AbrirPanel();
        ActualizarMensaje();
    }

    void AbrirPanel()
    {
        if (panelConfesion != null)
        {
            panelConfesion.SetActive(true);
            panelAbierto = true;
            
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
            
            if (player != null)
                player.enabled = true;
        }
    }

    void ActualizarMensaje()
    {
        string mensaje = "";

        if (player.reputacion >= 0)
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
        if (player.reputacion >= 0)
        {
            Debug.Log("No necesitas confesarte.");
            ActualizarMensaje();
            return;
        }

        string resultado = player.AddReputation(reputacionGanada);
        
        Debug.Log("Te has confesado. " + resultado);

        if (textoMensaje != null)
        {
            textoMensaje.text = $"Has confesado tus pecados.\n\n{resultado}\n\nQue la paz esté contigo.";
        }
        
        // Activar animación de persinar
        ActivarAnimacionPersinar();
    }
    
    void ActivarAnimacionPersinar()
    {
        if (npcAnimator == null)
        {
            Debug.LogWarning("No hay Animator asignado para el NPC.");
            return;
        }
        
        // Opción 1: Si usas un Trigger
        npcAnimator.SetTrigger(nombreAnimacionPersinar);
        
        // Opción 2: Si usas un Bool (descomenta si es el caso)
        // npcAnimator.SetBool(nombreAnimacionPersinar, true);
        
        // Opción 3: Si quieres reproducir directamente un estado (descomenta si es el caso)
        // npcAnimator.Play(nombreAnimacionPersinar);
        
        Debug.Log($"Animación {nombreAnimacionPersinar} activada.");
    }
}