using UnityEngine;
using TMPro;

public class Confesarse : MonoBehaviour, IInteractuable
{
    [Header("Configuración de Confesión")]
    public int reputacionGanada = 1;
    
    [Header("Límite Diario")]
    public bool usarLimiteDiario = true;
    public int confesionesMaxPorDia = 1;
    
    [Header("UI")]
    public GameObject panelConfesion;
    public TextMeshProUGUI textoMensaje;
    
    [Header("Animación")]
    public Animator npcAnimator;
    public string nombreAnimacionPersinar = "Persinar";
    
    private PlayerController player;
    private bool panelAbierto = false;
    
    private int ultimoDiaConfesion = -1;
    private int confesionesHoy = 0;
    
    public string TextoInteraccion => "Confesarse";

    void Start()
    {
        if (panelConfesion != null)
            panelConfesion.SetActive(false);
            
        if (npcAnimator == null)
            npcAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        if (usarLimiteDiario)
        {
            VerificarNuevoDia();
        }
        
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

    void VerificarNuevoDia()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null) return;
        
        int diaActual = gm.dia;
        
        if (diaActual != ultimoDiaConfesion)
        {
            confesionesHoy = 0;
            ultimoDiaConfesion = diaActual;
            Debug.Log($"[Confesarse] Nuevo día detectado. Confesiones disponibles: {confesionesMaxPorDia}");
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
        GameManager gm = GameManager.Instance;

        if (usarLimiteDiario && confesionesHoy >= confesionesMaxPorDia)
        {
            int horasRestantes = Mathf.CeilToInt(24f - gm.hora);
            mensaje = $"Ya te has confesado hoy.\n\nConfesiones hoy: {confesionesHoy}/{confesionesMaxPorDia}\n\nVuelve en ~{horasRestantes} hora(s).\nDía actual: {gm.dia}";
        }
        else if (player.reputacion >= 0)
        {
            mensaje = $"Tu reputación es buena ({player.reputacion}).\n\nNo necesitas confesarte en este momento.";
            
            if (usarLimiteDiario)
            {
                mensaje += $"\n\nConfesiones hoy: {confesionesHoy}/{confesionesMaxPorDia}";
            }
        }
        else
        {
            mensaje = $"Tu reputación actual es: {player.reputacion}\n\n¿Deseas confesarte?\n\nReputación: +{reputacionGanada}";
            
            if (usarLimiteDiario)
            {
                mensaje += $"\n\nConfesiones disponibles:\n{confesionesMaxPorDia - confesionesHoy}/{confesionesMaxPorDia}";
            }
        }

        if (textoMensaje != null)
            textoMensaje.text = mensaje;
    }

    void RealizarConfesion()
    {
        GameManager gm = GameManager.Instance;
        
        if (usarLimiteDiario && confesionesHoy >= confesionesMaxPorDia)
        {
            Debug.Log("Ya alcanzaste el límite de confesiones por hoy.");
            ActualizarMensaje();
            return;
        }
        
        if (player.reputacion >= 0)
        {
            Debug.Log("No necesitas confesarte.");
            ActualizarMensaje();
            return;
        }

        string resultado = player.AddReputation(reputacionGanada);
        
        if (usarLimiteDiario)
        {
            confesionesHoy++;
            Debug.Log($"[Confesarse] Confesiones realizadas hoy: {confesionesHoy}/{confesionesMaxPorDia}");
        }
        
        Debug.Log("Te has confesado. " + resultado);

        if (textoMensaje != null)
        {
            string mensajeConfesion = $"Has confesado tus pecados.\n\n{resultado}\n\nQue la paz esté contigo.";
            
            if (usarLimiteDiario)
            {
                int restantes = confesionesMaxPorDia - confesionesHoy;
                if (restantes > 0)
                {
                    mensajeConfesion += $"\n\nConfesiones restantes hoy: {restantes}";
                }
                else
                {
                    mensajeConfesion += "\n\n(No puedes confesarte más hoy)";
                }
            }
            
            textoMensaje.text = mensajeConfesion;
        }
        
        ActivarAnimacionPersinar();
    }
    
    void ActivarAnimacionPersinar()
    {
        if (npcAnimator == null)
        {
            Debug.LogWarning("No hay Animator asignado para el NPC.");
            return;
        }
        
        npcAnimator.SetTrigger(nombreAnimacionPersinar);
        
        Debug.Log($"Animación {nombreAnimacionPersinar} activada.");
    }
    
    public string ObtenerEstadoConfesiones()
    {
        if (!usarLimiteDiario)
            return "Sin límite diario";
            
        return $"Confesiones: {confesionesHoy}/{confesionesMaxPorDia} | Día: {ultimoDiaConfesion}";
    }
}