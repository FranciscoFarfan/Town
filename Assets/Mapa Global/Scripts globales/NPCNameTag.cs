using UnityEngine;
using TMPro;

/// <summary>
/// Maneja el nombre/etiqueta del NPC que aparece sobre su cabeza
/// </summary>
public class NPCNameTag : MonoBehaviour
{
    [Header("Configuración")]
    public TipoNPC tipoNPC;
    public TextMeshProUGUI textoNombre;
    
    [Header("Nombres aleatorios para personas comunes")]
    private static readonly string[] nombresHombres = { 
        "Carlos", "Juan", "Pedro", "Luis", "Miguel", "José", "Antonio", 
        "Francisco", "Ricardo", "Roberto", "Fernando", "Alejandro", "Daniel"
    };
    
    private static readonly string[] nombresMujeres = { 
        "María", "Ana", "Carmen", "Rosa", "Laura", "Patricia", "Isabel", 
        "Sofía", "Elena", "Julia", "Beatriz", "Gabriela", "Victoria"
    };
    
    private static readonly string[] apellidos = { 
        "García", "Rodríguez", "Martínez", "López", "González", "Pérez", 
        "Sánchez", "Ramírez", "Torres", "Flores", "Rivera", "Gómez", "Díaz"
    };

    public enum TipoNPC
    {
        JefeCriminal,
        Anciana,
        AsesinoContratante,
        Objetivo,
        PersonaComun,
        Repartidor,
        Destinatario
    }

    void Start()
    {
        if (textoNombre == null)
        {
            Debug.LogWarning($"No se asignó TextMeshProUGUI en {gameObject.name}");
            return;
        }

        ActualizarNombre();
    }

    void ActualizarNombre()
    {
        string nombreMostrar = "";

        switch (tipoNPC)
        {
            case TipoNPC.JefeCriminal:
                nombreMostrar = "Criminal";
                textoNombre.color = new Color(0.8f, 0.2f, 0.2f); // Rojo oscuro
                break;

            case TipoNPC.Anciana:
                nombreMostrar = "Anciana";
                textoNombre.color = new Color(0.6f, 0.6f, 0.9f); // Azul claro
                break;

            case TipoNPC.AsesinoContratante:
                nombreMostrar = "Asesino";
                textoNombre.color = new Color(0.5f, 0.1f, 0.1f); // Rojo muy oscuro
                break;

            case TipoNPC.Objetivo:
                nombreMostrar = "Objetivo";
                textoNombre.color = new Color(0.9f, 0.5f, 0.1f); // Naranja
                break;

            case TipoNPC.Repartidor:
                nombreMostrar = "Repartidor";
                textoNombre.color = new Color(0.3f, 0.7f, 0.3f); // Verde
                break;

            case TipoNPC.Destinatario:
                nombreMostrar = GenerarNombreAleatorio();
                textoNombre.color = new Color(0.5f, 0.8f, 0.5f); // Verde claro
                break;

            case TipoNPC.PersonaComun:
                nombreMostrar = GenerarNombreAleatorio();
                textoNombre.color = new Color(0.9f, 0.9f, 0.9f); // Blanco
                break;
        }

        textoNombre.text = nombreMostrar;
    }

    string GenerarNombreAleatorio()
    {
        bool esHombre = Random.Range(0, 2) == 0;
        string nombre = esHombre 
            ? nombresHombres[Random.Range(0, nombresHombres.Length)]
            : nombresMujeres[Random.Range(0, nombresMujeres.Length)];
        
        string apellido = apellidos[Random.Range(0, apellidos.Length)];
        
        return $"{nombre} {apellido}";
    }

    /// <summary>
    /// Método público para cambiar el tipo de NPC en runtime si es necesario
    /// </summary>
    public void CambiarTipoNPC(TipoNPC nuevoTipo)
    {
        tipoNPC = nuevoTipo;
        ActualizarNombre();
    }
}
