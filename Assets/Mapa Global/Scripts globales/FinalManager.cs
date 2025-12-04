using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class FinalManager : MonoBehaviour
{
    [Header("Referencias")]
    public VideoPlayer videoPlayer;
    public GameObject videoCanvas; // Canvas que contiene el VideoPlayer
    
    [Header("Umbrales de Dinero")]
    public float dineroAlto = 500f;
    public float dineroBajo = 0f;
    
    [Header("Umbrales de Reputación")]
    public float reputacionAlta = 30f;
    public float reputacionBaja = -20f;
    
    [Header("Ruta de Videos")]
    public string rutaVideos = "Videos/"; // Carpeta dentro de Resources

    private void Start()
    {
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();
            
        ReproducirFinal();
    }

    public void ReproducirFinal()
    {
        GameManager gm = GameManager.Instance;
        
        // Determinar el tipo de transporte
        string tipoTransporte = "pie";
        if (gm.IsInBoat)
            tipoTransporte = "bote";
        else if (gm.IsInCar)
            tipoTransporte = "carro";
        
        // Determinar el número del final (1-9)
        int numeroFinal = CalcularNumeroFinal(gm.dinero, gm.reputacion);
        
        // Construir el nombre del video
        string nombreVideo = $"{tipoTransporte}{numeroFinal}";
        
        // Cargar y reproducir el video
        CargarVideo(nombreVideo);
    }

    private int CalcularNumeroFinal(float dinero, float reputacion)
    {
        // Determinar nivel de dinero (columna)
        int nivelDinero;
        if (dinero >= dineroAlto)
            nivelDinero = 0; // Mucho dinero
        else if (dinero >= dineroBajo)
            nivelDinero = 1; // Dinero neutral
        else
            nivelDinero = 2; // Poco dinero
        
        // Determinar nivel de reputación (fila)
        int nivelReputacion;
        if (reputacion >= reputacionAlta)
            nivelReputacion = 0; // Mucha reputación
        else if (reputacion >= reputacionBaja)
            nivelReputacion = 1; // Reputación neutral
        else
            nivelReputacion = 2; // Mala reputación
        
        // Calcular número final (1-9)
        // Matriz 3x3:
        // 1 2 3  (mucha rep)
        // 4 5 6  (rep neutral)
        // 7 8 9  (mala rep)
        return (nivelReputacion * 3) + nivelDinero + 1;
    }

    private void CargarVideo(string nombreVideo)
    {
        // Opción 1: Cargar desde Resources
        VideoClip clip = Resources.Load<VideoClip>(rutaVideos + nombreVideo);
        
        if (clip != null)
        {
            videoPlayer.clip = clip;
        }
        else
        {
            // Opción 2: Cargar desde StreamingAssets (más común para videos)
            string path = System.IO.Path.Combine(Application.streamingAssetsPath, "videos", nombreVideo + ".mp4");
            videoPlayer.url = path;
        }
        
        // Configurar el VideoPlayer
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = false;
        videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
        
        // Evento cuando el video termina
        videoPlayer.loopPointReached += OnVideoTerminado;
        
        // Mostrar canvas y reproducir
        if (videoCanvas != null)
            videoCanvas.SetActive(true);
            
        videoPlayer.Play();
        
        Debug.Log($"Reproduciendo final: {nombreVideo}");
    }

    private void OnVideoTerminado(VideoPlayer vp)
    {
     
        Debug.Log("Fin del juego");
    
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoTerminado;
    }
}