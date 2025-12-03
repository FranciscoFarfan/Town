using UnityEngine;
using TMPro;

public class DialogoInicial : MonoBehaviour
{
    public TextMeshProUGUI textoUI;
    public GameObject panelDialogo;  

    private int indice = 0;

    private string[] dialogos = new string[]
    {
        "No puedo creer que sigo atrapado en este pueblo...",
        "Sé que hay algo más allá afuera esperando por mí.",
        "Si quiero salir de aquí, necesito dinero... y rápido.",
        "Tal vez alguien del pueblo necesite ayuda.",
        "Será mejor empezar por hablar con ese pescador."
    };

    void Start()
    {
        panelDialogo.SetActive(true);
        textoUI.text = dialogos[indice];
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            AvanzarDialogo();
        }
    }

    void AvanzarDialogo()
    {
        indice++;

        if (indice < dialogos.Length)
        {
            textoUI.text = dialogos[indice];
        }
        else
        {
            panelDialogo.SetActive(false); 
            this.enabled = false;
        }
    }
}
