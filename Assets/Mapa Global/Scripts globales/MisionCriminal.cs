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
    private static bool joyasEntregadasAlCriminal = false;
    private static bool misionMatarObjetivoCompletada = false;
    private static bool misionMatarObjetivoActiva = false;
    private static bool misionRecuperarJoyasCompletada = false;
    private static bool misionRecuperarJoyasActiva = false;
    private static bool criminalEliminado = false;
    private static string itemRobado = "";
    
    private PlayerController player;
    private bool panelAbierto = false;
    private bool estaMuerto = false;
    private RigidbodyConstraints constraintsOriginales;
    
    public enum TipoNPC
    {
        JefeCriminal,      // Criminal que pide robar las joyas
        Anciana,
        AsesinoContratante, // Asesino que pide matar al Objetivo
        Objetivo,          // NPC que debes matar para el asesino
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
                    if (misionRoboCompletada && !joyasEntregadasAlCriminal && TieneItem(itemRobado))
                        return "Entregar joyas";
                    if (misionRecuperarJoyasActiva && TieneArma())
                        return "Eliminar";
                    return "Trabajo sucio";
                    
                case TipoNPC.Anciana:
                    if (misionRecuperarJoyasCompletada)
                        return "Hablar";
                    if (criminalEliminado && TieneItem(itemRobado))
                        return "Devolver joyas";
                    return "Robar";
                    
                case TipoNPC.AsesinoContratante:
                    if (criminalEliminado && TieneArma())
                        return "Eliminar";
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
                    constraintsOriginales = playerRb.constraints;
                    playerRb.linearVelocity = Vector3.zero;
                    playerRb.angularVelocity = Vector3.zero;
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
                    playerRb.linearVelocity = Vector3.zero;
                    playerRb.angularVelocity = Vector3.zero;
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
                if (misionRecuperarJoyasActiva && TieneArma())
                {
                    mensaje = "Vienes por las joyas, ¿verdad?\t¿Crees que puedes matarme?\n\n[T] Eliminar";
                }
                else if (joyasEntregadasAlCriminal)
                {
                    mensaje = "Buen trabajo.\tYa puedes irte.";
                }
                else if (misionRoboCompletada && TieneItem(itemRobado))
                {
                    mensaje = $"Buen trabajo con la anciana.\tTráeme las {itemRobado}.\n\n[T] Entregar joyas";
                }
                else if (misionRoboCompletada && !TieneItem(itemRobado))
                {
                    mensaje = $"¿Dónde están las {itemRobado}?\tLas vendiste, ¿verdad?\tMal hecho.";
                }
                else if (misionRoboActiva)
                {
                    mensaje = $"Ve y róbale las {itemRobar} a la anciana.\tNo falles.";
                }
                else
                {
                    mensaje = $"Necesito que le robes unas {itemRobar} a una anciana.\n\nReputación: -2";
                }
                break;

            case TipoNPC.Anciana:
                if (misionRecuperarJoyasCompletada)
                {
                    mensaje = "¡Gracias por recuperar mis joyas!\t¡Eres un héroe!";
                }
                else if (criminalEliminado && TieneItem(itemRobado))
                {
                    mensaje = "¿Esas son mis joyas?\t¿Podrías devolvérmelas?\n\n[T] Devolver: +5 Reputación, +$100\t[R] Quedármelas";
                }
                else if (criminalEliminado && !TieneItem(itemRobado))
                {
                    mensaje = "Veo que no recuperaste mis joyas...\tQué decepción.";
                }
                else if (misionMatarObjetivoCompletada && joyasEntregadasAlCriminal)
                {
                    mensaje = $"¡Me robaste!\tPero sé quién tiene mis {itemRobado}.\tEl criminal las guarda.\n\n¿Podrías recuperarlas?\tReputación: +5\tDinero: $100";
                }
                else if (misionRoboCompletada)
                {
                    mensaje = "¡Me robaste!\tQué mala persona eres.";
                }
                else if (misionRoboActiva)
                {
                    mensaje = $"¿Qué haces aquí?\t\tRobar: {itemRobar}\tReputación: -2";
                }
                else
                {
                    mensaje = "Hola joven, ¿necesitas algo?";
                }
                break;

            case TipoNPC.AsesinoContratante:
                if (criminalEliminado && TieneArma())
                {
                    mensaje = "¿Mataste al criminal?\tMal hecho, era mi socio.\t¿Vienes por mí?\n\n[T] Eliminar";
                }
                else if (misionMatarObjetivoCompletada)
                {
                    mensaje = "Trabajo limpio.\t\tNo vuelvas a hablarme.";
                }
                else if (misionMatarObjetivoActiva)
                {
                    mensaje = $"Elimina al objetivo.\t\tUsa el arma que te di.";
                }
                else if (joyasEntregadasAlCriminal)
                {
                    mensaje = $"El criminal te recomendó.\tTengo un trabajo para ti.\n\nElimina a alguien que me debe dinero.\tRecompensa: {nombreArma}";
                }
                else
                {
                    mensaje = "No tengo nada para ti todavía.";
                }
                break;

            case TipoNPC.Objetivo:
                if (misionMatarObjetivoCompletada)
                {
                    mensaje = "Ya está eliminado.";
                }
                else if (misionMatarObjetivoActiva && TieneArma())
                {
                    mensaje = "¿Qué quieres?\t¿El asesino te mandó?\n\n[T] Eliminar";
                }
                else if (misionMatarObjetivoActiva && !TieneArma())
                {
                    mensaje = "No tengo nada que decirte.\tVete.";
                }
                else
                {
                    mensaje = "No tengo nada que decirte.";
                }
                break;

            case TipoNPC.PersonaComun:
                if (!misionRoboCompletada)
                {
                    mensaje = "Hola, ¿necesitas algo?\n\n(Primero debes completar la misión de la anciana para robar)";
                }
                else
                {
                    mensaje = "¿En qué puedo ayudarte?\n\nRobar objeto aleatorio\tReputación: -2";
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
                if (misionRecuperarJoyasActiva && TieneArma())
                    EliminarCriminal();
                else if (misionRoboCompletada && !joyasEntregadasAlCriminal && TieneItem(itemRobado))
                    EntregarJoyasAlCriminal();
                else
                    AceptarMisionRobo();
                break;

            case TipoNPC.Anciana:
                if (criminalEliminado && TieneItem(itemRobado))
                    DevolverJoyasAnciana();
                else if (misionMatarObjetivoCompletada && joyasEntregadasAlCriminal && !misionRecuperarJoyasActiva)
                    AceptarMisionRecuperarJoyas();
                else if (misionRoboActiva)
                    RobarAnciana();
                break;

            case TipoNPC.AsesinoContratante:
                if (criminalEliminado && TieneArma())
                    EliminarAsesino();
                else if (joyasEntregadasAlCriminal)
                    AceptarMisionMatarObjetivo();
                else
                    ActualizarMensaje();
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
            textoMensaje.text = $"Robaste: {itemRobado}\t\tReputación: -2\n\nLlévale las joyas al criminal.";
        }

        ActivarAnimacionReaccion();

        misionRoboCompletada = true;
        misionRoboActiva = false;

        Invoke("CerrarPanel", 3f);
    }

    void EntregarJoyasAlCriminal()
    {
        if (!TieneItem(itemRobado))
        {
            ActualizarMensaje();
            return;
        }

        player.RemoveFromInv(itemRobado, 1);
        joyasEntregadasAlCriminal = true;

        Debug.Log($"Entregaste las {itemRobado} al criminal.");

        if (textoMensaje != null)
        {
            textoMensaje.text = $"Buen trabajo.\tGuardaré estas {itemRobado}.\n\nAhora ve con el asesino, te recomendé con él.";
        }

        Invoke("CerrarPanel", 3f);
    }

    // ========== MISIÓN 2: MATAR AL OBJETIVO (Para el Asesino) ==========
    void AceptarMisionMatarObjetivo()
    {
        if (!joyasEntregadasAlCriminal || misionMatarObjetivoActiva || misionMatarObjetivoCompletada)
        {
            ActualizarMensaje();
            return;
        }

        misionMatarObjetivoActiva = true;
        
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
        if (misionMatarObjetivoActiva && TieneArma())
        {
            if (textoMensaje != null)
            {
                textoMensaje.text = "Objetivo eliminado.\t\tMisión completada.\n\nVuelve con el asesino.";
            }

            Morir();

            misionMatarObjetivoCompletada = true;
            misionMatarObjetivoActiva = false;

            Invoke("CerrarPanelYLimpiarPlayer", 3f);
            return;
        }

        if (!misionMatarObjetivoActiva || misionMatarObjetivoCompletada || !TieneArma())
        {
            ActualizarMensaje();
            return;
        }
    }

    // ========== MISIÓN 3: RECUPERAR LAS JOYAS (Matar al Criminal) ==========
    void AceptarMisionRecuperarJoyas()
    {
        if (misionRecuperarJoyasActiva || misionRecuperarJoyasCompletada || !misionMatarObjetivoCompletada)
        {
            ActualizarMensaje();
            return;
        }

        misionRecuperarJoyasActiva = true;

        Debug.Log("La anciana pide recuperar las joyas del criminal.");

        if (textoMensaje != null)
        {
            textoMensaje.text = $"Misión aceptada.\n\nRecupera mis {itemRobado} del criminal.\nTienes el arma, usa la.";
        }

        Invoke("CerrarPanel", 2f);
    }

    void EliminarCriminal()
    {
        if (!misionRecuperarJoyasActiva || !TieneArma())
        {
            ActualizarMensaje();
            return;
        }

        // Recuperar las joyas
        player.AddToInv(itemRobado, 1);
        criminalEliminado = true;

        Debug.Log($"Eliminaste al criminal y recuperaste las {itemRobado}");

        if (textoMensaje != null)
        {
            textoMensaje.text = $"Criminal eliminado.\t\tRecuperaste: {itemRobado}\n\nPuedes devolverlas a la anciana o quedártelas/venderlas.";
        }

        Morir();

        Invoke("CerrarPanelYLimpiarPlayer", 3f);
    }

    void DevolverJoyasAnciana()
    {
        if (!criminalEliminado || !TieneItem(itemRobado))
        {
            ActualizarMensaje();
            return;
        }

        if (misionRecuperarJoyasCompletada)
        {
            if (textoMensaje != null)
            {
                textoMensaje.text = "Ya me devolviste mis joyas.\t\t¡Gracias de nuevo!";
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
            textoMensaje.text = $"¡Gracias!\tReputación: +5\tDinero: +$100\t¡Eres un héroe!";
        }

        misionRecuperarJoyasCompletada = true;

        Invoke("CerrarPanel", 3f);
    }

    // ========== MISIÓN EXTRA: ELIMINAR AL ASESINO ==========
    void EliminarAsesino()
    {
        if (!criminalEliminado || !TieneArma())
        {
            ActualizarMensaje();
            return;
        }

        if (textoMensaje != null)
        {
            textoMensaje.text = "Asesino eliminado.\t\tYa no quedan cabos sueltos.";
        }

        Morir();

        Invoke("CerrarPanelYLimpiarPlayer", 3f);
    }

    // ========== ROBAR A PERSONAS COMUNES ==========
    void RobarPersonaComun()
    {
        if (!misionRoboCompletada)
        {
            if (textoMensaje != null)
            {
                textoMensaje.text = "Primero debes completar la misión de la anciana.";
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

    // ========== MÉTODOS AUXILIARES ==========
    bool TieneArma()
    {
        if (player == null) return false;
        PlayerItem arma = player.inventario.Find(i => i.nombre == nombreArma);
        return arma != null && arma.cantidad > 0;
    }

    bool TieneItem(string nombreItem)
    {
        if (player == null) return false;
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
        joyasEntregadasAlCriminal = false;
        misionMatarObjetivoCompletada = false;
        misionMatarObjetivoActiva = false;
        misionRecuperarJoyasCompletada = false;
        misionRecuperarJoyasActiva = false;
        criminalEliminado = false;
        itemRobado = "";
    }
}