using UnityEngine;
using TMPro;

public class Limpiar : MonoBehaviour, IInteractuable
{
    [Header("Configuración de Misión")]
    public int reputacionGanada = 4;
    public float dineroGanado = 50f;
    public int cantidadBasuraTotal = 5;
    
    [Header("Spawn de Basura")]
    public GameObject prefabBasura;
    public Transform[] puntosSpawn;
    
    [Header("UI")]
    public GameObject panelLimpiar;
    public TextMeshProUGUI textoMensaje;
    
    [Header("Animación")]
    public Animator npcAnimator; // Animator del NPC
    public string nombreAnimacionSalute = "Salute"; // Nombre del estado/trigger
    
    [HideInInspector]
    public bool misionActiva = false;
    
    private int basuraRecogida = 0;
    private PlayerController player;
    private bool panelAbierto = false;
    
    public string TextoInteraccion => "Solicitar limpieza";

    void Start()
    {
        if (panelLimpiar != null)
            panelLimpiar.SetActive(false);
            
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
                IniciarMision();
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
        if (panelLimpiar != null)
        {
            panelLimpiar.SetActive(true);
            panelAbierto = true;
            
            if (player != null)
                player.enabled = false;
        }
    }

    void CerrarPanel()
    {
        if (panelLimpiar != null)
        {
            panelLimpiar.SetActive(false);
            panelAbierto = false;
            
            if (player != null)
                player.enabled = true;
        }
    }

    void ActualizarMensaje()
    {
        string mensaje = "";

        if (misionActiva)
        {
            mensaje = $"Misión en progreso\nBasura recogida: {basuraRecogida}/{cantidadBasuraTotal}\nRecoge toda la basura de la zona para completar la misión.";
        }
        else
        {
            mensaje = $"Necesito que limpies la zona.   Recoge {cantidadBasuraTotal} objetos de basura.\nRecompensa:\nReputación: +{reputacionGanada}\tDinero: ${dineroGanado}";
        }

        if (textoMensaje != null)
            textoMensaje.text = mensaje;
    }

    void IniciarMision()
    {
        if (misionActiva)
        {
            Debug.Log("La misión ya está activa.");
            ActualizarMensaje();
            return;
        }

        misionActiva = true;
        basuraRecogida = 0;
        
        SpawnearBasura();
        
        Debug.Log($"Misión iniciada: Recoge {cantidadBasuraTotal} objetos de basura.");
        
        if (textoMensaje != null)
        {
            textoMensaje.text = $"¡Misión aceptada!\nBasura recogida: {basuraRecogida}/{cantidadBasuraTotal}\nBusca y recoge la basura marcada.";
        }
        
        // Activar animación de saludo
        ActivarAnimacionSalute();
        
        Invoke("CerrarPanel", 2f);
    }

    void SpawnearBasura()
    {
        if (prefabBasura == null)
        {
            Debug.LogWarning("No hay prefab de basura asignado.");
            return;
        }

        if (puntosSpawn == null || puntosSpawn.Length == 0)
        {
            Debug.LogWarning("No hay puntos de spawn asignados.");
            return;
        }

        for (int i = 0; i < cantidadBasuraTotal; i++)
        {
            if (i < puntosSpawn.Length && puntosSpawn[i] != null)
            {
                Instantiate(prefabBasura, puntosSpawn[i].position, Quaternion.identity);
            }
        }
    }

    public void RecogerBasura()
    {
        if (!misionActiva) return;

        basuraRecogida++;
        Debug.Log($"Basura recogida: {basuraRecogida}/{cantidadBasuraTotal}");

        if (basuraRecogida >= cantidadBasuraTotal)
        {
            CompletarMision();
        }
    }

    void CompletarMision()
    {
        misionActiva = false;

        string reputacionMsg = player.AddReputation(reputacionGanada);
        string dineroMsg = player.EarnMoney(dineroGanado);

        Debug.Log("¡Misión completada!");
        Debug.Log(reputacionMsg);
        Debug.Log(dineroMsg);

        AbrirPanel();
        if (textoMensaje != null)
        {
            textoMensaje.text = $"¡Misión completada!\n\n{reputacionMsg}\n{dineroMsg}\n\n¡Buen trabajo!";
        }
        
        // Activar animación de saludo al completar
        ActivarAnimacionSalute();
        
        Invoke("CerrarPanel", 3f);
    }
    
    void ActivarAnimacionSalute()
    {
        if (npcAnimator == null)
        {
            Debug.LogWarning("No hay Animator asignado para el NPC.");
            return;
        }
        
        // Opción 1: Si usas un Trigger
        npcAnimator.SetTrigger(nombreAnimacionSalute);
        
        // Opción 2: Si usas un Bool (descomenta si es el caso)
        // npcAnimator.SetBool(nombreAnimacionSalute, true);
        
        // Opción 3: Si quieres reproducir directamente un estado (descomenta si es el caso)
        // npcAnimator.Play(nombreAnimacionSalute);
        
        Debug.Log($"Animación {nombreAnimacionSalute} activada.");
    }
}