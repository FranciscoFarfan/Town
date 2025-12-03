using UnityEngine;

public class Caja : MonoBehaviour
{
    public NPCPescador pescador;
    private bool playerCerca = false;

    void Update()
    {
        if (playerCerca && Input.GetKeyDown(KeyCode.E))
        {
            pescador.TomoCana();
            gameObject.SetActive(false);
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

