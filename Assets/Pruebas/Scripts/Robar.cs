using UnityEngine;
using TMPro;

public class Robar : MonoBehaviour, IInteractuable
{
    [Header("Configuración")]
    public bool esMisionInicial = false; // true = NPC que da la misión de robar a la anciana
    public bool esAnciana = false; // true = es la anciana que será robada
    public string itemRobar = "Joyas"; // Item específico para la anciana
    
    [Header("UI")]
    public GameObject panelRobar;
    public TextMeshProUGUI textoMensaje;
    
    private static int ultimoDiaRobo = -1;
    private static bool misionAncianaCompletada = false;
    private static bool misionAncianaActiva = false;
    
    private PlayerController player;
    private bool panelAbierto = false;
    
    public string TextoInteraccion
    {
        get
        {
            if (esMisionInicial) return "Trabajo sospechoso";
            if (esAnciana) return "Robar";
            return "Robar";
        }
    }

    void Start()
    {
        if (panelRobar != null)
            panelRobar.SetActive(false);
    }

    void Update()
    {
        if (panelAbierto)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                if (esMisionInicial)
                    AceptarMisionAnciana();
                else
                    RealizarRobo();
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
        if (panelRobar != null)
        {
            panelRobar.SetActive(true);
            panelAbierto = true;
            
            if (player != null)
                player.enabled = false;
        }
    }

    void CerrarPanel()
    {
        if (panelRobar != null)
        {
            panelRobar.SetActive(false);
            panelAbierto = false;
            
            if (player != null)
                player.enabled = true;
        }
    }

    void ActualizarMensaje()
    {
        GameManager gm = GameManager.Instance;
        string mensaje = "";

        if (esMisionInicial)
        {
            // NPC que da la misión de robar a la anciana
            if (misionAncianaCompletada)
            {
                mensaje = "Ya completaste ese trabajo.\n\nAhora puedes robar a cualquiera una vez al día.";
            }
            else if (misionAncianaActiva)
            {
                mensaje = $"Ve y róbale las {itemRobar} a la anciana.\n\nSé discreto.";
            }
            else
            {
                mensaje = $"Tengo un trabajo... delicado.\n\nNecesito que le robes unas {itemRobar} a una anciana.\n\nPenalización:\nReputación: -2";
            }
        }
        else if (esAnciana)
        {
            // La anciana
            if (misionAncianaActiva && !misionAncianaCompletada)
            {
                mensaje = $"¿Qué haces aquí?\n\nRobar: {itemRobar}\nReputación: -2";
            }
            else if (misionAncianaCompletada)
            {
                mensaje = "¡Ya me robaste! ¡Fuera de aquí!";
            }
            else
            {
                mensaje = "Hola joven, ¿necesitas algo?";
            }
        }
        else
        {
            // Persona común
            if (!misionAncianaCompletada)
            {
                mensaje = "Primero debes completar tu primera misión de robo.";
            }
            else if (ultimoDiaRobo == gm.dia)
            {
                mensaje = "Ya robaste hoy.\n\nNo llames la atención.";
            }
            else
            {
                mensaje = "Robar a esta persona\n\nObjeto: Aleatorio\nReputación: -2";
            }
        }

        if (textoMensaje != null)
            textoMensaje.text = mensaje;
    }

    void AceptarMisionAnciana()
    {
        if (misionAncianaCompletada)
        {
            Debug.Log("Ya completaste esa misión.");
            ActualizarMensaje();
            return;
        }

        if (misionAncianaActiva)
        {
            Debug.Log("La misión ya está activa.");
            ActualizarMensaje();
            return;
        }

        misionAncianaActiva = true;
        Debug.Log($"Misión aceptada: Roba las {itemRobar} a la anciana.");

        if (textoMensaje != null)
        {
            textoMensaje.text = $"Misión aceptada.\n\nVe y róbale las {itemRobar} a la anciana.\n\nSé cuidadoso.";
        }

        Invoke("CerrarPanel", 2f);
    }

    void RealizarRobo()
    {
        GameManager gm = GameManager.Instance;

        if (esAnciana)
        {
            // Robar a la anciana (misión inicial)
            if (!misionAncianaActiva || misionAncianaCompletada)
            {
                Debug.Log("No puedes robar aquí ahora.");
                ActualizarMensaje();
                return;
            }

            // Dar el item robado
            player.AddToInv(itemRobar, 1);

            // Perder reputación
            string repMsg = player.LoseReputation(2);

            Debug.Log($"Robaste {itemRobar} a la anciana.");
            Debug.Log(repMsg);

            if (textoMensaje != null)
            {
                textoMensaje.text = $"Robaste: {itemRobar}\n\n{repMsg}\n\nAhora puedes venderlo.";
            }

            misionAncianaCompletada = true;
            misionAncianaActiva = false;

            Invoke("CerrarPanel", 3f);
        }
        else
        {
            // Robar a persona común (después de completar misión anciana)
            if (!misionAncianaCompletada)
            {
                Debug.Log("Primero completa la misión inicial.");
                ActualizarMensaje();
                return;
            }

            if (ultimoDiaRobo == gm.dia)
            {
                Debug.Log("Ya robaste hoy.");
                ActualizarMensaje();
                return;
            }

            // Seleccionar item aleatorio
            if (gm.baseDeDatos.Count == 0)
            {
                Debug.LogWarning("No hay items en la base de datos.");
                return;
            }

            ItemData itemAleatorio = gm.baseDeDatos[Random.Range(0, gm.baseDeDatos.Count)];
            string itemRobado = itemAleatorio.nombre;

            // Dar item robado
            player.AddToInv(itemRobado, 1);

            // Perder reputación
            string repMsg = player.LoseReputation(2);

            // Actualizar último día de robo
            ultimoDiaRobo = gm.dia;

            Debug.Log($"Robaste {itemRobado}.");
            Debug.Log(repMsg);

            if (textoMensaje != null)
            {
                textoMensaje.text = $"Robaste: {itemRobado}\n\n{repMsg}\n\nAhora puedes venderlo.";
            }

            Invoke("CerrarPanel", 3f);
        }
    }
}