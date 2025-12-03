using UnityEngine;
using TMPro;

public class MisionCriminal : MonoBehaviour, IInteractuable
{
    [Header("Tipo de NPC")]
    public TipoNPC tipoNPC;
    
    [Header("Configuración")]
    public string itemRobar = "Joyas";
    public string nombreArma = "Pistola";
    
    [Header("UI")]
    public GameObject panelMision;
    public TextMeshProUGUI textoMensaje;
    
    [Header("Animación")]
    public Animator npcAnimator;
    public string nombreAnimacionReaccion = "Reaccion";
    public string nombreParametroMuerte = "Muerto";
    public float tiempoAntesDeDestruir = 3f;
    
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
    private bool estaMuerto = false;
    private RigidbodyConstraints constraintsOriginales; // Guardar las constraints originales
    
    public enum TipoNPC
    {
        JefeCriminal,
        Anciana,
        AsesinoContratante,
        Objetivo,
        PersonaComun
    }
    
    public string TextoInteraccion
    {
        get
        {
            if (estaMuerto) return "";
            
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
                case TipoNPC.PersonaComun:
                    return misionRoboCompletada ? "Robar" : "Hablar";
                default:
                    return "Interactuar";
            }
        }
    }

    void Start()
    {
        if (panelMision != null)
            panelMision.SetActive(false);
            
        if (npcAnimator == null)
            npcAnimator = GetComponent<Animator>();
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
        if (estaMuerto)
        {
            Debug.Log("Este NPC está muerto.");
            return;
        }
        
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
            {
                Rigidbody playerRb = player.GetComponent<Rigidbody>();
                if (playerRb != null)
                {
                    // Guardar las constraints originales
                    constraintsOriginales = playerRb.constraints;
                    
                    // Resetear velocidades
                    playerRb.linearVelocity = Vector3.zero;
                    playerRb.angularVelocity = Vector3.zero;
                    
                    // Congelar todas las posiciones para evitar cualquier movimiento
                    playerRb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                }
                player.enabled = false;
            }
        }
    }

    void CerrarPanel()
    {
        if (panelMision != null)
        {
            panelMision.SetActive(false);
            panelAbierto = false;
            
            if (player != null)
            {
                Rigidbody playerRb = player.GetComponent<Rigidbody>();
                if (playerRb != null)
                {
                    // Resetear velocidades otra vez por seguridad
                    playerRb.linearVelocity = Vector3.zero;
                    playerRb.angularVelocity = Vector3.zero;
                    
                    // Restaurar las constraints originales
                    playerRb.constraints = constraintsOriginales;
                }
                player.enabled = true;
            }
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
                    mensaje = "Buen trabajo con la anciana.\nAhora espera nuevas órdenes.";
                }
                else if (misionRoboActiva)
                {
                    mensaje = $"Ve y róbale las {itemRobar} a la anciana.\nNo falles.";
                }
                else
                {
                    mensaje = $"Necesito que le robes unas {itemRobar}\na una anciana.\n\nReputación: -2";
                }
                break;

            case TipoNPC.Anciana:
                if (misionRecuperarCompletada)
                {
                    mensaje = "Gracias por recuperar mis joyas.\n¡Eres un héroe!";
                }
                else if (misionRecuperarActiva)
                {
                    mensaje = "¿Recuperaste mis joyas?\nPor favor, devuélvemelas.";
                }
                else if (misionRoboCompletada)
                {
                    mensaje = "¡Me robaste! Pero sé quién tiene\nmis joyas ahora.\n¿Podrías recuperarlas?\n\nReputación: +5\tDinero: $100";
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
                    mensaje = "Trabajo limpio.\nNo vuelvas a hablarme.";
                }
                else if (misionMatarActiva)
                {
                    mensaje = $"Elimina al objetivo.\nUsa el arma que te di.";
                }
                else if (misionRoboCompletada)
                {
                    mensaje = $"Veo que completaste tu primer trabajo.\nTengo algo más... permanente.\n\nElimina a alguien que me debe dinero.\nRecompensa: {nombreArma}";
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
                    mensaje = $"Tengo las {itemRobado}.\n¿Vienes por ellas?\n\nEliminar objetivo";
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

            case TipoNPC.PersonaComun:
                if (!misionRoboCompletada)
                {
                    mensaje = "Hola, ¿necesitas algo?\n\n(Primero debes completar la misión\nde la anciana para robar)";
                }
                else
                {
                    mensaje = "¿En qué puedo ayudarte?\n\nRobar objeto aleatorio\nReputación: -2";
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

            case TipoNPC.PersonaComun:
                RobarPersonaComun();
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
            textoMensaje.text = $"Robaste: {itemRobado}\n\nReputación: -2\n\nAhora puedes venderlo o continuar\ncon más misiones.";
        }

        ActivarAnimacionReaccion();

        misionRoboCompletada = true;
        misionRoboActiva = false;

        Invoke("CerrarPanel", 3f);
    }

    // ========== ROBAR A PERSONAS COMUNES ==========
    void RobarPersonaComun()
    {
        if (!misionRoboCompletada)
        {
            if (textoMensaje != null)
            {
                textoMensaje.text = "Primero debes completar la misión\nde la anciana.";
            }
            Invoke("CerrarPanel", 2f);
            return;
        }

        GameManager gm = GameManager.Instance;

        if (gm.baseDeDatos.Count == 0)
        {
            Debug.LogWarning("No hay items en la base de datos.");
            return;
        }

        ItemData itemAleatorio = gm.baseDeDatos[Random.Range(0, gm.baseDeDatos.Count)];
        string itemRobadoAleatorio = itemAleatorio.nombre;

        player.AddToInv(itemRobadoAleatorio, 1);
        string repMsg = player.LoseReputation(2);

        Debug.Log($"Robaste {itemRobadoAleatorio}. {repMsg}");

        if (textoMensaje != null)
        {
            textoMensaje.text = $"Robaste: {itemRobadoAleatorio}\n\n{repMsg}\n\nAhora puedes venderlo.";
        }

        ActivarAnimacionReaccion();

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
                player.AddToInv(itemRobado, 1);
                Debug.Log($"Eliminaste al objetivo y recuperaste las {itemRobado}");

                if (textoMensaje != null)
                {
                    textoMensaje.text = $"Objetivo eliminado.\n\nRecuperaste: {itemRobado}\n\nDevuélvelas a la anciana.";
                }
            }

            Morir();
            Invoke("CerrarPanelYLimpiarPlayer", 3f);
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

        Morir();

        misionMatarCompletada = true;
        misionMatarActiva = false;

        Invoke("CerrarPanelYLimpiarPlayer", 3f);
    }

    void CerrarPanelYLimpiarPlayer()
    {
        CerrarPanel();
        
        if (player != null)
        {
            player.LimpiarInteractuable();
        }
    }

    // ========== MÉTODO PÚBLICO PARA MORIR ==========
    public void Morir()
    {
        if (estaMuerto)
        {
            Debug.Log($"{gameObject.name} ya está muerto.");
            return;
        }

        Debug.Log($"=== INICIANDO MUERTE DE {gameObject.name} ===");
        estaMuerto = true;

        if (npcAnimator == null)
        {
            npcAnimator = GetComponent<Animator>();
        }

        if (npcAnimator != null)
        {
            Debug.Log($"Animator encontrado en {gameObject.name}");

            bool parametroExiste = false;
            foreach (AnimatorControllerParameter param in npcAnimator.parameters)
            {
                if (param.name == nombreParametroMuerte)
                {
                    parametroExiste = true;
                    break;
                }
            }

            if (parametroExiste)
            {
                Debug.Log($"Activando parámetro '{nombreParametroMuerte}' en {gameObject.name}");
                npcAnimator.SetBool(nombreParametroMuerte, true);
            }
            else
            {
                Debug.LogError($"¡El parámetro '{nombreParametroMuerte}' NO existe en el Animator de {gameObject.name}!");
            }
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} NO tiene Animator, solo se desactivará.");
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
            Debug.Log($"Collider desactivado en {gameObject.name}");
        }

        Invoke("DesactivarRenderer", tiempoAntesDeDestruir);

        this.enabled = false;
        Debug.Log($"Script desactivado en {gameObject.name}");
    }

    void DesactivarRenderer()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in renderers)
        {
            rend.enabled = false;
        }
        Debug.Log($"Renderers desactivados en {gameObject.name}");
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
            textoMensaje.text = $"Misión aceptada.\n\nRecupera mis {itemRobado}.\nEl objetivo las tiene.";
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
                textoMensaje.text = $"¡No tienes mis {itemRobado}!\nVe a recuperarlas.";
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
            textoMensaje.text = $"¡Gracias!\n\nReputación: +5\tDinero: +$100\n\n¡Eres un héroe!";
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

    void ActivarAnimacionReaccion()
    {
        if (npcAnimator == null)
        {
            return;
        }
        
        npcAnimator.SetTrigger(nombreAnimacionReaccion);
        Debug.Log($"Animación {nombreAnimacionReaccion} activada.");
    }

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