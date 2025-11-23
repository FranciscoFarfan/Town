using UnityEngine;

public class DoorTriggerPoint : MonoBehaviour, IInteractuable
{
    public DoorTrigger doorParent;
    public bool isIndoor;

    void Start()
    {
        if (doorParent == null)
            doorParent = GetComponentInParent<DoorTrigger>();
    }

    public void Interaccion()
    {
        doorParent.Interaccion();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isIndoor)
                doorParent.SetIndoor(true);
            else
                doorParent.SetOutdoor(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isIndoor)
                doorParent.SetIndoor(false);
            else
                doorParent.SetOutdoor(false);
        }
    }
}
