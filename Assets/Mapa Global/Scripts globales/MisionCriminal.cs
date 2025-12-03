using UnityEngine;
using TMPro;

public class MisionCriminal : MonoBehaviour, IInteractuable
{
    [Header("Tipo de NPC")]
    public TipoNPC tipoNPC;
    
    [Header("Configuración")]
    public string itemRobar = "Joyas";
    public string nombreArma = "Pistola";
    public GameObject npcObjetivo; // El NPC que debe ser eliminado
    
    [Header("UI")]
    public GameObject panelMision;
    public TextMeshProUGUI textoMensaje;
    
    // Estados globales de las misiones
    private static bool misionRoboCompletada = false;
    private static bool misionRoboActiva = false;
    private static bool misionMatarCompletada = false;
    private static bool misionMatarActiva = false;
    private static bool misionRecuperarCompletada = false;
    private static bool misionRecuperarActiva = false;
    private static string itemRobado = "";
    
    private PlayerController player;
    private bool panelAbierto = false;
    
    public enum TipoNPC
    {
        JefeCriminal,      // Da la misión de robar a la anciana
        Anciana,           // Víctima del robo inicial / Da misión de recuperar
        AsesinoContratante, // Da la misión de matar
        Objetivo           // El NPC que debe ser eliminado
    }
    
    public string TextoInteraccion
    {
        get
        {
            switch (tipoNPC)
            {
                case TipoNPC.JefeCriminal:
                    return "Trabajo sucio";
                case TipoNPC.Anciana:
                    if (misionRecuperarActiva || misionRecuperarCompletada)
                        return "Hablar con la anciana";
                    return "Robar";
                case TipoNPC.AsesinoContratante:
                    return "Trabajo peligroso";
                case TipoNPC.Objetivo:
                    return "Eliminar";
                default:
                    return "Interactuar";
            }
        }
    }

    void Start()
    {
        if (panelMision != null)
            panelMision.SetActive(false);
    }

    void Update()
    {
        if (panelAbierto)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                RealizarAccion();
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

        if (player == null)
        {
            Debug.LogWarning("No se encontró PlayerController.");
            return;
        }

        AbrirPanel();
        ActualizarMensaje();
    }

    void AbrirPanel()
    {
        if (panelMision != null)
        {
            panelMision.SetActive(true);
            panelAbierto = true;
            
            if (player != null)
                player.enabled = false;
        }
    }

    void CerrarPanel()
    {
        if (panelMision != null)
        {
            panelMision.SetActive(false);
            panelAbierto = false;
            
            if (player != null)
                player.enabled = true;
        }
    }

    void ActualizarMensaje()
    {
        string mensaje = "";

        switch (tipoNPC)
        {
            case TipoNPC.JefeCriminal:
                if (misionRoboCompletada)
                {
                    mensaje = "Buen trabajo con la anciana.\n\nAhora espera nuevas órdenes.";
                }
                else if (misionRoboActiva)
                {
                    mensaje = $"Ve y róbale las {itemRobar} a la anciana.\n\nNo falles.";
                }
                else
                {
                    mensaje = $"Necesito que le robes unas {itemRobar} a una anciana.\n\nReputación: -2";
                }
                break;

            case TipoNPC.Anciana:
                if (misionRecuperarCompletada)
                {
                    mensaje = "Gracias por recuperar mis joyas.\n\n¡Eres un héroe!";
                }
                else if (misionRecuperarActiva)
                {
                    mensaje = "¿Recuperaste mis joyas?\n\nPor favor, devuélvemelas.";
                }
                else if (misionRoboCompletada)
                {
                    mensaje = "¡Me robaste! Pero sé quién tiene mis joyas ahora.\n\n¿Podrías recuperarlas?\n\nReputación: +5\nDinero: $100";
                }
                else if (misionRoboActiva)
                {
                    mensaje = $"¿Qué haces aquí?\n\nRobar: {itemRobar}\nReputación: -2";
                }
                else
                {
                    mensaje = "Hola joven, ¿necesitas algo?";
                }
                break;

            case TipoNPC.AsesinoContratante:
                if (misionMatarCompletada)
                {
                    mensaje = "Trabajo limpio.\n\nNo vuelvas a hablarme.";
                }
                else if (misionMatarActiva)
                {
                    mensaje = $"Elimina al objetivo.\n\nUsa el arma que te di.";
                }
                else if (misionRoboCompletada)
                {
                    mensaje = $"Veo que completaste tu primer trabajo.\n\nTengo algo más... permanente.\n\nElimina a alguien que me debe dinero.\n\nRecompensa: {nombreArma}";
                }
                else
                {
                    mensaje = "No tengo nada para ti todavía.";
                }
                break;

            case TipoNPC.Objetivo:
                if (misionMatarCompletada)
                {
                    mensaje = "Ya está eliminado.";
                }
                else if (misionRecuperarActiva && TieneArma())
                {
                    mensaje = $"Tengo las {itemRobado}.\n\n¿Vienes por ellas?\n\nEliminar objetivo";
                }
                else if (misionMatarActiva && TieneArma())
                {
                    mensaje = "¿Qué quieres?\n\nEliminar";
                }
                else
                {
                    mensaje = "No tengo nada que decirte.";
                }
                break;
        }

        if (textoMensaje != null)
            textoMensaje.text = mensaje;
    }

    void RealizarAccion()
    {
        switch (tipoNPC)
        {
            case TipoNPC.JefeCriminal:
                AceptarMisionRobo();
                break;

            case TipoNPC.Anciana:
                if (misionRecuperarActiva)
                    EntregarJoyas();
                else if (misionRoboActiva)
                    RobarAnciana();
                else if (misionRoboCompletada && !misionRecuperarActiva)
                    AceptarMisionRecuperar();
                break;

            case TipoNPC.AsesinoContratante:
                AceptarMisionMatar();
                break;

            case TipoNPC.Objetivo:
                EliminarObjetivo();
                break;
        }
    }

    // ========== MISIÓN 1: ROBAR A LA ANCIANA ==========
    void AceptarMisionRobo()
    {
        if (misionRoboCompletada || misionRoboActiva)
        {
            ActualizarMensaje();
            return;
        }

        misionRoboActiva = true;
        itemRobado = itemRobar;
        
        Debug.Log($"Misión aceptada: Roba las {itemRobar} a la anciana.");

        if (textoMensaje != null)
        {
            textoMensaje.text = $"Misión aceptada.\n\nRoba las {itemRobar} a la anciana.";
        }

        Invoke("CerrarPanel", 2f);
    }

    void RobarAnciana()
    {
        if (!misionRoboActiva || misionRoboCompletada)
        {
            ActualizarMensaje();
            return;
        }

        player.AddToInv(itemRobado, 1);
        player.LoseReputation(2);

        Debug.Log($"Robaste {itemRobado} a la anciana. -2 Reputación");

        if (textoMensaje != null)
        {
            textoMensaje.text = $"Robaste: {itemRobado}\n\nReputación: -2\n\nAhora puedes venderlo o continuar con más misiones.";
        }

        misionRoboCompletada = true;
        misionRoboActiva = false;

        Invoke("CerrarPanel", 3f);
    }

    // ========== MISIÓN 2: MATAR AL OBJETIVO ==========
    void AceptarMisionMatar()
    {
        if (!misionRoboCompletada || misionMatarActiva || misionMatarCompletada)
        {
            ActualizarMensaje();
            return;
        }

        misionMatarActiva = true;
        
        // Dar el arma al jugador
        player.AddToInv(nombreArma, 1);

        Debug.Log($"Misión de asesinato aceptada. Recibiste: {nombreArma}");

        if (textoMensaje != null)
        {
            textoMensaje.text = $"Misión aceptada.\n\nRecibiste: {nombreArma}\n\nElimina al objetivo.";
        }

        Invoke("CerrarPanel", 3f);
    }

    void EliminarObjetivo()
    {
        // Para misión de recuperar
        if (misionRecuperarActiva && TieneArma())
        {
            if (!TieneItem(itemRobado))
            {
                // El objetivo tiene las joyas
                player.AddToInv(itemRobado, 1);
                Debug.Log($"Eliminaste al objetivo y recuperaste las {itemRobado}");

                if (textoMensaje != null)
                {
                    textoMensaje.text = $"Objetivo eliminado.\n\nRecuperaste: {itemRobado}\n\nDevuélvelas a la anciana.";
                }
            }

            if (npcObjetivo != null)
            {
                Destroy(npcObjetivo);
            }

            Invoke("CerrarPanel", 3f);
            return;
        }

        // Para misión de matar
        if (!misionMatarActiva || misionMatarCompletada || !TieneArma())
        {
            ActualizarMensaje();
            return;
        }

        Debug.Log("Objetivo eliminado.");

        if (textoMensaje != null)
        {
            textoMensaje.text = "Objetivo eliminado.\n\nMisión completada.";
        }

        if (npcObjetivo != null)
        {
            Destroy(npcObjetivo);
        }

        misionMatarCompletada = true;
        misionMatarActiva = false;

        Invoke("CerrarPanel", 3f);
    }

    // ========== MISIÓN 3: RECUPERAR LAS JOYAS ==========
    void AceptarMisionRecuperar()
    {
        if (!misionMatarCompletada || misionRecuperarActiva || misionRecuperarCompletada)
        {
            ActualizarMensaje();
            return;
        }

        misionRecuperarActiva = true;

        Debug.Log("Misión de recuperación aceptada.");

        if (textoMensaje != null)
        {
            textoMensaje.text = $"Misión aceptada.\n\nRecupera mis {itemRobado}.\n\nEl objetivo las tiene.";
        }

        Invoke("CerrarPanel", 2f);
    }

    void EntregarJoyas()
    {
        if (!misionRecuperarActiva || misionRecuperarCompletada)
        {
            ActualizarMensaje();
            return;
        }

        if (!TieneItem(itemRobado))
        {
            if (textoMensaje != null)
            {
                textoMensaje.text = $"¡No tienes mis {itemRobado}!\n\nVe a recuperarlas.";
            }
            Invoke("CerrarPanel", 2f);
            return;
        }

        player.RemoveFromInv(itemRobado, 1);
        player.AddReputation(5);
        player.EarnMoney(100f);

        Debug.Log("Joyas devueltas. +5 Reputación, +$100");

        if (textoMensaje != null)
        {
            textoMensaje.text = $"¡Gracias!\n\nReputación: +5\nDinero: +$100\n\n¡Eres un héroe!";
        }

        misionRecuperarCompletada = true;
        misionRecuperarActiva = false;

        Invoke("CerrarPanel", 3f);
    }

    // ========== MÉTODOS AUXILIARES ==========
    bool TieneArma()
    {
        PlayerItem arma = player.inventario.Find(i => i.nombre == nombreArma);
        return arma != null && arma.cantidad > 0;
    }

    bool TieneItem(string nombreItem)
    {
        PlayerItem item = player.inventario.Find(i => i.nombre == nombreItem);
        return item != null && item.cantidad > 0;
    }

    // Método para resetear todas las misiones (útil para testing)
    public static void ResetearMisiones()
    {
        misionRoboCompletada = false;
        misionRoboActiva = false;
        misionMatarCompletada = false;
        misionMatarActiva = false;
        misionRecuperarCompletada = false;
        misionRecuperarActiva = false;
        itemRobado = "";
    }
}