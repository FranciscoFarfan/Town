using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class ItemData
{
    public string nombre;
    public float precio;
    [Range(1,3)]
    public int rareza;
    public GameObject prefab; // Prefab para spawnear en el mapa
}


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Tiempo del juego")]
    public float hora = 8f; // hora inicial
    public int dia = 1;
    public float velocidadTiempo = 1f; // multiplicador de avance

    [Header("Base de datos de ítems")]
    public List<ItemData> baseDeDatos = new List<ItemData>();

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Update()
    {
        // Avanza el tiempo
        hora += Time.deltaTime * velocidadTiempo;
        if (hora >= 24f)
        {
            hora = 0f;
            dia++;
        }
    }

    // 🔹 Método para spawnear ítems en el mapa
    public void SpawnItem(string nombre, Vector3 posicion)
    {
        ItemData item = baseDeDatos.Find(i => i.nombre == nombre);
        if (item != null && item.prefab != null)
        {
            Instantiate(item.prefab, posicion, Quaternion.identity);
        }
    }

}
