using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public Transform indoor;
    public Transform outdoor;
    public KeyCode interactKey = KeyCode.E;

    private Transform currentTrigger;
    private PlayerController playerController;

    void Start()
    {
        if (indoor == null) indoor = transform.Find("Indoor");
        if (outdoor == null) outdoor = transform.Find("Outdoor");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.doorAvailable = true; // 🔔 Activa UI
            }

            // Detecta cuál trigger tocó
            if (other.transform.position == indoor.position)
                currentTrigger = indoor;
            else
                currentTrigger = outdoor;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && playerController != null)
        {
            playerController.doorAvailable = false; // 🔕 Desactiva UI
            currentTrigger = null;
            playerController = null;
        }
    }

    void Update()
    {
        if (playerController != null && currentTrigger != null)
        {
            if (Input.GetKeyDown(interactKey))
            {
                if (currentTrigger == indoor)
                    playerController.transform.position = outdoor.position;
                else
                    playerController.transform.position = indoor.position;
            }
        }
    }
}