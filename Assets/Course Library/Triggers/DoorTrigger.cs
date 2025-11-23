using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public Transform indoorPoint;
    public Transform outdoorPoint;

    private bool playerIsIndoor = false;
    private bool playerIsOutdoor = false;

    private Transform player;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    public void Interaccion()
    {
        if (playerIsIndoor)
        {
            Teleport(outdoorPoint);
        }
        else if (playerIsOutdoor)
        {
            Teleport(indoorPoint);
        }
    }

    private void Teleport(Transform destination)
    {
        Vector3 newPos = destination.position;
        newPos.y += 0.1f; 

        player.position = newPos;
    }

    // Estos métodos serán llamados por los triggers hijos
    public void SetIndoor(bool state)
    {
        playerIsIndoor = state;
    }

    public void SetOutdoor(bool state)
    {
        playerIsOutdoor = state;
    }
}
