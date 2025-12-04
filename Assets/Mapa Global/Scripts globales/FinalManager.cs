using UnityEngine;
using UnityEngine.Video;

public class FinalManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject videoCanvas;
    public float dineroAlto = 10000f;
    public float dineroBajo = 3000f;
    public float reputacionAlta = 75f;
    public float reputacionBaja = 25f;

    void Start()
    {
       
    }

    public void ReproducirFinal()
    {
        GameManager gm = GameManager.Instance;
        var stats = gm.GetStats();
        
        string tipoTransporte = "pie";
        if (stats.enBote)
        {
            tipoTransporte = "bote";
        }
        else if (stats.enCarro)
        {
            tipoTransporte = "carro";
        }
        
        int numeroFinal = CalcularNumeroFinal(stats.dinero, stats.reputacion);
        string nombreVideo = tipoTransporte + numeroFinal;
        
        CargarVideo(nombreVideo);
    }

    int CalcularNumeroFinal(float dinero, float reputacion)
    {
        int nivelDinero = 0;
        if (dinero >= dineroAlto)
        {
            nivelDinero = 0;
        }
        else if (dinero >= dineroBajo)
        {
            nivelDinero = 1;
        }
        else
        {
            nivelDinero = 2;
        }
        
        int nivelReputacion = 0;
        if (reputacion >= reputacionAlta)
        {
            nivelReputacion = 0;
        }
        else if (reputacion >= reputacionBaja)
        {
            nivelReputacion = 1;
        }
        else
        {
            nivelReputacion = 2;
        }
        
        return (nivelReputacion * 3) + nivelDinero + 1;
    }

    void CargarVideo(string nombreVideo)
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, "videos", nombreVideo + ".mp4");
        videoPlayer.url = path;
        
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = false;
        
        videoPlayer.loopPointReached += OnVideoTerminado;
        
        if (videoCanvas != null)
        {
            videoCanvas.SetActive(true);
        }
            
        videoPlayer.Play();
        
        Debug.Log("Reproduciendo final: " + nombreVideo);
    }

    void OnVideoTerminado(VideoPlayer vp)
    {
        Debug.Log("Video terminado");
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoTerminado;
        }
    }
}