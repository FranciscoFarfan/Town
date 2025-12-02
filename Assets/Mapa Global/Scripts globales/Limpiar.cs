using UnityEngine;
using TMPro;

public class Limpiar : MonoBehaviour, IInteractuable
{
    [Header("Configuración de Misión")]
    public int reputacionGanada = 4;
    public float dineroGanado = 50f;
    public int cantidadBasuraTotal = 5; // Cantidad de basura a recoger
    
    [Header("Spawn de Basura")]
    public GameObject prefabBasura;
    public Transform[] puntosSpawn; // Puntos donde aparecerá la basura
    
    [Header("UI")]
    public GameObject panelLimpiar;
    public TextMeshProUGUI textoMensaje;
    
    [HideInInspector]
    public bool misionActiva = false;
    
    private int basuraRecogida = 0;
    private int ultimoDiaLimpieza = -1;
    private PlayerController player;
    private bool panelAbierto = false;
    
    public string TextoInteraccion => "Solicitar limpieza";

    void Start()
    {
        if (panelLimpiar != null)
            panelLimpiar.SetActive(false);
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
        GameManager gm = GameManager.Instance;
        string mensaje = "";

        if (misionActiva)
        {
            mensaje = $"Misión en progreso\n\nBasura recogida: {basuraRecogida}/{cantidadBasuraTotal}\n\nRecoge toda la basura de la zona para completar la misión.";
        }
        else if (ultimoDiaLimpieza == gm.dia)
        {
            mensaje = "Ya has limpiado hoy.\n\nVuelve mañana para una nueva tarea.";
        }
        else
        {
            mensaje = $"Necesito que limpies la zona.\n\nRecoge {cantidadBasuraTotal} objetos de basura.\n\nRecompensa:\nReputación: +{reputacionGanada}\nDinero: ${dineroGanado}";
        }

        if (textoMensaje != null)
            textoMensaje.text = mensaje;
    }

    void IniciarMision()
    {
        GameManager gm = GameManager.Instance;

        if (misionActiva)
        {
            Debug.Log("La misión ya está activa.");
            ActualizarMensaje();
            return;
        }

        if (ultimoDiaLimpieza == gm.dia)
        {
            Debug.Log("Ya has limpiado hoy.");
            ActualizarMensaje();
            return;
        }

        // Iniciar misión
        misionActiva = true;
        basuraRecogida = 0;
        ultimoDiaLimpieza = gm.dia;
        
        // Spawnear basura
        SpawnearBasura();
        
        Debug.Log($"Misión iniciada: Recoge {cantidadBasuraTotal} objetos de basura.");
        
        if (textoMensaje != null)
        {
            textoMensaje.text = $"¡Misión aceptada!\n\nBasura recogida: {basuraRecogida}/{cantidadBasuraTotal}\n\nBusca y recoge la basura marcada.";
        }
        
        // Cerrar panel después de aceptar
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

        // Spawnear basura en puntos aleatorios
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

        // Verificar si se completó la misión
        if (basuraRecogida >= cantidadBasuraTotal)
        {
            CompletarMision();
        }
    }

    void CompletarMision()
    {
        misionActiva = false;

        // Dar recompensas
        string reputacionMsg = player.AddReputation(reputacionGanada);
        string dineroMsg = player.EarnMoney(dineroGanado);

        Debug.Log("¡Misión completada!");
        Debug.Log(reputacionMsg);
        Debug.Log(dineroMsg);

        // Mostrar panel de completado
        AbrirPanel();
        if (textoMensaje != null)
        {
            textoMensaje.text = $"¡Misión completada!\n\n{reputacionMsg}\n{dineroMsg}\n\n¡Buen trabajo!";
        }
        
        Invoke("CerrarPanel", 3f);
    }
}