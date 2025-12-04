using UnityEngine;

/// <summary>
/// Hace que el Canvas siempre mire a la cámara (efecto billboard)
/// Asignar este script al Canvas que está sobre la cabeza del NPC
/// </summary>
public class Billboard : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        
        if (mainCamera == null)
        {
            Debug.LogWarning("No se encontró la cámara principal. El billboard no funcionará.");
        }
    }

    void LateUpdate()
    {
        if (mainCamera != null)
        {
            // Hacer que el canvas siempre mire a la cámara
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                           mainCamera.transform.rotation * Vector3.up);
        }
    }
}
