using UnityEngine;
using System.Diagnostics;

public class FinalManager : MonoBehaviour
{
    [Header("Configuración de Videos")]
    public string carpetaVideos = "videos"; // Carpeta dentro de StreamingAssets

    void Start()
    {
       
    }

    public void ReproducirFinal()
    {
        // Obtener el PlayerController para dinero y reputación
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player == null)
        {
            UnityEngine.Debug.LogError("No se encontró PlayerController en la escena.");
            return;
        }
        
        // Obtener el GameManager para el estado de vehículos
        GameManager gm = GameManager.Instance;
        var stats = gm.GetStats();
        
        UnityEngine.Debug.Log($"=== DEBUG FINAL ===");
        UnityEngine.Debug.Log($"Dinero: {player.dinero}");
        UnityEngine.Debug.Log($"Reputacion: {player.reputacion}");
        UnityEngine.Debug.Log($"En Carro: {stats.enCarro}");
        UnityEngine.Debug.Log($"En Bote: {stats.enBote}");
        
        string tipoTransporte = "pie";
        if (stats.enBote)
        {
            tipoTransporte = "bote";
        }
        else if (stats.enCarro)
        {
            tipoTransporte = "carro";
        }
        
        int numeroFinal = CalcularNumeroFinal(player.dinero, player.reputacion);
        string nombreVideo = tipoTransporte + numeroFinal;
        
        UnityEngine.Debug.Log($"Tipo Transporte: {tipoTransporte}");
        UnityEngine.Debug.Log($"Numero Final: {numeroFinal}");
        UnityEngine.Debug.Log($"Video a reproducir: {nombreVideo}");
        
        AbrirVideoEnReproductor(nombreVideo);
    }

    int CalcularNumeroFinal(float dinero, float reputacion)
    {
        // DINERO: Alto 300-500, Medio 100-299, Bajo < 100
        int nivelDinero = 0;
        if (dinero >= 300f)
        {
            nivelDinero = 0; // Alto (300-500)
        }
        else if (dinero >= 100f)
        {
            nivelDinero = 1; // Medio (100-299)
        }
        else
        {
            nivelDinero = 2; // Bajo (< 100)
        }
        
        // REPUTACIÓN: Alta > 20, Media -10 a 20, Baja < -10
        int nivelReputacion = 0;
        if (reputacion > 20f)
        {
            nivelReputacion = 0; // Alta
        }
        else if (reputacion >= -10f && reputacion <= 20f)
        {
            nivelReputacion = 1; // Media
        }
        else // reputacion < -10
        {
            nivelReputacion = 2; // Baja
        }
        
        UnityEngine.Debug.Log($"Nivel Dinero: {nivelDinero} (Dinero: {dinero})");
        UnityEngine.Debug.Log($"Nivel Reputacion: {nivelReputacion} (Reputacion: {reputacion})");
        
        return (nivelDinero * 3) + nivelReputacion + 1;
    }

    void AbrirVideoEnReproductor(string nombreVideo)
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, carpetaVideos, nombreVideo + ".mp4");
        
        if (System.IO.File.Exists(path))
        {
            UnityEngine.Debug.Log($"Abriendo video: {nombreVideo} en reproductor del sistema");
            UnityEngine.Debug.Log($"Ruta: {path}");
            
            // Abrir el video con el reproductor predeterminado de Windows
            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
        }
        else
        {
            UnityEngine.Debug.LogError($"No se encontró el video en: {path}");
        }
    }
}