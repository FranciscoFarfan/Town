using UnityEngine;
using TMPro;

public class Entregas : MonoBehaviour, IInteractuable
{
    [Header("Configuración")]
    public bool esNPCInicial = true; // true = da el paquete, false = recibe el paquete
    public Entregas[] npcsPosiblesDestino; // NPCs que pueden recibir el paquete
    
    [Header("UI")]
    public GameObject panelEntregas;
    public TextMeshProUGUI textoMensaje;
    
    private static bool misionActiva = false;
    private static string itemAEntregar = "";
    private static Entregas npcDestino = null;
    private static Entregas npcOrigen = null;
    
    private PlayerController player;
    private bool panelAbierto = false;
    
    public string TextoInteraccion => esNPCInicial ? "Solicitar entrega" : "Entregar paquete";

    void Start()
    {
        if (panelEntregas != null)
            panelEntregas.SetActive(false);
    }

    void Update()
    {
        if (panelAbierto)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                if (esNPCInicial)
                    IniciarMision();
                else
                    EntregarPaquete();
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
        if (panelEntregas != null)
        {
            panelEntregas.SetActive(true);
            panelAbierto = true;
            
            if (player != null)
                player.enabled = false;
        }
    }

    void CerrarPanel()
    {
        if (panelEntregas != null)
        {
            panelEntregas.SetActive(false);
            panelAbierto = false;
            
            if (player != null)
                player.enabled = true;
        }
    }

    void ActualizarMensaje()
    {
        string mensaje = "";

        if (esNPCInicial)
        {
            // NPC que da el paquete
            if (misionActiva)
            {
                mensaje = $"Ya tienes una entrega activa.\n\nDebes entregar: {itemAEntregar}\n\nDestino: {npcDestino.gameObject.name}";
            }
            else
            {
                mensaje = "Necesito que entregues un paquete.\n\nRecompensa:\nReputación: +2\nDinero: 10% del valor del objeto\n\nCuidado: Si lo vendes perderás reputación.";
            }
        }
        else
        {
            // NPC que recibe el paquete
            if (misionActiva && npcDestino == this)
            {
                mensaje = $"¿Traes mi paquete?\n\nDebes entregar: {itemAEntregar}";
            }
            else
            {
                mensaje = "No tengo ningún paquete esperando.";
            }
        }

        if (textoMensaje != null)
            textoMensaje.text = mensaje;
    }

    void IniciarMision()
    {
        GameManager gm = GameManager.Instance;

        if (misionActiva)
        {
            ActualizarMensaje();
            return;
        }

        if (npcsPosiblesDestino == null || npcsPosiblesDestino.Length == 0)
        {
            Debug.LogWarning("No hay NPCs de destino configurados.");
            return;
        }

        if (gm.baseDeDatos.Count == 0)
        {
            Debug.LogWarning("No hay items en la base de datos.");
            return;
        }

        ItemData itemAleatorio = gm.baseDeDatos[Random.Range(0, gm.baseDeDatos.Count)];
        itemAEntregar = itemAleatorio.nombre;

        npcDestino = npcsPosiblesDestino[Random.Range(0, npcsPosiblesDestino.Length)];
        npcOrigen = this;

        player.AddToInv(itemAEntregar, 1);

        misionActiva = true;

        Debug.Log($"Misión de entrega aceptada: Lleva {itemAEntregar} a {npcDestino.gameObject.name}");

        if (textoMensaje != null)
        {
            textoMensaje.text = $"Misión aceptada!\n\nEntrega: {itemAEntregar}\nDestino: {npcDestino.gameObject.name}\n\nNo lo vendas o perderás reputación!";
        }

        Invoke("CerrarPanel", 3f);
    }

    void EntregarPaquete()
    {
        if (!misionActiva || npcDestino != this)
        {
            Debug.Log("No tienes ningún paquete para entregar aquí.");
            ActualizarMensaje();
            return;
        }

        // Verificar que tenga el item
        PlayerItem item = player.inventario.Find(i => i.nombre == itemAEntregar);
        
        if (item == null || item.cantidad < 1)
        {
            if (textoMensaje != null)
            {
                textoMensaje.text = $"¡No tienes el paquete!\n\n¿Lo vendiste?\n\nPerdiste reputación por robar.";
            }
            
            // Penalización por vender el paquete
            player.LoseReputation(3);
            misionActiva = false;
            itemAEntregar = "";
            npcDestino = null;
            npcOrigen = null;
            
            Invoke("CerrarPanel", 3f);
            return;
        }

        // Remover item del inventario
        player.RemoveFromInv(itemAEntregar, 1);

        // Calcular recompensa (10% del valor del objeto)
        ItemData itemData = GameManager.Instance.baseDeDatos.Find(i => i.nombre == itemAEntregar);
        float recompensa = itemData != null ? itemData.precio * 0.1f : 10f;

        // Dar recompensas
        string repMsg = player.AddReputation(2);
        string dineroMsg = player.EarnMoney(recompensa);

        Debug.Log("¡Entrega completada!");
        Debug.Log(repMsg);
        Debug.Log(dineroMsg);

        if (textoMensaje != null)
        {
            textoMensaje.text = $"¡Entrega completada!\n\n{repMsg}\n{dineroMsg}\n\n¡Gracias!";
        }

        // Resetear misión
        misionActiva = false;
        itemAEntregar = "";
        npcDestino = null;
        npcOrigen = null;

        Invoke("CerrarPanel", 3f);
    }

    // Método para detectar si el jugador vendió el paquete
    public static void OnItemVendido(string nombreItem)
    {
        if (misionActiva && nombreItem == itemAEntregar)
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.LoseReputation(3);
                Debug.Log($"¡Vendiste el paquete que debías entregar! Perdiste 3 de reputación.");
            }

            misionActiva = false;
            itemAEntregar = "";
            npcDestino = null;
            npcOrigen = null;
        }
    }
}