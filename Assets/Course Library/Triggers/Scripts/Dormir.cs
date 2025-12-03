using UnityEngine;

public class Dormir : MonoBehaviour, IInteractuable
{
    public string TextoInteraccion => "Dormir "; // texto dinámico
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interaccion()
    {
        GameManager.Instance.IniciarAvanceRapido();
    }
}
