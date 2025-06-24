using UnityEngine;

/// <summary>
/// Controllador visual de las herramientas
/// Metodos SelectTool, Y falta implementar las animaciones
/// </summary>
public class ToolController : MonoBehaviour
{
    [Header("Modelos de herramienta")]
    public GameObject manoVacia;
    public GameObject rastrillo;
    public GameObject regadera;

    //animaciones
    private Animator animRastrillo;
    private Animator animRegadera;
    bool isAnimating = false;

    void Awake()
    {
        // Buscamos el Animator dentro de cada prefab automáticamente:
        if (rastrillo != null)
            animRastrillo = rastrillo.GetComponentInChildren<Animator>();
        if (regadera != null)
            animRegadera = regadera.GetComponentInChildren<Animator>();

        // Opcional: comprobar errores
        if (animRastrillo == null) Debug.LogWarning("No se encontró Animator en rastrillo");
        if (animRegadera == null) Debug.LogWarning("No se encontró Animator en regadera");
    }

    void Start()
    {
        manoVacia.SetActive(true);
        rastrillo.SetActive(false);
        regadera.SetActive(false);
    }

    void Update()
    {
        // 1 = mano vacía, 2 = rastrillo, 3 = regadera
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectTool(manoVacia);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectTool(rastrillo);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectTool(regadera);
    }

    void SelectTool(GameObject herramienta)
    {
        manoVacia.SetActive(false);
        rastrillo.SetActive(false);
        regadera.SetActive(false);
        herramienta.SetActive(true);
    }

    /*IMplementar funciones para ejecutar animaciones*/
    public void PlayRastrillar() => animRastrillo.SetTrigger("Rastrillar");
    public void PlayRegar() => animRegadera.SetTrigger("RegarTrigger");
}

