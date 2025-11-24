using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ConfessionInteractableUI : MonoBehaviour, IInteractuable
{
    public string npcName = "Sacerdote";
    public string prefsKey = "ConfesionDia"; // PlayerPrefs key

    private void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
        this.gameObject.tag = "Trigger";
    }

    public void Interaccion()
    {
        if (GameManager.instance == null)
        {
            Debug.LogWarning("[ConfessionUI] GameManager no encontrado.");
            return;
        }

        int rep = GameManager.instance.reputacion;

        // Si reputación no negativa -> informar
        if (rep >= 0)
        {
            DialogUIManager_TMP.Instance.ShowDialog(
                npcName,
                $"No necesitas confesarte. Tu reputación es {rep}.",
                null,
                ("Cerrar", null)
            );
            return;
        }

        // Check diario
        string last = PlayerPrefs.GetString(prefsKey, "");
        string today = System.DateTime.Today.ToString("yyyy-MM-dd");
        if (last == today)
        {
            DialogUIManager_TMP.Instance.ShowDialog(
                npcName,
                "Ya te confesaste hoy. Vuelve mañana si aún necesitas confesar.",
                null,
                ("Cerrar", null)
            );
            return;
        }

        // Mostrar confirmación
        string body = "¿Deseas confesarte? Esto aumentará tu reputación en +1. No obtendrás dinero.";
        DialogUIManager_TMP.Instance.ShowDialog(
            npcName,
            body,
            null,
            ("Confesar", () => {
                // Acción al confirmar
                GameManager.instance.reputacion += 1;
                PlayerPrefs.SetString(prefsKey, today);
                PlayerPrefs.Save();
                Debug.Log("[ConfessionUI] Confesión realizada. Reputación +1.");
                // Mensaje de agradecimiento/resultado
                DialogUIManager_TMP.Instance.ShowDialog(npcName, "Has confesado. Tu alma se siente un poco mejor. (+1 reputación)", null, ("Cerrar", null));
            }),
            ("Cancelar", null)
        );
    }
}
