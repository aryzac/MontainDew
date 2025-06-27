using System.Linq;
using UnityEngine;
using Cinemachine;

public enum TipoHerramienta { Ninguna = 1, Rastrillo = 2, Regadera = 3 }

public class PlayerInteraccion : MonoBehaviour
{
    [Tooltip("Distancia máxima de interacción con el macetero")]
    public float distanciaMax = 2f;

    [Tooltip("Lista de tipos de semilla válidos para plantar")]
    public InventoryItemData[] semillasData;

    [Tooltip("Herramienta activa: 1 = ninguna, 2 = rastrillo, 3 = regadera")]
    public TipoHerramienta herramientaActiva = TipoHerramienta.Ninguna;

    private Camera cam;
    private ToolController toolController;

    void Awake()
    {
        // Intentar obtener la cámara de Cinemachine
        var brain = Object.FindFirstObjectByType<CinemachineBrain>();
        if (brain != null && brain.OutputCamera != null)
            cam = brain.OutputCamera;
        else if (Camera.main != null)
            cam = Camera.main;
        else
            Debug.LogError("PlayerInteraccion: no se encontró ninguna cámara válida.");

        toolController = FindObjectOfType<ToolController>();
        if (toolController == null)
            Debug.LogError("PlayerInteraccion: no se encontró ToolController en la escena.");
    }

    void Update()
    {
        // Selección de herramienta: 1 = ninguna, 2 = rastrillo, 3 = regadera
        if (Input.GetKeyDown(KeyCode.Alpha1)) herramientaActiva = TipoHerramienta.Ninguna;
        if (Input.GetKeyDown(KeyCode.Alpha2)) herramientaActiva = TipoHerramienta.Rastrillo;
        if (Input.GetKeyDown(KeyCode.Alpha3)) herramientaActiva = TipoHerramienta.Regadera;

        // Interacción con E
        if (Input.GetKeyDown(KeyCode.E))
            IntentarInteractuar();
    }

    private void IntentarInteractuar()
    {
        if (cam == null) return;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        if (!Physics.Raycast(ray, out var hit, distanciaMax))
            return;

        var cp = hit.collider.GetComponent<CrecimientoPlanta>();

        var BoxCollided = hit.collider.GetComponentInParent<BuyTeleport>(); //CAMBIAR
        
        if (cp == null && BoxCollided == null) return;

        if (BoxCollided != null)
        {
            BoxCollided.ComprarPortal();
            return;
        }
        
        switch (cp.estado)
        {
            case CrecimientoPlanta.Estado.Virgen:
                if (herramientaActiva == TipoHerramienta.Rastrillo)
                { 
                    toolController.PlayRastrillar();
                    cp.Arar();
                }
                break;

            case CrecimientoPlanta.Estado.Arada:
                if (herramientaActiva == TipoHerramienta.Ninguna) PlantarConSemillaDisponible(cp);

                break;

            case CrecimientoPlanta.Estado.Plantada:
                if (herramientaActiva == TipoHerramienta.Regadera)
                {
                    toolController.PlayRegar();
                    cp.Regar();
                }
                break;

            case CrecimientoPlanta.Estado.Lista:
                cp.Cosechar();
                break;
        }
    }

    private void PlantarConSemillaDisponible(CrecimientoPlanta cp)
    {
        var inv = InventorySystem.Instance;
        InventoryItemData seleccionada = null;

        // Buscar primera semilla disponible
        foreach (var semData in semillasData)
        {
            var entry = inv.inventory.FirstOrDefault(i => i.data == semData);
            if (entry != null && entry.stackSize > 0)
            {
                seleccionada = semData;
                break;
            }
        }

        if (seleccionada == null)
        {
            NotificationManager.Instance.ShowMessage("No tienes semillas disponibles en el inventario.");
            return;
        }

        // Descontar semilla y plantar
        inv.RemoveItem(seleccionada);
        cp.PlantarSemilla();
    }
}
