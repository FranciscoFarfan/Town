using UnityEngine;

public class GenericTrigger : MonoBehaviour, IInteractuable
{
    public string TextoInteraccion => "Generico "; // texto dinámico
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interaccion()
    {
        Debug.Log("Interacción ejecutada!");
    }
}
