using UnityEngine;

public class ZonaPesca : MonoBehaviour
{
    public NPCPescador pescador;
    private bool playerCerca = false;
    private bool yaPesco = false;
    
    void Update()
    {
        if (playerCerca && Input.GetKeyDown(KeyCode.E) && !yaPesco)
{
        if (pescador.tieneCana) // O mejor: pescador.puedePescar
        {
            yaPesco = true;
            pescador.PescoPez();
        }
    }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerCerca = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerCerca = false;
    }
}
