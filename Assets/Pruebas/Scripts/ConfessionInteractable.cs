using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ConfessionInteractable : MonoBehaviour, IInteractuable
{
    public string npcName = "Confesor";

    private void Reset()
    {
        // Asegura las condiciones para interactuar
        var col = GetComponent<Collider>();
        col.isTrigger = true;
        this.gameObject.tag = "Trigger";
    }

    public void Interaccion()
    {
        // Verificar reputacion negativa
        if (GameManager.instance.reputacion >= 0)
        {
            Debug.Log($"[CONFESIÓN] No necesitas confesarte. Reputación actual: {GameManager.instance.reputacion}");
            return;
        }

        // Verificar si ya se confesó hoy
        string lastDate = PlayerPrefs.GetString("ConfesionDia", "");
        string today = System.DateTime.Today.ToString("yyyy-MM-dd");

        if (lastDate == today)
        {
            Debug.Log("[CONFESIÓN] Ya te confesaste hoy.");
            return;
        }

        // Aplicar confesión
        GameManager.instance.reputacion += 1;
        PlayerPrefs.SetString("ConfesionDia", today);
        PlayerPrefs.Save();

        Debug.Log("[CONFESIÓN] Te has confesado. Reputación +1");
    }
}
