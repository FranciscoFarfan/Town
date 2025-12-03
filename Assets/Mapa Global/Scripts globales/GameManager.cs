using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class ItemData
{
    public string nombre;
    public float precio;
    [Range(1,3)]
    public int rareza;
    public GameObject prefab; // Prefab para spawnear en el mapa
}


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("Oro y Reputacion")]
    public int oro;
    public int reputacion;


    [Header("Tiempo del juego")]
    public float hora = 8f; // hora inicial
    public int dia = 1;
    public float velocidadTiempo = 1f; // multiplicador de avance

    [Header("Avance rápido de tiempo")]
    public Camera camaraJugador; // Asignar en el inspector
    public Vector3 posicionCinematica = new Vector3(0, 10, 0); // Posición para la cinemática
    public float velocidadRapida = 10f; // Velocidad durante el avance rápido
    private bool avanzandoTiempo = false;

    [Header("Sonidos ambientales")]
    public AudioSource townSound;
    public AudioSource farmSounds;
    public AudioSource peopleSounds;
    public AudioSource forestSounds;
    public AudioSource seaSounds;

    [Header("Curvas de volumen (0-24 horas)")]
    public AnimationCurve curvaVolumenDiurno = AnimationCurve.EaseInOut(0, 0, 24, 0); // Para Town, People, Farm
    private float volumenMaximoDiurno = 1f;

    [Header("Base de datos de ítems")]
    public List<ItemData> baseDeDatos = new List<ItemData>();

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        // Configurar curva de volumen por defecto si no está configurada
        if (curvaVolumenDiurno.keys.Length <= 2)
        {
            curvaVolumenDiurno = new AnimationCurve(
                new Keyframe(0f, 0f),      // 00:00 - Silencio
                new Keyframe(6f, 0.2f),    // 06:00 - Empezando a despertar
                new Keyframe(8f, 0.6f),    // 08:00 - Actividad matutina
                new Keyframe(12f, 1f),     // 12:00 - Máxima actividad (mediodía)
                new Keyframe(18f, 0.8f),   // 18:00 - Tarde activa
                new Keyframe(20f, 0.4f),   // 20:00 - Atardeciendo
                new Keyframe(22f, 0.1f),   // 22:00 - Noche tranquila
                new Keyframe(24f, 0f)      // 24:00 - Silencio nocturno
            );
        }
    }

    void Update()
    {
        // Avanza el tiempo
        hora += Time.deltaTime * velocidadTiempo;
        if (hora >= 24f)
        {
            hora = 0f;
            dia++;
        }

        // Actualizar volúmenes de sonidos ambientales
        ActualizarVolumenesSonidos();
    }

    // 🔹 Actualizar volúmenes de sonidos según la hora del día
    void ActualizarVolumenesSonidos()
    {
        // Evaluar la curva en la hora actual (0-24)
        float factorVolumen = curvaVolumenDiurno.Evaluate(hora) * volumenMaximoDiurno;

        // Aplicar volumen a sonidos diurnos (Town, People, Farm)
        if (townSound != null)
            townSound.volume = factorVolumen;

        if (peopleSounds != null)
            peopleSounds.volume = factorVolumen;

        if (farmSounds != null)
            farmSounds.volume = factorVolumen;

    }

    // 🔹 Método para spawnear ítems en el mapa
    public void SpawnItem(string nombre, Vector3 posicion)
    {
        ItemData item = baseDeDatos.Find(i => i.nombre == nombre);
        if (item != null && item.prefab != null)
        {
            Instantiate(item.prefab, posicion, Quaternion.identity);
        }
    }
    
    public void AumentarReputacion(int cantidad)
    {
        reputacion += cantidad;
        Debug.Log($"[Reputación] Nueva reputación: {reputacion}");
    }

    public void AgregarOro(int cantidad)
    {
        oro += cantidad;
        Debug.Log($"[Oro] Nuevo oro: {oro}");
    }

    // 🔹 Función para avanzar rápidamente el tiempo (solo si es después de las 8 PM)
    public void IniciarAvanceRapido()
    {
        if (hora >= 20f && !avanzandoTiempo)
        {
            StartCoroutine(AvanzarTiempoRapido());
        }
        else if (hora < 20f)
        {
            Debug.Log("[Avance Rápido] Solo se puede usar después de las 8 PM (20:00)");
        }
        else
        {
            Debug.Log("[Avance Rápido] Ya se está avanzando el tiempo");
        }
    }

    private System.Collections.IEnumerator AvanzarTiempoRapido()
    {
        avanzandoTiempo = true;

        // Obtener referencia al PlayerController
        PlayerController playerController = FindFirstObjectByType<PlayerController>();
        
        // Desactivar el control del jugador
        if (playerController != null)
        {
            playerController.controleActivo = false;
        }

        // Guardar la posición y rotación original de la cámara
        Transform padreOriginal = camaraJugador.transform.parent;
        Vector3 posicionOriginal = camaraJugador.transform.localPosition;
        Quaternion rotacionOriginal = camaraJugador.transform.localRotation;

        // Desemparentar la cámara del jugador
        camaraJugador.transform.SetParent(null);

        // Mover la cámara a la posición cinemática
        camaraJugador.transform.position = posicionCinematica;
        // Hacer que la cámara mire hacia la ciudad
        camaraJugador.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

        // Esperar un frame para asegurar que la rotación se aplique
        yield return null;

        Debug.Log("[Avance Rápido] Iniciando avance rápido del tiempo...");

        // Guardar la velocidad original
        float velocidadOriginal = velocidadTiempo;

        // Aumentar la velocidad del tiempo
        velocidadTiempo = velocidadRapida;

        // Esperar hasta que sea de día (por ejemplo, las 6 AM)
        while (hora >= 20f || hora < 7f)
        {
            yield return null; // Esperar un frame
        }

        // Restaurar la velocidad del tiempo
        velocidadTiempo = velocidadOriginal;

        Debug.Log("[Avance Rápido] Avance rápido completado. Es de día.");

        // Regresar la cámara a su posición original
        camaraJugador.transform.SetParent(padreOriginal);
        camaraJugador.transform.localPosition = posicionOriginal;
        camaraJugador.transform.localRotation = rotacionOriginal;

        // Reactivar el control del jugador
        if (playerController != null)
        {
            playerController.controleActivo = true;
        }

        avanzandoTiempo = false;
    }

}
