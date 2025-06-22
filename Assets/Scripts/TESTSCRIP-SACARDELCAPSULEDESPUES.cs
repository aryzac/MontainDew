using UnityEngine;

public class TESTSCRIPSACARDELCAPSULEDESPUES : MonoBehaviour
{
     [Header("Semillas para testeo")]
    public InventoryItemData semilla1;
    public InventoryItemData semilla2;
    [Tooltip("Cantidad inicial de cada semilla")]
    public int cantidadInicial = 5;

    void Start()
    {
        // Asegurar que el sistema existe antes de intentar agregar
        if (InventorySystem.Instance == null)
        {
            Debug.LogError("GameInitializer: no se encontró InventorySystem en la escena.");
            return;
        }

        // Agregar 'cantidadInicial' unidades de cada semilla
        for (int i = 0; i < cantidadInicial; i++)
        {
            InventorySystem.Instance.AddItem(semilla1);
            InventorySystem.Instance.AddItem(semilla2);
        }

        Debug.Log($"Inventario inicializado con {cantidadInicial}× Semilla1 y {cantidadInicial}× Semilla2.");
    }
}
