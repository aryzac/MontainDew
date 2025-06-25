using UnityEngine;
using TMPro;

public class CrecimientoPlanta : MonoBehaviour
{
    public enum Estado { Virgen, Arada, Plantada, Lista }
    [HideInInspector] public Estado estado = Estado.Virgen;

    [Header("Referencias de Prefabs")]
    public GameObject prefabHojita;
    public GameObject prefabPlanta;

    [Header("Tiempos")]
    [Tooltip("Tiempo total en segundos para pasar de Plantada → Lista")]
    public float tiempoCrecimiento = 30f;
    [Tooltip("% de reducción de tiempo por cada riego (0–1)")]
    [Range(0f, 1f)]
    public float potenciaRiego = 0.2f;

    [Header("Recompensa")]
    public int dineroPorCosecha = 5;

    // Internos
    private float timerRestante;
    private GameObject instanciaHojita;
    private GameObject instanciaPlanta;
    private TextMeshProUGUI textoEstado;

    void Awake()
    {
        // Buscar el TextMeshProUGUI en hijos
        textoEstado = GetComponentInChildren<TextMeshProUGUI>();
        if (textoEstado == null)
            Debug.LogError("CrecimientoPlanta: no se encontró TextMeshProUGUI en los hijos.");

        // Inicializar estado
        SetEstado(Estado.Virgen);
    }

    void Update()
    {
        if (estado == Estado.Plantada)
        {
            // Cuenta regresiva y actualización de texto
            timerRestante -= Time.deltaTime;
            textoEstado.text = $"Germinando... ({timerRestante:0}s)";

            if (timerRestante <= 0f)
                SetEstado(Estado.Lista);
        }
    }

    /// <summary>
    /// Cambia de estado, actualiza texto e instancia visuales.
    /// </summary>
    void SetEstado(Estado nuevo)
    {
        // Destruir instancias anteriores
        if (instanciaHojita) Destroy(instanciaHojita);
        if (instanciaPlanta) Destroy(instanciaPlanta);

        estado = nuevo;

        switch (estado)
        {
            case Estado.Virgen:
                textoEstado.text = "Tierra virgen";
                break;

            case Estado.Arada:
                textoEstado.text = "Arada";
                break;

            case Estado.Plantada:
                timerRestante = tiempoCrecimiento;
                instanciaHojita = Instantiate(prefabHojita, transform);
                textoEstado.text = $"Germinando... ({timerRestante:0}s)";
                break;

            case Estado.Lista:
                instanciaPlanta = Instantiate(prefabPlanta, transform);
                textoEstado.text = "¡Listo para cosechar!";
                break;
        }
    }

    /// <summary>
    /// Arar: sólo si estamos en Virgen.
    /// </summary>
    public bool Arar()
    {
        if (estado != Estado.Virgen) return false;
        SetEstado(Estado.Arada);
        return true;
    }

    /// <summary>
    /// Plantar semilla: sólo si estamos Arada.
    /// </summary>
    public bool PlantarSemilla()
    {
        if (estado != Estado.Arada) return false;
        SetEstado(Estado.Plantada);
        return true;
    }

    /// <summary>
    /// Regar: sólo si estamos Plantada.
    /// </summary>
    public bool Regar()
    {
        if (estado != Estado.Plantada) return false;
        // Reducir tiempo restante
        float reduccion = tiempoCrecimiento * potenciaRiego;
        timerRestante = Mathf.Max(timerRestante - reduccion, 0f);
        // Actualizar texto inmediatamente
        textoEstado.text = $"Germinando... ({timerRestante:0}s)";
        return true;
    }

    /// <summary>
    /// Cosechar: sólo si estamos Lista.
    /// </summary>
    public bool Cosechar()
    {
        if (estado != Estado.Lista) return false;
        // Sumar dinero
        GameManager.Instance.generarDinero(dineroPorCosecha);
        // Volver a tierra virgen
        SetEstado(Estado.Virgen);
        return true;
    }
}
