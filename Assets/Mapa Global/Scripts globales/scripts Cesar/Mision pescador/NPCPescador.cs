using UnityEngine;
using TMPro;
using System.Collections;

public class NPCPescador : MonoBehaviour
{
    public TextMeshProUGUI dialogoUI;
    public GameObject panelDialogo;
    public GameObject zona;
    public Transform puntoPesca;
    public GameObject cajaCana;

    // NUEVAS REFERENCIAS PARA HACER DESAPARECER/APARECER
    public GameObject zonaFalsaPesca;  // La zona temporal donde haces la misión
    public GameObject zonaRealPesca;   // La zona verdadera que aparecerá después

    private bool eventoIniciado = false;
    public bool tieneCana = false;
    public bool tienePez = false;
    public bool puedePescar = false;
    private bool misionTerminada = false;

    private int esperar = 0;

    void OnTriggerEnter(Collider other)
    {
        if (!eventoIniciado && other.CompareTag("Player"))
        {
            eventoIniciado = true;
            IniciarDialogo();
        }
    }

    void Update()
    {
        if (esperar > 0 && Input.GetKeyDown(KeyCode.E))
        {
            if (esperar == 1) Teletransportar();

            esperar = 0;
        }
    }

    void IniciarDialogo()
    {
        panelDialogo.SetActive(true);
        dialogoUI.text =
            "Pescador: Hola, justo contigo quería hablar.\n" +
            "Pescar aquí es complicado pero tiene recompensa.\n" +
            "¿Me ayudas a pescar un pez?\n\n" +
            "Presiona E para continuar.";

        esperar = 1;
    }

    void Teletransportar()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        player.transform.position = puntoPesca.position;
        transform.position = puntoPesca.position + new Vector3(1f, 0, -1f);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        zona.SetActive(false);
        cajaCana.SetActive(true);

        dialogoUI.text = "Pescador: Agarra la caña de la caja.\nPresiona E para cerrar.";

        esperar = 2;
    }

    public void TomoCana()
    {
        tieneCana = true;
        puedePescar = true;

        panelDialogo.SetActive(true);
        dialogoUI.text = "Conseguiste: Caña de pescar.\nPresiona E para comenzar a pescar.";
        esperar = 2;
    }

    public void PescoPez()
    {
        tienePez = true;

        panelDialogo.SetActive(true);
        dialogoUI.text = "Pescaste un pez.\nPresiona E para hablar con el pescador.";
        esperar = 2;
    }

    void OnTriggerStay(Collider other)
    {
        if (eventoIniciado && tienePez && !misionTerminada && Input.GetKeyDown(KeyCode.E))
        {
            DialogoFinal();
        }
    }

    void DialogoFinal()
    {
        if (misionTerminada) return;

        panelDialogo.SetActive(true);
        StartCoroutine(TextoFinalSecuencia());
    }

    void FinalMision()
    {
        GameManager.Instance.AumentarReputacion(1);
        misionTerminada = true;
        panelDialogo.SetActive(false);

        // AQUÍ HACEMOS LA TRANSICIÓN
        StartCoroutine(TransicionZonaPesca());
    }

    IEnumerator TransicionZonaPesca()
    {
        // Esperar un momento antes de hacer el cambio
        yield return new WaitForSeconds(1f);

        // Desactivar objetos de la misión
        gameObject.SetActive(false);        // El pescador
        cajaCana.SetActive(false);          // La caja
        if (zonaFalsaPesca != null)
            zonaFalsaPesca.SetActive(false); // La zona temporal

        // Activar la zona real de pesca
        if (zonaRealPesca != null)
            zonaRealPesca.SetActive(true);  // La zona verdadera (cambié a true!)

        Debug.Log("Zona de pesca real activada");
    }

    IEnumerator TextoFinalSecuencia()
    {
        string texto1 =
            "Pescador: ¡Gracias por tu ayuda!\n" +
            "Quédate con la caña y el pez.\n" +
            "Ese pescado puedes venderlo en el pueblo.\n" +
            "+1 Reputación\n";

        yield return StartCoroutine(EfectoEscritura(texto1));
        yield return new WaitForSeconds(2f);

        string texto2 =
            "Pescador: Ahora que tienes tu caña,\n" +
            "puedes venir a pescar una vez al día.\n";

        yield return StartCoroutine(EfectoEscritura(texto2));
        yield return new WaitForSeconds(2f);

        FinalMision();
    }

    IEnumerator EfectoEscritura(string texto)
    {
        dialogoUI.text = "";
        foreach (char letra in texto)
        {
            dialogoUI.text += letra;
            yield return new WaitForSeconds(0.03f);
        }
    }
}